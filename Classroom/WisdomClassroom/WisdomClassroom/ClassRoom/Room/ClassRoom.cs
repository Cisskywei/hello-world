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

        // 模式控制相关
        public Enums.TeachingMode _modeltype = Enums.TeachingMode.WatchLearnModel_Sync;
        private int _modelindex = 0;
        private BaseModel[] _model;

        // 指令接收分发
        private CommandReceive _receiver = new CommandReceive();

        // 房间初始化
        public void InitRoom()
        {
            IninCommandListen();
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

            // 测试
            if(player.selfid == 1)
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
            allstudents.Clear();
            allobjects.Clear();
            groups.Clear();
            _uuid_of_player.Clear();
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
        private void IninCommandListen()
        {
            _receiver.AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.ChangeMode, ChangeModel);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.InitScene, InitScene);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Hold, Hold);
            _receiver.AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.Release, Release);
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
                        hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.Command_func, (msg));
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

    }
}
