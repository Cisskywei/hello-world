using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using System.Threading;

namespace veClassRoom.Room
{
    /// <summary>
    /// 真正的房间 包括大厅、场景
    /// </summary>
    class RealRoom : BaseRoomClass
    {

        public ClassData classinfor = new ClassData();

        public Dictionary<int, GroupInRoom> grouplist = new Dictionary<int, GroupInRoom>();

        // 场景的 初始化的数据   用于重置场景
        private SceneData _originalscene = new SceneData();

        ////////////////////////////////////////////////////      场景初始化、同步、销毁等操作模块      ///////////////////////////////////////////////

        public override void CreateScene(string name)
        {
            base.CreateScene(name);
            this.model = Enums.TeachingMode.WatchLearnModel_Sync;
        }

        public void CreateScene(int id)
        {
            this.sceneid = id;

            this.isinitclass = false;
            this.model = Enums.TeachingMode.WatchLearnModel_Sync;
        }

        /// <summary>
        /// 老师打开vr课件才会有的初始化场景
        /// </summary>
        /// <param name="data"></param>
        public override void InitScenes(Hashtable data)
        {
            base.InitScenes(data);

            // 初始化原始服务器数据
            this._originalscene.InitSceneData(this.moveablesceneobject, this.sceneplaylistbyid, this.sceneorderlist);

            isinitclass = true;

            Console.WriteLine("初始化服务器");
        }

        // 离开房间后的重置
        public void ResetClassRoom()
        {
            ClearScene();
            grouplist.Clear();
            _originalscene.ClearSceneData();
            classinfor.ClearClassData();
        }

