using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class ClassRoom
    {
        public int selfid = -1;

        //教室信息
        public CourseData courseinfor = new CourseData();
        // 课件场景原始信息
        public SceneData originscene = new SceneData();

        public PlayerInScene teacher;
        public Dictionary<int, PlayerInScene> allstudents = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();
        public Dictionary<int, Team> groups = new Dictionary<int, Team>();
        public ArrayList _uuid_of_player = new ArrayList();

        private ArrayList _uuid_sync_cache = new ArrayList();

        // 这个uuid list 只是保存大屏显示的list  默认一个 
        // 模式里面不保存这个list 大屏控制操作 首先通过 classroom 过滤处理 
        public string _uuid_of_screen = string.Empty;

        // 模式控制相关
        public Enums.TeachingMode _modeltype = Enums.TeachingMode.WatchLearnModel_Sync;
        private int _modelindex = 0;
        private BaseModel[] _model;

        // 指令接收分发
        private CommandReceive _receiver = new CommandReceive();

        // 房间初始化
        public void InitRoom()
        {
            InitCommandListen();
        }

        public string FindUuid(int userid)
        {
            if(!allstudents.ContainsKey(userid))
            {
                return null;
            }

            return allstudents[userid].uuid;
        }

        public ArrayList FindUuidsExcept(int userid)
        {
            if (!allstudents.ContainsKey(userid))
            {
                return null;
            }

            ArrayList a = new ArrayList();

            foreach(PlayerInScene p in allstudents.Values)
            {
                if(p.selfid == userid)
                {
                    continue;
                }

                a.Add(p.uuid);
            }

            return a;
        }

        public int Enter(UserInfor user)
        {
            if (user == null)
            {
                return -1;
            }

            //if (user.isentercourse)
            //{
            //    // 重复进入房间
            //    // TODO
            //    //进行场景指令同步

            //    return false;
            //}

            user.islogin = true;
            user.isenterlobby = true;
            user = courseinfor.AddUserInfor(user);

            PlayerInScene player = new PlayerInScene();
            player.Init(user, this.selfid);

            if (player.teamid > -1)
            {
                // 小组添加人员
                if (groups.ContainsKey(player.teamid))
                {
                    groups[(player.teamid)].AddPlayer(player);
                }
            }

            int id = player.selfid;
            string uuid = player.uuid;
            if (this.allstudents.ContainsKey(id))
            {
                // 重复登陆  可能是掉线重登
                //TODO
                PlayerInScene p = this.allstudents[id];

                if (p.uuid != null)
                {
                    if (_uuid_of_player.Contains(p.uuid))
                    {
                        _uuid_of_player.Remove(p.uuid);
                    }
                }

                _uuid_of_player.Add(uuid);

                this.allstudents[id] = player;
            }
            else
            {
                this.allstudents.Add(id, player);

                if (uuid != null)
                {
                    if (!_uuid_of_player.Contains(uuid))
                    {
                        _uuid_of_player.Add(uuid);
                    }
                }
            }

            // 测试
            if (player.selfid == 1)
            {
                player.permission = Enums.PermissionEnum.Teacher;
            }

            if (player.permission == Enums.PermissionEnum.Teacher)
            {
                // 登陆者是老师
                this.teacher = player;
            }
            else
            {
                // 登陆者是学生
                TellTeacherPlayerIn(id);
            }

            Console.WriteLine("当前玩家 id : " + id);
            Console.WriteLine("当前玩家数 : " + allstudents.Count + " _uuid_of_player " + _uuid_of_player.Count);

            return selfid;
        }

        public void Leave(int userid)
        {
            PlayerInScene ps = null;
            int id = (int)userid;
            string uuid = string.Empty;
            if (allstudents.ContainsKey(userid))
            {
                ps = allstudents[userid];
                uuid = ps.uuid;

                allstudents.Remove(userid);
            }

            if (uuid != string.Empty && _uuid_of_player.Contains(uuid))
            {
                _uuid_of_player.Remove(uuid);
            }

            if (allstudents.Count <= 0)
            {
                Clear();

                RoomManager.getInstance().DeleteRoomByNameById(this.selfid);

                Console.WriteLine("清除房间 : " + this.selfid);

                return;
            }

            // 玩家离开释放 玩家锁住的物体
            if (ps != null)
            {
                int teamid = ps.teamid;
                if(teamid >= 0)
                {
                    if(groups.ContainsKey(teamid))
                    {
                        groups[teamid].PlayerLeave(userid);
                    }
                }

                if(_modelindex >= 0 && _modelindex < _model.Length)
                {
                    _model[_modelindex].PlayerLeave(userid);
                }
            }
        }

        public void Clear()
        {
            _syncPipeData = false;

            allstudents.Clear();
            allobjects.Clear();
            groups.Clear();
            _uuid_of_player.Clear();

            RemoveCommandListen();
        }

        //模式初始化  在进入课件的时候初始化
        private void InitModel()
        {
            if(_model == null)
            {
                _model = new BaseModel[7];
            }

            _model[0] = new WatchLearnModelSync();
            _model[1] = new WatchLearnModelAsync();
            _model[2] = new GuidanceModePersonal();
            _model[3] = new GuidanceModeGroup();
            _model[4] = new SelfTrainPersonal();
            _model[5] = new SelfTrainGroup();
            _model[6] = new SelfTrainAll();

            _modelindex = 0;
            _model[0].InitModel(new Object[] { teacher, _uuid_of_player, allobjects });
            _model[0].StartSynclient();
        }

        // 初始化指令监听函数  只处理服务器需要处理的消息
        private void InitCommandListen()
        {
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.InitScene, InitScene);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Hold, Hold);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Release, Release);

            // 大屏显示 classroom 过滤
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.BigScreen, BigScreen);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.BigScreen, BigScreen);

            // 课程资料推送 推题相关
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataAll, PushCourseDataAll);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.PushDataAll, PushCourseDataAll);
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataOne, PushCourseDataOne);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.PushDataOne, PushCourseDataOne);

            // 答题 打开文件 等交互
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenContent, OpenContent);
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.VideoCtrl, VideoCtrl);
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PPtCtrl, PPtCtrl);
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenPPt, OpenPPt);

        }

        private void RemoveCommandListen()
        {
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.InitScene, InitScene);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Hold, Hold);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Release, Release);

            // 大屏显示 classroom 过滤
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.BigScreen, BigScreen);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.BigScreen, BigScreen);

            // 课程资料推送 推题相关
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataAll, PushCourseDataAll);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.PushDataAll, PushCourseDataAll);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataOne, PushCourseDataOne);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.PushDataOne, PushCourseDataOne);

            // 答题 打开文件 等交互
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenContent, OpenContent);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.VideoCtrl, VideoCtrl);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PPtCtrl, PPtCtrl);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
            _receiver.RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenPPt, OpenPPt);

        }

        // 指令操作
        public void Command(Int64 userid, ArrayList msg)
        {
            if(!_receiver.Receive((int)userid,msg))
            {
                // 服务器没有需要处理的消息 则直接广播

                // 现在根据模式广播
                if (_modelindex < 0 || _model[_modelindex] == null)
                {
                    Console.WriteLine("消息广播给所有玩家");
                    if (_uuid_of_player.Count > 0)
                    {
                        hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.Command_func, userid,(msg));
                    }
                }
                else
                {
                    Console.WriteLine("消息传递给当前模式" + _modelindex);
                    _model[_modelindex].Commond(userid, msg);
                }
            }
            
        }

        public void ChangeClientAllOnce(int userid, Hashtable data)
        {
            Console.WriteLine("同步场景数据" + data.Count);
            _model[_modelindex].CheckChangeObjectAllOnce<string>(userid, data, string.Empty);
        }

        // 服务器处理函数
        public void InitScene(int userid, ArrayList msg)
        {
            if(teacher == null || userid != teacher.selfid)
            {
                return;
            }

            Hashtable data = (Hashtable)msg[2];

            Console.WriteLine("初始化服务器场景数据 InitModel" + data.Count);

            InitModel();

            Console.WriteLine("初始化服务器场景数据" + data.Count);

            foreach (DictionaryEntry de in data)
            {
                int id = Convert.ToInt32(de.Key);
                if (allobjects.ContainsKey(id))
                {
                    allobjects[id].Change3DInfor((Hashtable)de.Value);
                }
                else
                {
                    ObjectInScene s = new ObjectInScene();
                    s.selfid = id;
                    s.Change3DInfor((Hashtable)de.Value);
                    allobjects.Add(s.selfid, s);
                }
            }

            Console.WriteLine("初始化服务器场景数据完毕");
        }

        public void ChangeModel(int userid, ArrayList msg)
        {
            Int64 modelid = (Int64)msg[2];

            ArrayList args = null;
            if (msg.Count > 3)
            {
                args = (ArrayList)msg[3];
            }
            
            Enums.TeachingMode tm = (Enums.TeachingMode)modelid;

            if(_modeltype == tm)
            {
                return;
            }

            int oldmodelid = _modelindex;

            switch (tm)
            {
                case Enums.TeachingMode.WatchLearnModel_Sync:
                    _modelindex = 0;
                    _model[0].InitModel(new Object[] { teacher, _uuid_of_player, allobjects });
                    _model[0].StartSynclient();
                    break;
                case Enums.TeachingMode.WatchLearnModel_Async:
                    _modelindex = 1;
                    _model[1].InitModel(new Object[] { teacher, allstudents, allobjects });
                    _model[1].StartSynclient();
                    break;
                case Enums.TeachingMode.GuidanceMode_Personal:
                    if(args == null)
                    {
                        return;
                    }
                    Int64 person = (Int64)args[0];
                    if (!allstudents.ContainsKey((int)person))
                    {
                        return;
                    }
                    _modelindex = 2;
                    _model[2].InitModel(new Object[] { teacher, (int)person, allstudents, allobjects });
                    _model[2].StartSynclient();
                    break;
                case Enums.TeachingMode.GuidanceMode_Group:
                    if (args == null)
                    {
                        return;
                    }
                    Int64 groupid = (Int64)args[0];
                    if(groups == null || !groups.ContainsKey((int)groupid))
                    {
                        return;
                    }
                    _modelindex = 3;
                    _model[3].InitModel(new Object[] { teacher, groups[(int)groupid].allstudents, allstudents, allobjects });
                    _model[3].StartSynclient();
                    break;
                case Enums.TeachingMode.SelfTrain_Personal:
                    if (args == null)
                    {
                        return;
                    }
                    Int64 p = (Int64)args[0];
                    if(!allstudents.ContainsKey((int)p))
                    {
                        return;
                    }
                    _modelindex = 4;
                    _model[4].InitModel(new Object[] { teacher, allstudents[(int)p], allobjects });
                    _model[4].StartSynclient();
                    break;
                case Enums.TeachingMode.SelfTrain_Group:
                    if (args == null)
                    {
                        return;
                    }
                    Int64 g = (Int64)args[0];
                    if (groups == null || !groups.ContainsKey((int)g))
                    {
                        return;
                    }
                    _modelindex = 5;
                    _model[5].InitModel(new Object[] { teacher, groups[(int)g]});
                    _model[5].StartSynclient();
                    break;
                case Enums.TeachingMode.SelfTrain_All:
                    _modelindex = 6;
                    _model[6].InitModel(new Object[] { teacher, allstudents, allobjects});
                    _model[6].StartSynclient();
                    break;
                default:
                    break;
            }

            _model[oldmodelid].StopSynclient();

            _modeltype = tm;

            //hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retOnlinePlayer", (modelid));

            Console.WriteLine("切换教学模式" + modelid);
        }

        public void Hold(int userid, ArrayList msg)
        {
            Int64 ibjectid = (Int64)msg[2];
            _model[_modelindex].CheckOperationHold<string>(userid, (int)ibjectid, string.Empty);

            Console.WriteLine("Hold");
        }

        public void Release(int userid, ArrayList msg)
        {
            Int64 ibjectid = (Int64)msg[2];
            _model[_modelindex].CheckOperationRelease<string>(userid, (int)ibjectid, string.Empty);

            Console.WriteLine("Release");
        }

        public void BigScreen(int userid, ArrayList msg)
        {
            hub.hub.gates.call_client(_uuid_of_screen, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, (msg));

            Console.WriteLine("BigScreen");
        }

        // 课程资料 题目相关
        // 获取题目数据
        public void QuestionList(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if (allstudents.Count <= 0 || !allstudents.ContainsKey(userid))
            {
                return;
            }

            string questiones = string.Empty;

            if(courseinfor != null)
            {
                questiones = courseinfor._questioninfor;
            }

            if(questiones == null || questiones == string.Empty)
            {
                BackDataService.getInstance().GetCourseQuestionList(allstudents[userid].token, Question_List_Succeed, Question_List_Failure, userid.ToString());
            }
            else
            {
                msg.Add(questiones);
                hub.hub.gates.call_client(allstudents[userid].uuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
            }
        }

        // 暂存题目数据
        public void Question_List_Succeed(BackDataType.QuestionInforRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allstudents.ContainsKey(id))
                {
                    return;
                }

                PlayerInScene user = allstudents[id];

                // 转换编码格式
                if(jsondata != null)
                {
                    jsondata = JsonDataHelp.getInstance().EncodeBase64(null, jsondata);

                    if(courseinfor != null && courseinfor._questioninfor == string.Empty)
                    {
                        courseinfor._questioninfor = jsondata;
                    }

                    // 广播 所有学生 测验题信息
                    if (_uuid_of_player.Count > 0)
                    {
                        ArrayList msg = new ArrayList();
                        msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
                        msg.Add((Int64)CommandDefine.SecondLayer.QuestionList);
                        msg.Add(jsondata);
                        hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.Command_func, (Int64)id, msg);
                    }
                }
            }
            catch
            {

            }
        }

        public void Question_List_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allstudents.ContainsKey(id))
                {
                    return;
                }

                //PlayerInScene user = sceneplaylistbyid[id];

                //hub.hub.gates.call_client(user.uuid, "cMsgConnect", "retAcquireQuestionList", id, "null");
            }
            catch
            {

            }
        }

        // 获取课程资料列表
        public void MaterialItemList(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if(msg == null || msg.Count <= 2)
            {
                return;
            }

            Int64 courseid = (Int64)msg[2];

            BackDataService.getInstance().GetCourseMaterialList(allstudents[userid].token, (int)courseid, Material_List_Succeed, Material_List_Failure, userid.ToString());
        }

        // 暂存题目数据
        public void Material_List_Succeed(BackDataType.MaterialItemInforRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allstudents.ContainsKey(id))
                {
                    return;
                }

                PlayerInScene user = allstudents[id];

                // 转换编码格式
                if (jsondata != null)
                {
                    jsondata = JsonDataHelp.getInstance().EncodeBase64(null, jsondata);

                    if (courseinfor != null && courseinfor._materiallist == string.Empty)
                    {
                        courseinfor._materiallist = jsondata;
                    }

                    ArrayList msg = new ArrayList();
                    msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
                    msg.Add((Int64)CommandDefine.SecondLayer.MaterialList);
                    msg.Add(jsondata);

                    hub.hub.gates.call_client(user.uuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)id, msg);
                }
            }
            catch
            {

            }
        }

        public void Material_List_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allstudents.ContainsKey(id))
                {
                    return;
                }

                //PlayerInScene user = allstudents[id];

                //hub.hub.gates.call_client(user.uuid, "cMsgConnect", "retMaterialItemList", (Int64)id, "null");
            }
            catch
            {

            }
        }

        // 在线玩家列表
        public void OnlinePlayers(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.uuid == null || this.teacher.selfid != userid)
            {
                return;
            }

            ArrayList players = new ArrayList();
            foreach (PlayerInScene p in this.allstudents.Values)
            {
                if (p.selfid == userid)
                {
                    continue;
                }

                players.Add((Int64)p.selfid);
            }

            msg.Add(players);
            hub.hub.gates.call_client(this.teacher.uuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
        }

        // 学生上线 告诉老师
        public void TellTeacherPlayerIn(int userid)
        {
            // 告诉老师 学生上线
            if (this.teacher != null && this.teacher.uuid != null)
            {
                ArrayList msg = new ArrayList();
                msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
                msg.Add((Int64)CommandDefine.SecondLayer.OnlineOnePlayer);
                msg.Add((Int64)userid);
                hub.hub.gates.call_client(this.teacher.uuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
            }
        }

        // 资料推送
        private void PushCourseDataOne(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            // 保存推送记录  待实现
            //classinfor.AddMaterialPushed((int)fileid);

            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            string uuid = this.teacher.uuid;
            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();

        }

        private void PushCourseDataAll(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            if(msg == null || msg.Count <= 2)
            {
                return;
            }

            // 保存推送全部资料记录
            //classinfor.AddMaterialPushed((int)fileid);

            string uuid = this.teacher.uuid;

            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();
        }

        // 答题 打开文件 等交互
        public void OpenContent(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            string uuid = teacher.uuid;
            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();
        }

        //public void EndFastQuestion(int userid, ArrayList msg)
        //{
        //    if (this.teacher == null || this.teacher.selfid != userid)
        //    {
        //        return;
        //    }

        //    if(msg == null || msg.Count <= 2)
        //    {
        //        return;
        //    }

        //    Int64 target = (Int64)msg[2];
        //    int targetid = (int)target;

        //    if (!allstudents.ContainsKey(targetid))
        //    {
        //        return;
        //    }

        //    string targetuuid = allstudents[targetid].uuid;

        //    hub.hub.gates.call_client(targetuuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
        //}

        public void VideoCtrl(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            string uuid = teacher.uuid;
            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();
        }

        public void PPtCtrl(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            string uuid = teacher.uuid;
            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();
        }

        private void TestInClass(int userid, ArrayList msg)
        {
            if (this.teacher == null)
            {
                return;
            }

            if(userid == teacher.selfid)
            {
                string uuid = teacher.uuid;
                if (_uuid_sync_cache.Count > 0)
                {
                    _uuid_sync_cache.Clear();
                }

                for (int i = 0; i < _uuid_of_player.Count; i++)
                {
                    if ((string)_uuid_of_player[i] == uuid)
                    {
                        continue;
                    }

                    _uuid_sync_cache.Add(_uuid_of_player[i]);
                }

                hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

                _uuid_sync_cache.Clear();
            }
            else
            {
                hub.hub.gates.call_client(teacher.uuid, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
            }
            
        }

        private void OpenPPt(int userid, ArrayList msg)
        {
            if (this.teacher == null || this.teacher.selfid != userid)
            {
                return;
            }

            string uuid = teacher.uuid;
            if (_uuid_sync_cache.Count > 0)
            {
                _uuid_sync_cache.Clear();
            }

            for (int i = 0; i < _uuid_of_player.Count; i++)
            {
                if ((string)_uuid_of_player[i] == uuid)
                {
                    continue;
                }

                _uuid_sync_cache.Add(_uuid_of_player[i]);
            }

            hub.hub.gates.call_group_client(_uuid_sync_cache, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);

            _uuid_sync_cache.Clear();
        }

        // 同步管道
        private ContinuousPipe _pipe = new ContinuousPipe();

        public void ReceivePipeData(int fromid, int toid, Hashtable data)
        {
            _pipe.Receive(fromid, toid, data);

            if(!_syncPipeData)
            {
                _syncPipeData = true;
                SyncPipeData(0);
            }
        }

        // 教室同步
        private bool _syncPipeData = false;
        public void SyncPipeData(long tick)
        {
            _pipe.SyncClient();

            if (_syncPipeData)
            {
                hub.hub.timer.addticktime(200, SyncPipeData);
            }
        }
    }
}