        public void PlayerInitSelf3DInfor(Int64 id, Hashtable infor)
        {
            int userid = (int)id;

            if (!this.sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            this.sceneplaylistbyid[userid].InitPlayerHeadHand(infor);
        }

        // 限于场景课件的开始
        public void BeginClass(Int64 id)
        {
            int userid = (int)id;

            Console.WriteLine("开始上课  ： " + userid + istartclass);
            if (istartclass)
            {
                return;
            }
            if (leader == null)
            {
                Console.WriteLine("老师没有进入教室 ： ");
                return;
            }

            try
            {
                if (!sceneplaylistbyid.ContainsKey(userid))
                {
                    Console.WriteLine("服务器不包含该玩家 ： " + userid);
                    return;
                }

                PlayerInScene p = sceneplaylistbyid[userid];
                if (p.selfid != leader.selfid)
                {
                    Console.WriteLine("进入教室的不是老师 无权进行开始上课的操作");
                    return;
                }

            }
            catch
            {

            }
            if (!isinitclass)
            {
                return;
            }
            // 进行一次场景同步
            SceneSynchronizationAll();

            StartSyncClient();

            istartclass = true;

            Console.WriteLine("开始上课  并且启动同步线程： " + userid);
        }

        ////////////////////////////////////////////////////      场景初始化、同步、销毁等操作模块   End   ///////////////////////////////////////////////

        ////////////////////////////////////////////////////      向后台服务器获取玩家信息、验证登陆、反馈数据等操作模块      ///////////////////////////////////////////////

        // 获取房间所有报名者的信息
        public void GetAllPlayerInfor()
        {
        }

        public void ReplyPlayerInforToService()
        {
            // 向服务器保存玩家数据
        }

        public void ReplyPlayerInforToService(UserInfor playerinfor)
        {
            // 向服务器保存玩家数据
        }

        public void ReplyPlayerInforToService(PlayerInScene player)
        {
            // 向服务器保存玩家数据
        }

        private void CheckPlayerInfor(string token, string name, string uuid)
        {
            UserInfor playerinfor = BackDataService.getInstance().CheckUser(token, name);

            if (playerinfor == null)
            {
                return;
            }

            createPlayer(playerinfor);
        }

        private void CheckPlayerInforLocal(string token, string name, string uuid)
        {
        }

        ////////////////////////////////////////////////////      向后台服务器获取玩家信息、验证登陆、反馈数据等操作模块  End    ///////////////////////////////////////////////


        ////////////////////////////////////////////////////      玩家登陆进入房间、离开、玩家创建等操作模块      ///////////////////////////////////////////////

        // 进入智慧教室
        public int PlayerEnterScene(UserInfor user)
        {
            //       CheckPlayerInfor(token, name, uuid);
            //CheckPlayerInforLocal(token, name, uuid);

            // 跳过服务器验证 只为测试
            createPlayer(user);

            // 只为测试
            for(int i=0; i<10; i++)
            {
                UserInfor ui = new UserInfor();
                ui.selfid = i + 100;
                ui.user_id = (i + 100).ToString();
                ui.access_token = "token" + i;
                ui.user_name = "name" + i;
                createPlayer(ui);
            }

            if(istartclass && (user.identity != "teacher"))
            {
                SceneSynchronizationPlayer(user.uuid);
            }

            // 告诉老师 学生上线
            if(this.leader != null)
            {
                hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retOnlinePlayer", (Int64)(user.selfid));
            }

            return this.sceneid;
        }

        public void PlayerLeaveScene(Int64 id, string uuid)
        {
            PlayerInScene ps = null;
            int userid = (int)id;
            if (sceneplaylistbyid.ContainsKey(userid))
            {
                ps = sceneplaylistbyid[userid];

                sceneplaylistbyid.Remove(userid);
            }

            if (_uuid_of_player.Contains(uuid))
            {
                _uuid_of_player.Remove(uuid);
            }

            if(sceneplaylistbyid.Count <= 0)
            {
                isactive = false;

                ClearScene();

                RoomManager.getInstance().DeleteRoomByNameById(this.sceneid);

                Console.WriteLine("清除房间 : " + this.scenename);

                return;
            }

            // 玩家离开释放 玩家锁住的物体
            if(ps != null)
            {
                foreach (ObjectInScene so in moveablesceneobject.Values)
                {
                    if (!so.locked)
                    {
                        continue;
                    }

                    if (so.locker == ps.token)
                    {
                        so.locker = null;
                        so.locked = false;
                        so.lockpermission = 0;

                        so.physical.useGravity = true;

                        so.changeorno = true;
                    }
                }
            }
        }

        private void createPlayer(UserInfor playerinfor)
        {
            if(playerinfor == null)
            {
                return;
            }

            if (playerinfor.isentercourse)
            {
                // 重复进入房间
                // TODO
                //进行场景指令同步

                return;
            }

            playerinfor.islogin = true;
            playerinfor.isentercourse = true;
            playerinfor = classinfor.AddUserInfor(playerinfor);

            PlayerInScene player = new PlayerInScene(playerinfor, this.sceneid);

            if(player.groupid > -1)
            {
                if(grouplist.ContainsKey(player.groupid))
                {
                    grouplist[(player.groupid)].AddMember(player);
                }
            }

            Console.WriteLine("服务器房间模式 : " + this.model);

            // 切换玩家自身模式
            if(player!=null)
            {
                player.ChangePlayerModel(this.model);
            }

            if (player.isleader)
            {
                // 登陆者是老师
                this.leader = player;
                Console.WriteLine("老师登陆 ： " + player.token);
            }else
            {
                // 登陆者是学生
            }

            int id = playerinfor.selfid;
            string uuid = playerinfor.uuid;
            if (this.sceneplaylistbyid.ContainsKey(id))
            {
                // 重复登陆  可能是掉线重登
                //TODO
                PlayerInScene p = this.sceneplaylistbyid[id];

                if(p.uuid != null)
                {
                    if (_uuid_of_player.Contains(p.uuid))
                    {
                        _uuid_of_player.Remove(p.uuid);
                    }

                    _uuid_of_player.Add(uuid);
                }

                this.sceneplaylistbyid[id] = player;

                Console.WriteLine("用户重复登陆 : " + player.name);
            }
            else
            {
                this.sceneplaylistbyid.Add(id, player);

                if(uuid != null)
                {
                    if (!_uuid_of_player.Contains(uuid))
                    {
                        _uuid_of_player.Add(uuid);
                    }
                }
            }
            Console.WriteLine("当前玩家 id : " + id);
            Console.WriteLine("当前玩家数 : " + sceneplaylistbyid.Count + " _uuid_of_player " + _uuid_of_player.Count);

            // 通知客户端 玩家进入 教学大厅

            //// 通知客户端显示登陆信息
            //string msg = playerinfor.user_name + "登陆进入房间";
            //TellClientMsg(this._uuid_of_player, msg);
        }

        ////////////////////////////////////////////////////      玩家登陆进入房间、离开、玩家创建等操作模块   End   ///////////////////////////////////////////////


        ////////////////////////////////////////////////////      具体和客户端的操作      ///////////////////////////////////////////////

        public override void Switch_Model(int userid, Int64 tomodel, string uuid)
        {
            if(this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if(sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                Console.WriteLine("服务器玩家不存在 : " + " sceneplaylist count : " + sceneplaylistbyid.Count);
                return;
            }

            if(!checkLeaderFeasible(sceneplaylistbyid[userid]))
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 : " + " tomodel: " + tomodel);
                return;
            }

            Console.WriteLine("切换模式成功 : " + " tomodel: " + tomodel + "  开始更改玩家状态");

            Enums.TeachingMode m = (Enums.TeachingMode)(tomodel);

            if (m != this.model)
            {
                this.model = m;

                try
                {
                    foreach (PlayerInScene player in sceneplaylistbyid.Values)
                    {
                        player.ChangePlayerModel(this.model);
                    }
                }
                catch
                {

                }

            }

            //if (_uuid_of_player.Count > 0)
            //{
            //    hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_change_one_model", userid, tomodel);
            //}

            Console.WriteLine("更改了当前房间所有玩家模式成功 " + " tomodel: " + tomodel);
        }

        public override void Req_Object_Operate_permissions(Int64 id, string objectname, string uuid)
        {
            int userid = (int)id;

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            PlayerInScene ps = findPlayerById(userid);
            if(ps == null)
            {
                return;
            }

            // 先判断是否是小组模式
            if(checkIsInGroup())
            {
                Console.WriteLine("是小组内操作");
                //先判断是否有小组
                if (ps.group != null)
                {
                    ps.group.Req_Object_Operate_permissions(userid, objectname, uuid);

                    return;
                }
            }

            // 没有小组执行以下操作

            //具体的操作控制
            bool ret = false;

            do
            {
                if (!(checkOperateFeasible(ps) || ps.isCanOperate))
                {
                    Console.WriteLine("Req_Object_Operate_permissions 1 无权操作 : " + "token : " + userid);
                    break;
                }

                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + userid + "请求操作物体" + objectname + "失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = moveablesceneobject[objectname];
                if (o.locked)
                {
                    if (o.locker == ps.token)
                    {
                        Console.WriteLine("token : " + ps.token + "重复请求操作物体" + objectname);
                        break;
                    }

                    if(o.lockpermission < ps.permission)
                    {
                        o.locker = ps.token;
                        o.locked = true;
                        o.lockpermission = ps.permission;

                        o.physical.useGravity = false;

                        o.changeorno = true;

                        ret = true;

                        Console.WriteLine("token : " + ps.token + "权限较高夺取了 token  " + o.locker + " 的物体 " + objectname);
                        break;
                    }

                    break;
                }
                else
                {
                    o.locker = ps.token;
                    o.locked = true;
                    o.lockpermission = ps.permission;

                    o.physical.useGravity = false;

                    o.changeorno = true;

                    ret = true;
                }

            } while (false);

            if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objectname, "hold", ret ? "yes" : "no");
            }
        }

        public override void Req_Object_Release_permissions(Int64 id, string objectname, string uuid)
        {
            int userid = (int)id;

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            PlayerInScene ps = findPlayerById(userid);
            if (ps == null)
            {
                return;
            }

            // 先判断是否是小组模式
            if (checkIsInGroup())
            {
                Console.WriteLine("是小组内操作");
                // 先判断是否有小组
                if (ps.group != null)
                {
                    ps.group.Req_Object_Release_permissions(userid, objectname, uuid);

                    return;
                }
            }

            // 没有小组执行以下操作

            //具体的操作控制
            bool ret = false;
            do
            {
                if (!(checkOperateFeasible(ps) || ps.isCanOperate))
                {
                    Console.WriteLine("Req_Object_Release_permissions 1 无权操作 : " + "token : " + userid);
                    break;
                }

                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + userid + "请求释放物体" + objectname + "操作权限失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = moveablesceneobject[objectname];
                if (!o.locked)
                {
                    Console.WriteLine("token : " + userid + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体锁已被释放");
                    break;
                }
                else if (o.locker != ps.token)
                {
                    Console.WriteLine("token : " + ps.token + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体被 token : " + o.locker + "锁住");
                    break;
                }
                else
                {
                    o.locker = null;
                    o.locked = false;
                    o.lockpermission = Enums.PermissionEnum.None;

                    // 设置重力
                    o.physical.useGravity = true;

                    o.changeorno = true;

                    ret = true;
                }

            } while (false);

            if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objectname, "release", ret ? "yes" : "no");
            }
        }

        public override void ret_sync_commond(string typ, string commond, Int64 id, string other, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int userid = (int)id;

            if (sceneplaylistbyid.Count <= 0|| !sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            if (_uuid_of_player == null || _uuid_of_player.Count <= 0)
            {
                return;
            }

            try
            {
                _uuid_of_player.Remove(uuid);
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_sync_commond", typ, commond, userid, other);
                _uuid_of_player.Add(uuid);
            }
            catch
            {

            }

            // 指令缓存
            sceneorderlist.Add(new OrderInScene(0, userid, typ,commond,other));  //后期加入时间机制
        }

        public override void ChangeClientAllOnce(Int64 userid, Hashtable clientallonce)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            int id = (int)userid;

            PlayerInScene ps = findPlayerById(id);
            if (ps == null)
            {
                return;
            }

            if(checkIsInGroup())
            {
                if(ps.group != null)
                {
                    ps.group.ChangeClientAllOnce(userid, clientallonce);
                    return;
                }
            }

            if (!(checkOperateEffective(ps) || ps.isCanSend || ps.isbechoosed))
            {
                Console.WriteLine("无权操作 : " + "token : " + userid);
                return;
            }

            do
            {
                Hashtable player = (Hashtable)clientallonce["player"];
                Hashtable objects = (Hashtable)clientallonce["objects"];

                if (player != null && player.Count > 0)
                {
                    sceneplaylistbyid[id].InitPlayerHeadHand(player);
                }

                if (objects != null && objects.Count > 0)
                {
                    ChangeObjectAllOnce(id, objects);
                }

            } while (false);
        }

        public void ret_sync_group_commond(string typ, string commond, Int64 id, string other, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int userid = (int)id;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            PlayerInScene ps = findPlayerById(userid);
            if (ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if (ps.group != null)
            {
                ps.group.ret_sync_commond(typ, commond, userid, other, uuid);

                return;
            }

            // 没有小组执行以下操作

            ret_sync_commond(typ, commond, userid, other, uuid);
        }

        Hashtable msgObject = new Hashtable();
        Hashtable msgPlayer = new Hashtable();
        public override void SyncClient(Int64 tick)
        {
            // 同步数据
            foreach (ObjectInScene so in moveablesceneobject.Values)
            {
                if (!so.changeorno)
                {
                    continue;
                }

                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (PlayerInScene sp in sceneplaylistbyid.Values)
            {
                if (!sp.changeorno)
                {
                    continue;
                }

                // 同步客户端
                msgPlayer.Add(sp.selfid, sp.Serialize());
            }

            // 同步指令
            //TODO

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0)
            {
                try
                {

                    if (_uuid_sync_cache.Count > 0)
                    {
                        _uuid_sync_cache.Clear();
                    }

                    foreach (PlayerInScene p in sceneplaylistbyid.Values)
                    {
                        if (p.isCanReceive && p.uuid != null)
                        {
                            _uuid_sync_cache.Add(p.uuid);
                        }
                    }

                }
                catch
                {

                }

                if (_uuid_sync_cache.Count > 0)
                {
                    hub.hub.gates.call_group_client(_uuid_sync_cache, "cMsgConnect", "SyncClient", msgObject, msgPlayer);

                    Console.WriteLine("同步客户端数据 _uuid_sync_cache : " + tick);

                    _uuid_sync_cache.Clear();
                }
            }

            // 同步之后清除
            msgObject.Clear();
            msgPlayer.Clear();

            if (_syncstate)
            {
                hub.hub.timer.addticktime(400, SyncClient);
            }
        }


        // 动态修改 玩家 权限 等操作
        // TODO
        // 更改某一玩家的模式状态
        public void Change_One_Model(Int64 id, Int64 tomodel, string uuid, Int64 target)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int userid = (int)id;
            int targetid = (int)target;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            if(!sceneplaylistbyid.ContainsKey(targetid))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[userid]))
            {
                // 权限不够
                return;
            }

            PlayerInScene ps = sceneplaylistbyid[targetid];
            this.model = (Enums.TeachingMode)(tomodel);
            ps.ChangePlayerModel(this.model);

            hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "ret_change_one_model", targetid, tomodel);
        }

        // 更改某些玩家的模式状态
        public void Change_Some_Model(Int64 id, Int64 tomodel, ArrayList someid)
        {
            int userid = (int)id;

            if (someid == null || someid.Count <= 0)
            {
                return;
            }

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[userid]))
            {
                // 权限不够
                return;
            }

            this.model = (Enums.TeachingMode)(tomodel);

            ArrayList uuidcache = new ArrayList();
            foreach(int t in someid)
            {
                if(!sceneplaylistbyid.ContainsKey(t))
                {
                    continue;
                }

                PlayerInScene ps = sceneplaylistbyid[t];
                ps.ChangePlayerModel(this.model);

                uuidcache.Add(ps.uuid);
            }

            if(uuidcache.Count > 0)
            {
                hub.hub.gates.call_group_client(uuidcache, "cMsgConnect", "ret_change_one_model", userid, tomodel);
            }

            uuidcache.Clear();
        }


        ////////////////////////////////////////////////////   小组 管理相关     ///////////////////////////////////////////////////////////////////////////////////
        // 分组函数
        private Hashtable divideGroupOnAverage()
        {
            if(classinfor == null || classinfor.courseinfor == null)
            {
                return null;
            }

            Dictionary<int, UserInfor> students = classinfor.allstudents;

            if (students == null || students.Count <= 0)
            {
                return null;
            }

            Hashtable players = new Hashtable();

            int count = students.Count;
            // 10人以下分组算法
            int groupcount = count / 10 + 1;
            int membercount = count / groupcount;
            int leftmember = count % groupcount;

            string groupname = "组";

            int cgroup = 0;
            int cmember = 1;
            GroupInRoom girr = null;
            if (grouplist.ContainsKey(cgroup))
            {
                girr = grouplist[cgroup];
            }
            else
            {
                girr = new GroupInRoom(groupname + (cgroup + 1));
            }
            foreach (PlayerInScene ps in sceneplaylistbyid.Values)
            {
                if (ps.selfid == this.leader.selfid)
                {
                    continue;
                }

                girr.AddMember(ps);
                ps.group = girr;

                players.Add(ps.selfid, girr.name);

                cmember++;

                if(leftmember>0)
                {
                    if (cmember > membercount + 1)
                    {
                        if(grouplist.ContainsKey(cgroup))
                        {
                            grouplist[cgroup] = girr;
                        }
                        else
                        {
                            grouplist.Add(cgroup, girr);
                        }
                        cgroup++;

                        if (grouplist.ContainsKey(cgroup))
                        {
                            girr = grouplist[cgroup];
                        }
                        else
                        {
                            girr = new GroupInRoom(groupname + (cgroup + 1));
                        }

                        cmember = 1;
                    }

                    leftmember--;
                }
                else if(cmember > membercount)
                {
                    if (grouplist.ContainsKey(cgroup))
                    {
                        grouplist[cgroup] = girr;
                    }
                    else
                    {
                        grouplist.Add(cgroup, girr);
                    }
                    cgroup++;

                    if (grouplist.ContainsKey(cgroup))
                    {
                        girr = grouplist[cgroup];
                    }
                    else
                    {
                        girr = new GroupInRoom(groupname + (cgroup + 1));
                    }

                    cmember = 1;
                }
            }

            int id = 0;
            foreach (UserInfor uu in students.Values)
            {
                id = Convert.ToInt32(uu.user_id);

                if(id == this.leader.selfid)
                {
                    continue;
                }

                if (sceneplaylistbyid.ContainsKey(id))
                {
                    UserInfor u = classinfor.FindUserInforById(id);
                    PlayerInScene ps = sceneplaylistbyid[id];
                    if (u != null)
                    {
                        u.groupid = ps.groupid;
                        u.groupname = ps.groupname;
                    }

                    continue;
                }

                players.Add(id, girr.name);
                uu.groupid = cgroup;
                uu.groupname = girr.name;

                cmember++;

                if (leftmember > 0)
                {
                    if (cmember > membercount + 1)
                    {
                        if (grouplist.ContainsKey(cgroup))
                        {
                            grouplist[cgroup] = girr;
                        }
                        else
                        {
                            grouplist.Add(cgroup, girr);
                        }

                        cgroup++;

                        if (grouplist.ContainsKey(cgroup))
                        {
                            girr = grouplist[cgroup];
                        }
                        else
                        {
                            girr = new GroupInRoom(groupname + (cgroup + 1));
                        }

                        cmember = 1;
                    }

                    leftmember--;
                }
                else if (cmember > membercount)
                {
                    if (grouplist.ContainsKey(cgroup))
                    {
                        grouplist[cgroup] = girr;
                    }
                    else
                    {
                        grouplist.Add(cgroup, girr);
                    }

                    cgroup++;

                    if (grouplist.ContainsKey(cgroup))
                    {
                        girr = grouplist[cgroup];
                    }
                    else
                    {
                        girr = new GroupInRoom(groupname + (cgroup + 1));
                    }

                    cmember = 1;
                }
            }

            if(!grouplist.ContainsKey(cgroup))
            {
                grouplist.Add(cgroup, girr);
            }

            // 测试输出
            foreach(DictionaryEntry di in players)
            {
                Console.WriteLine(di.Key + "  ---  分组 " + di.Value);
            }

            return players;
        }

        public void DivideGroup(Int64 id, string rules)
        {
            int userid = (int)id;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                Console.WriteLine("服务器玩家不存在 : " + "userid : " + userid + " sceneplaylist count : " + sceneplaylistbyid.Count);
                return;
            }

            if(userid != this.leader.selfid)
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 不是老师不可分组 : " + "userid : " + userid);
                return;
            }

            // 按成绩分组
            //        divideGroupByGrade();
            // 平均分组
            Hashtable players = divideGroupOnAverage();
            if(this.leader != null)
            {
                hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retDivideGroup", id, players);
            }
        }

        /// 选择某一学生 或者某一小组 操作 全班同学观看
        /// 仅限于观学模式
        public void ChooseOneOrGroupOperate(Int64 id, string groupname, bool isgroup = false)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int userid = (int)id;

            // 当前模式必须是   指导模式才行
            if (this.model != Enums.TeachingMode.WatchLearnModel_Async || this.model != Enums.TeachingMode.WatchLearnModel_Sync)
            {
                return;
            }

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[userid]))
            {
                // 权限不够
                return;
            }

            if(isgroup)
            {
                int groupid = Convert.ToInt32(groupname);
                if(grouplist.Count <= 0 || !grouplist.ContainsKey(groupid))
                {
                    Console.WriteLine("当前房间不存在该小组 : " + groupname);
                    return;
                }

                // 组内协作 并且广播 给 所有玩家 
                // TODO
                // 1,改变 组内成员权限 可操作 可同步
                GroupInRoom gir = grouplist[groupid];
                if(gir == null)
                {
                    return;
                }

  //              gir.SwitchModelTo(Enums.ModelEnums.Collaboration);
                // 2,向所选择小组注入观看成员
                gir.InjectiveViewer(_uuid_of_player);

                // 通知该小组成员 被选中
   ///             gir.TellPlayerBeSelected(token, name);
            }
            else
            {
                // 改变一个人的权限 并且广播
                int targetid = Convert.ToInt32(groupname);
                if (!sceneplaylistbyid.ContainsKey(targetid))
                {
                    return;
                }

                // 学生可操作 并且广播给所有学生 但是 老师可介入
                //TODO
                // 1,特殊改变某一学生的操作开关
                PlayerInScene ps = sceneplaylistbyid[targetid];
                if(ps == null)
                {
                    return;
                }

                ps.ChangePlayerCanOperate(this.leader.permission, true);
                ps.ChangePlayerCanSend(this.leader.permission, true);
                ps.ChangePlayerCanReceive(this.permission, true);

                // 通知学生他被选中
                hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "ret_choose_one_operate", userid, groupname);
            }
        }

        /////////////////////////////////////////   和客户端交互需要用到的 辅助函数  （私有）     //////////////////////////////////////////////////////////////////////////////
        // TODO


        ////////////////////////////////////////     具体和客户端的界面操作有关的 rpc函数   与UI界面 交互接口      /////////////////////////////////////////////////////////////

        /// <summary>
        /// 教学模式切换操作
        /// </summary>
        /// <param name="token">操作者token</param>
        /// <param name="mode">切换的教学模式 TeachingMode 对应教学5大模式：观学模式、指导模式，自主训练、视频点播、视频直播</param>
        /// <param name="target">对应操作类型的操作对象：个人、组、、全部、空</param>
        public void SwitchTeachMode(Int64 userid, Int64 mode, Int64 isgroup, string target = null)
        {
            Console.WriteLine("客户端切换模式 UI " + "mode" + mode + "target" + target);

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[id]))
            {
                // 权限不够
                return;
            }

            Enums.TeachingMode tm = (Enums.TeachingMode)mode;

            switch(tm)
            {
                case Enums.TeachingMode.WatchLearnModel_Sync:
                   // break;
                case Enums.TeachingMode.WatchLearnModel_Async:
                    this.model = tm;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            player.ChangePlayerModel(this.model);
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.GuidanceMode_Personal:
                    if (target == null)
                    {
                        break;
                    }
                    int targetuserid = Convert.ToInt32(target);
                    if(!sceneplaylistbyid.ContainsKey(targetuserid))
                    {
                        Console.WriteLine("GuidanceMode_Personal UI 服务器玩家不存在 : " + "target : " + target);
                        return;
                    }
                    this.model = tm;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            player.ChangePlayerModel(this.model);
                            if (player.selfid == targetuserid)
                            {
                                player.ChangePlayerCanSend(this.leader.permission, true);
                                player.ChangePlayerCanReceive(this.leader.permission, true);
                                player.ChangePlayerCanOperate(this.leader.permission, true);
                                player.isbechoosed = true;
                            }
                            else
                            {
                                player.isbechoosed = false;
                            }
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.GuidanceMode_Group:
                    if (target == null)
                    {
                        break;
                    }
                    int targetid = Convert.ToInt32(target);
                    if (grouplist.Count <= 0 || !grouplist.ContainsKey(targetid))
                    {
                        Console.WriteLine("GuidanceMode_Personal UI 服务器分组不存在 : " + "小组名字 : " + target);
                        return;
                    }
                    this.model = tm;
                    GroupInRoom gir = grouplist[targetid];
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            if(!gir.HasMember(player.selfid))
                            {
                                player.ChangePlayerModel(this.model);
                                player.isbechoosed = false;
     //                           gir.InjectiveViewer(player.token);
                            }
                            else
                            {
                                player.ChangePlayerModel(this.model);
                                player.ChangePlayerCanSend(this.leader.permission, true);
                                player.ChangePlayerCanReceive(this.leader.permission, true);
                                player.ChangePlayerCanOperate(this.leader.permission, true);
                                player.isbechoosed = true;
                            }
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_Personal:
                    this.model = tm;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            player.ChangePlayerModel(this.model);
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_Group:   // 开启小组内部同步模式 初始化小组场景数据
                    this.model = tm;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            player.ChangePlayerModel(this.model);
                        }

                        foreach(GroupInRoom g in grouplist.Values)
                        {
                            g.InitSceneData(this.moveablesceneobject, this.sceneplaylistbyid);
                            g.StartSyncClient();
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_All:
                    this.model = tm;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylistbyid.Values)
                        {
                            player.ChangePlayerModel(this.model);
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.VideoOnDemand_General:
                    break;
                case Enums.TeachingMode.VideoOnDemand_Full:
                    break;
                case Enums.TeachingMode.VideoOnLive_General:
                    break;
                case Enums.TeachingMode.VideoOnLive_Full:
                    break;
                default:
                    break;
            }

            if(!(this.model == Enums.TeachingMode.GuidanceMode_Group || this.model == Enums.TeachingMode.SelfTrain_Group))
            {
                foreach (GroupInRoom g in grouplist.Values)
                {
                    g.StopSyncClient();
                    // 进行场景数据同步和老师的一样
                }
            }

            // 回复客户端
            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retSwitchTeachMode", userid, mode, target);
            }

            // 打印输出 学生模式
            Console.WriteLine("服务器当前模式   --------------------   : " + this.model);
            foreach (PlayerInScene pls in sceneplaylistbyid.Values)
            {
                Console.WriteLine(pls.selfid + "当前模式" + pls.model);
                if(pls.group != null)
                {
                    Console.WriteLine(pls.selfid + "所在小组" + pls.group.name);
                }
                Console.WriteLine(pls.selfid + "isCanOperate" + pls.isCanOperate);
                Console.WriteLine(pls.selfid + "isCanReceive" + pls.isCanReceive);
                Console.WriteLine(pls.selfid + "isCanSend" + pls.isCanSend);
                Console.WriteLine("\n");
            }
        }

        // 获取题目数据
        public void AcquireQuestionList(Int64 userid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[id]))
            {
                // 权限不够
                Console.WriteLine("UI 获取题目列表 权限不够 : " + "userid : " + userid);

                if (this.questionjsondata != null)
                {
                    hub.hub.gates.call_client(sceneplaylistbyid[id].uuid, "cMsgConnect", "retAcquireQuestionList", id, this.questionjsondata);
                }

                return;
            }

            if(this.questionjsondata != null)
            {
                hub.hub.gates.call_client(sceneplaylistbyid[id].uuid, "cMsgConnect", "retAcquireQuestionList", id, this.questionjsondata);
            }
            else
            {
                BackDataService.getInstance().GetCourseQuestionList(sceneplaylistbyid[id].token, Question_List_Succeed, Question_List_Failure, userid.ToString());
            }
        }

        // 暂存题目数据
        private string questionjsondata = null;
        public void Question_List_Succeed(BackDataType.QuestionInforRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!sceneplaylistbyid.ContainsKey(id))
                {
                    return;
                }

                PlayerInScene user = sceneplaylistbyid[id];

                // 转换编码格式

                if(jsondata != null)
                {
                    jsondata = JsonDataHelp.getInstance().EncodeBase64(null, jsondata);

                    this.questionjsondata = jsondata;

                    // 广播 所有学生 测验题信息
                    if (_uuid_of_player.Count > 0)
                    {
                        hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retAcquireQuestionList", id, jsondata);
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
                if (!sceneplaylistbyid.ContainsKey(id))
                {
                    return;
                }

                PlayerInScene user = sceneplaylistbyid[id];

                hub.hub.gates.call_client(user.uuid, "cMsgConnect", "retAcquireQuestionList", id, "null");
            }
            catch
            {

            }
        }

        /// <summary>
        /// 老师随堂测验的操作， 记得区分 在vr课件里 还是 在 学习大厅
        /// </summary>
        /// <param name="token">操作者token</param>
        /// <param name="typ">随堂测验的类型</param>
        /// <param name="question">具体问题信息id</param>
        /// <param name="other">额外的参数信息</param>
        public void InClassTest(Int64 userid, Int64 typ, Int64 questionid, string other = null)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[id]))
            {
                // 权限不够
                return;
            }

            //Enums.InClassTestType ctt = (Enums.InClassTestType)typ;

            //switch(ctt)
            //{
            //    case Enums.InClassTestType.Test:
            //        break;
            //    case Enums.InClassTestType.Fast:
            //        break;
            //    case Enums.InClassTestType.Ask:
            //        break;
            //    default:
            //        break;
            //}

            // 初始化 对应的 class类

            // 只为测试抢答
            if(typ == 2)
            {
                isfastquestionbegin = 1;
            }
            else
            {
                isfastquestionbegin = 0;
            }

            // 广播 所有学生 测验题
            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retInClassTest", userid, typ, questionid, other);
            }
        }

        /// <summary>
        /// 重置场景相关
        /// </summary>
        /// <param name="token">操作者token</param>
        /// <param name="typ">重置对象类型：全部、组、个人</param>
        /// <param name="target">具体重置对象信息：名字</param>
        public void ResetScene(Int64 userid, Int64 typ, string target)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[id]))
            {
                // 权限不够
                Console.WriteLine("UI 教学模式切换操作 权限不够 : " + "userid : " + userid);
                return;
            }

            Enums.OperatingRange or = (Enums.OperatingRange)typ;

            switch(or)
            {
                case Enums.OperatingRange.Personal:
                    if(target == null)
                    {
                        return;
                    }
                    int targetid = Convert.ToInt32(target);

                    if (!sceneplaylistbyid.ContainsKey(targetid))
                    {
                        Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + target);
                        return;
                    }
                    PlayerInScene ps = sceneplaylistbyid[targetid];
                    hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "retResetScene", userid);
                    break;
                case Enums.OperatingRange.Team:
                    if (target == null)
                    {
                        return;
                    }

                    int teamid = Convert.ToInt32(target);
                    if (!grouplist.ContainsKey(teamid))
                    {
                        Console.WriteLine("UI 服务器小组不存在 : " + "token : " + target);
                        return;
                    }
                    GroupInRoom gir = grouplist[teamid];
                    hub.hub.gates.call_group_client(gir._uuid_of_player, "cMsgConnect", "retResetScene", userid);
                    break;
                case Enums.OperatingRange.All:
                    if(_uuid_of_player.Count > 0)
                    {
                        hub.hub.gates.call_group_client(this._uuid_of_player, "cMsgConnect", "retResetScene", userid);
                    }
                    break;
                default:
                    break;
            }
        }


        ////////////////////////////////////////     具体和学生互动有关的 rpc函数  学生点赞、举手  老师  接收答题反馈 等  /////////////////////////////////////
        // 学生答题反馈
        public void AnswerQuestion(Int64 userid, Int64 questionid, Int64 optionid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retAnswerQuestion", userid, questionid, optionid);
        }
        // 学生抢答反馈
        private int isfastquestionbegin = 0;
        public void AnswerFastQuestion(Int64 userid, Int64 questionid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if(userid == this.leader.selfid)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if(isfastquestionbegin == 2)
            {
                Console.WriteLine("已经有人抢到题目了");
                return;
            }
            
            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retAnswerFastQuestion", userid, questionid);
            hub.hub.gates.call_client(sceneplaylistbyid[id].uuid, "cMsgConnect", "retAnswerFastQuestion", userid, questionid);

            if (isfastquestionbegin == 1)
            {
                // 开始抢答
                isfastquestionbegin = 2;
            }
        }
        // 点赞
        public void SendLikeToTeacher(Int64 userid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retSendLike", userid);
        }

        // 举手
        public void SendDoubtToTeacher(Int64 userid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retSendDoubt", userid);
        }

        // 推送电子白板
        public void SwitchWhiteBoard(Int64 userid, Int64 openclose)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retWhiteBoard", userid, openclose);
        }

        // 返回大厅
        public void BackToLobby(Int64 userid)
        {
            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retBackToLobby", userid);
        }

        // 获取在线学生列表
        public void GetOnlinePlayers(Int64 userid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            int id = (int)userid;

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[id]))
            {
                // 权限不够
                return;
            }

            ArrayList players = new ArrayList();
            foreach(PlayerInScene p in this.sceneplaylistbyid.Values)
            {
                if(p.selfid == this.leader.selfid)
                {
                    continue;
                }

                players.Add((Int64)p.selfid);
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retOnlinePlayers", players);
        }

    }
}
