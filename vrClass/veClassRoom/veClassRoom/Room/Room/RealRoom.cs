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
        public Hashtable userlist = new Hashtable();                     // 从服务器获取的当前教室的学生列表 用于登录比对

        public Dictionary<string, GroupInRoom> grouplist = new Dictionary<string, GroupInRoom>();

        // 场景的 初始化的数据   用于重置场景
        private SceneData _originalscene = new SceneData();

        ////////////////////////////////////////////////////      场景初始化、同步、销毁等操作模块      ///////////////////////////////////////////////

        public override void CreateScene(string name, imodule model, bool isaddmodel = false)
        {
            base.CreateScene(name, model, isaddmodel);
            this.model = Enums.ModelEnums.SynchronousOne;
        }

        public override void InitScenes(Hashtable data)
        {
            base.InitScenes(data);

            // 初始化原始服务器数据
            this._originalscene.InitSceneData(this.moveablesceneobject, this.sceneplaylist, this.sceneorderlist);

            Console.WriteLine("初始化服务器");
        }

        public void BeginClass(string token)
        {
            if(istartclass)
            {
                return;
            }
            if (leader == null)
            {
                return;
            }

            try
            {
                if(!sceneplaylist.ContainsKey(token))
                {
                    return;
                }

                PlayerInScene p = sceneplaylist[token];
                if(p.uuid != leader.uuid)
                {
                    return;
                }

            }catch
            {

            }
            if(!isinitclass)
            {
                return;
            }
            // 进行一次场景同步
            SceneSynchronizationAll();
            Thread t = RoomManager.getInstance().FindThreadOfRoom(this.scenename);
            if(t == null)
            {
                t = RoomManager.getInstance().ApplyThreadForRoom(this.scenename);
            }

            if(t!=null)
            {
                if(t.ThreadState == ThreadState.Unstarted)
                t.Start();
            }

            istartclass = true;
        }

        public override void SceneSynchronizationAll()
        {
            base.SceneSynchronizationAll();
        }

        ////////////////////////////////////////////////////      场景初始化、同步、销毁等操作模块   End   ///////////////////////////////////////////////

        ////////////////////////////////////////////////////      向后台服务器获取玩家信息、验证登陆、反馈数据等操作模块      ///////////////////////////////////////////////

        // 获取房间所有报名者的信息
        public void GetAllPlayerInfor()
        {
            userlist = BackDataService.getInstance().GetUserList(this.scenename, null);
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

            createPlayer(playerinfor, token, name, uuid);
        }

        private void CheckPlayerInforLocal(string token, string name, string uuid)
        {
            if (userlist.Count <= 0 || !userlist.ContainsKey(token))
            {
                return;
            }

            UserInfor playerinfor = userlist[token] as UserInfor;

            createPlayer(playerinfor, token, name, uuid);
        }

        ////////////////////////////////////////////////////      向后台服务器获取玩家信息、验证登陆、反馈数据等操作模块  End    ///////////////////////////////////////////////


        ////////////////////////////////////////////////////      玩家登陆进入房间、离开、玩家创建等操作模块      ///////////////////////////////////////////////

        public string PlayerEnterScene(string token, string name, string uuid)
        {
            Console.WriteLine("用户真正进入房间 : " + "name:" + name + " token:" + token + " uuid:" + uuid + " roomname:" + scenename);
            //       CheckPlayerInfor(token, name, uuid);
            //CheckPlayerInforLocal(token, name, uuid);

            // 跳过服务器验证 只为测试
            UserInfor p = new UserInfor();
            if (token.Contains("teacher"))
            {
                p.isleader = true;
            }
            createPlayer(p, token, name, uuid);

            if(istartclass && !p.isleader)
            {
                SceneSynchronizationPlayer(uuid);
            }

            return this.scenename;
        }

        public void PlayerLeaveScene(string token, string name, string uuid)
        {
            if (sceneplaylist.ContainsKey(token))
            {
                sceneplaylist.Remove(token);
            }

            if (_uuid_of_player.Contains(uuid))
            {
                _uuid_of_player.Remove(uuid);
            }

            if(sceneplaylist.Count <= 0)
            {
                isactive = false;

                ClearScene();

                RoomManager.getInstance().DeleteRoomByName(this.scenename);

                Console.WriteLine("清除房间 : " + this.scenename);

                return;
            }

            // 玩家离开释放 玩家锁住的物体
            foreach (ObjectInScene so in moveablesceneobject.Values)
            {
                if (!so.locked)
                {
                    continue;
                }

                if (so.locker == token)
                {
                    so.locker = null;
                    so.locked = false;
                    so.lockpermission = 0;
                }
            }
        }

        private void createPlayer(UserInfor playerinfor, string token, string name, string uuid)
        {
            if(playerinfor == null)
            {
                return;
            }

            if (playerinfor.islogin)
            {
                // 重复登陆
                // TODO

                return;
            }

            PlayerInScene player = new PlayerInScene(playerinfor,token,name,uuid);

            // 切换玩家自身模式
            if(player!=null)
            {
                player.ChangePlayerModel(this.model);
            }

            if (player.isleader)
            {
                // 登陆者是老师
                this.leader = player;

            }else
            {
                // 登陆者是学生
            }

            // 只为测试
     //       this.leader = player;

            // 通知客户端显示登陆信息
            string msg = name + "登陆进入房间";
            TellClientMsg(this._uuid_of_player, msg);

            Console.WriteLine("通知其他玩家用户进入房间 : " + "msg : " + msg + " 当前玩家数: " + this._uuid_of_player.Count);

            if (this.sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("用户重复登陆进入房间 : " + "name:" + name + " token:" + token + " uuid:" + uuid + " roomname:" + scenename);

                // 重复登陆  可能是掉线重登
                //TODO
                PlayerInScene p = this.sceneplaylist[token];
                if (_uuid_of_player.Contains(p.uuid))
                {
                    _uuid_of_player.Remove(p.uuid);
                }

                _uuid_of_player.Add(uuid);

                this.sceneplaylist[token] = player;
            }
            else
            {
                this.sceneplaylist.Add(token, player);

                if (!_uuid_of_player.Contains(uuid))
                {
                    _uuid_of_player.Add(uuid);
                }
            }
        }

        ////////////////////////////////////////////////////      玩家登陆进入房间、离开、玩家创建等操作模块   End   ///////////////////////////////////////////////


        ////////////////////////////////////////////////////      具体和客户端的操作      ///////////////////////////////////////////////

        public override void Switch_Model(string token, string tomodel, string uuid)
        {
            if(this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if(sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if(!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 : " + "token : " + token + " tomodel: " + tomodel);
                return;
            }

            Console.WriteLine("切换模式成功 : " + "token : " + token + " tomodel: " + tomodel + "  开始更改玩家状态");

            Enums.ModelEnums m = this.model;
            switch (tomodel)
            {
                case "Separate":
                    m = Enums.ModelEnums.Separate;
                    break;
                case "SynchronousOne":
                    m = Enums.ModelEnums.SynchronousOne;
                    break;
                case "SynchronousMultiple":
                    m = Enums.ModelEnums.SynchronousMultiple;
                    break;
                case "Collaboration":
                    m = Enums.ModelEnums.Collaboration;
                    break;
                default:
                    break;
            }

            if (m != this.model)
            {
                this.model = m;

                try
                {
                    foreach (PlayerInScene player in sceneplaylist.Values)
                    {
                        player.ChangePlayerModel(this.model);
                    }
                }
                catch
                {

                }

            }

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_change_one_model", token, tomodel);
            }

            Console.WriteLine("更改了当前房间所有玩家模式成功 " + " tomodel: " + tomodel);
        }

        public override void Req_Object_Operate_permissions(string token, string objectname, string uuid)
        {
            if(this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            PlayerInScene ps = findPlayerByToken(token);
            if(ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if(ps.group != null)
            {
                ps.group.Req_Object_Operate_permissions(token, objectname, uuid);

                return;
            }

            // 没有小组执行以下操作

            if (!(checkOperateFeasible(ps) || ps.isCanOperate))
            {
                Console.WriteLine("无权操作 : " + "token : " + token);
                return;
            }

            //具体的操作控制
            bool ret = false;

            do
            {
                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + token + "请求操作物体" + objectname + "失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = moveablesceneobject[objectname];
                if (o.locked)
                {
                    if (o.locker == token)
                    {
                        Console.WriteLine("token : " + token + "重复请求操作物体" + objectname);
                        break;
                    }

                    if(o.lockpermission < ps.permission)
                    {
                        o.locker = token;
                        o.locked = true;
                        o.lockpermission = ps.permission;

                        ret = true;

                        Console.WriteLine("token : " + token + "权限较高夺取了 token  " + o.locker + " 的物体 " + objectname);
                        break;
                    }

                    break;
                }
                else
                {
                    o.locker = token;
                    o.locked = true;
                    o.lockpermission = ps.permission;

                    ret = true;
                }

            } while (false);

            if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "hold", ret ? "yes" : "no");
            }
        }

        public override void Req_Object_Release_permissions(string token, string objectname, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            PlayerInScene ps = findPlayerByToken(token);
            if (ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if (ps.group != null)
            {
                ps.group.Req_Object_Release_permissions(token, objectname, uuid);

                return;
            }

            // 没有小组执行以下操作

            if (!(checkOperateFeasible(ps) || ps.isCanOperate))
            {
                Console.WriteLine("无权操作 : " + "token : " + token);
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = moveablesceneobject[objectname];
                if (!o.locked)
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体锁已被释放");
                    break;
                }
                else if (o.locker != token)
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体被 token : " + o.locker + "锁住");
                    break;
                }
                else
                {
                    o.locker = null;
                    o.locked = false;
                    o.lockpermission = Enums.PermissionEnum.None;

                    ret = true;
                }

            } while (false);

            if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "release", ret ? "yes" : "no");
            }
        }

        public override void ChangeObjectAllOnce(string token, Hashtable clientallonce)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            PlayerInScene ps = findPlayerByToken(token);
            if (ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if (ps.group != null)
            {
                ps.group.ChangeObjectAllOnce(token, clientallonce);

                return;
            }

            // 没有小组执行以下操作

            if (!(checkOperateEffective(ps) || ps.isCanSend))
            {
                Console.WriteLine("无权操作 : " + "token : " + token);
                return;
            }

            do
            {
                foreach (DictionaryEntry de in clientallonce)
                {
                    if (!moveablesceneobject.ContainsKey((string)de.Key))
                    {
                        // 服务器不包含该物体
                        continue;
                    }

                    var o = moveablesceneobject[(string)de.Key];

                    if (!o.locked || o.locker == null)
                    {
                        continue;
                    }

                    if (!(o.locker == token || o.lockpermission < ps.permission))
                    {
                        continue;
                    }

                    o.Conversion((Hashtable)de.Value);
                }

            } while (false);
        }

        public override void ChangePlayerAllOnce(string token, Hashtable clientallonce)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            PlayerInScene ps = findPlayerByToken(token);
            if (ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if (ps.group != null)
            {
                ps.group.ChangeObjectAllOnce(token, clientallonce);

                return;
            }

            // 没有小组执行以下操作

            if (!(checkOperateEffective(ps) || ps.isCanSend))
            {
                Console.WriteLine("无权操作 : " + "token : " + token);
                return;
            }

            do
            {
                foreach (DictionaryEntry de in clientallonce)
                {
                    if (!moveablesceneobject.ContainsKey((string)de.Key))
                    {
                        // 服务器不包含该物体
                        continue;
                    }

                    var o = moveablesceneobject[(string)de.Key];

                    if (!o.locked || o.locker == null)
                    {
                        continue;
                    }

                    if (!(o.locker == token || o.lockpermission < ps.permission))
                    {
                        continue;
                    }

                    o.Conversion((Hashtable)de.Value);
                }

            } while (false);
        }

        public override void ret_sync_commond(string typ, string commond, string token, string other, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0|| !sceneplaylist.ContainsKey(token))
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
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_sync_commond", typ, commond, token, other);
                _uuid_of_player.Add(uuid);
            }
            catch
            {

            }

            // 指令缓存
            sceneorderlist.Add(new OrderInScene(0,token,typ,commond,other));  //后期加入时间机制
        }

        public void ret_sync_group_commond(string typ, string commond, string token, string other, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                return;
            }

            PlayerInScene ps = findPlayerByToken(token);
            if (ps == null)
            {
                return;
            }

            // 先判断是否有小组
            if (ps.group != null)
            {
                ps.group.ret_sync_commond(typ, commond, token, other, uuid);

                return;
            }

            // 没有小组执行以下操作

            ret_sync_commond(typ, commond, token, other, uuid);
        }

        public override void SyncClient()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            while (_syncstate)
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

                foreach (PlayerInScene sp in sceneplaylist.Values)
                {
                    if (!sp.changeorno)
                    {
                        continue;
                    }

                    // 同步客户端
                    msgPlayer.Add(sp.name, sp.Serialize());
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

                        foreach (PlayerInScene p in sceneplaylist.Values)
                        {
                            if (p.isCanReceive)
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
                        _uuid_sync_cache.Clear();

                        //Console.WriteLine("同步客户端了哟");
                    }
                }

                // 同步之后清除
                msgObject.Clear();
                msgPlayer.Clear();

                Thread.Sleep(16);
            }
        }


        // 动态修改 玩家 权限 等操作
        // TODO
        // 更改某一玩家的模式状态
        public void Change_One_Model(string token, string tomodel, string uuid, string onetoken)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if(!sceneplaylist.ContainsKey(onetoken))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + onetoken + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 : " + "token : " + token + " tomodel: " + tomodel);
                return;
            }

            PlayerInScene ps = sceneplaylist[onetoken];
            this.model = Utilities.getInstance().convertModelToEnum(tomodel);
            ps.ChangePlayerModel(this.model);

            hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "ret_change_one_model", token, tomodel);

            Console.WriteLine("token : " + token + "切换 " + "totoken " + onetoken + " 模式成功 : " + " tomodel: " + tomodel + "  开始更改玩家状态");

        }

        // 更改某些玩家的模式状态
        public void Change_Some_Model(string token, string tomodel, string uuid, ArrayList sometoken)
        {
            if (sometoken == null || sometoken.Count <= 0)
            {
                return;
            }

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 : " + "token : " + token + " tomodel: " + tomodel);
                return;
            }

            this.model = Utilities.getInstance().convertModelToEnum(tomodel);

            ArrayList uuidcache = new ArrayList();
            foreach(string t in sometoken)
            {
                if(!sceneplaylist.ContainsKey(t))
                {
                    continue;
                }

                PlayerInScene ps = sceneplaylist[t];
                ps.ChangePlayerModel(this.model);

                uuidcache.Add(ps.uuid);
            }

            if(uuidcache.Count > 0)
            {
                hub.hub.gates.call_group_client(uuidcache, "cMsgConnect", "ret_change_one_model", token, tomodel);
            }

            uuidcache.Clear();

            Console.WriteLine("token : " + token + "切换 一些玩家模式成功 : " + " tomodel: " + tomodel);
        }


        ////////////////////////////////////////////////////   小组 管理相关     ///////////////////////////////////////////////////////////////////////////////////
        // 分组函数
        private void divideGroupByGrade()
        {
            int count = sceneplaylist.Count;
            int groupcount = count / 10;
            int remainder = count % 10;
            if(remainder > 0)
            {
                groupcount++;
            }
            int membercount = count / groupcount;
            remainder = count % groupcount; // 多余的人数 插入到 前几组之中

            string groupname = "group";
            for(int i=0;i<groupcount;i++)
            {
                GroupInRoom gir = new GroupInRoom();
                if(remainder > 0)
                {
                    gir.playercount = membercount + 1;
                    remainder--;
                }
                else
                {
                    gir.playercount = membercount;
                }
                grouplist.Add(groupname + i, gir);
            }

            remainder = 0;
            membercount = 0;
            GroupInRoom girr = grouplist[groupname + remainder];
            foreach (PlayerInScene ps in sceneplaylist.Values)
            {
                if(girr == null)
                {
                    break;
                }

                girr.AddMember(ps);
                ps.group = girr;
                membercount++;

                if (membercount >= girr.playercount)
                {
                    remainder++;
                    membercount = 0;
                    girr = grouplist[groupname + remainder];
                }
            }
        }

        public void DivideGroup(string token, string rules, string uuid)
        {
            if (token == null)
            {
                return;
            }

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if(token != this.leader.token)
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 不是老师不可分组 : " + "token : " + token);
                return;
            }

            // 按成绩分组
            divideGroupByGrade();
        }

        /// 选择某一学生 或者某一小组 操作 全班同学观看
        /// 仅限于观学模式
        public void ChooseOneOrGroupOperate(string token, string name, bool isgroup = false)
        {
            if(token == null)
            {
                return;
            }

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            // 当前模式必须是   指导模式才行
            if(this.model != Enums.ModelEnums.SynchronousMultiple)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("选择人物权限不够 : " + "token : " + token);
                return;
            }

            if(isgroup)
            {
                if(grouplist.Count <= 0 || !grouplist.ContainsKey(name))
                {
                    Console.WriteLine("当前房间不存在该小组 : " + name);
                    return;
                }

                // 组内协作 并且广播 给 所有玩家 
                // TODO
                // 1,改变 组内成员权限 可操作 可同步
                GroupInRoom gir = grouplist[name];
                if(gir == null)
                {
                    return;
                }

                gir.SwitchModelTo(Enums.ModelEnums.Collaboration);
                // 2,向所选择小组注入观看成员
                gir.InjectiveViewer(_uuid_of_player);

                // 通知该小组成员 被选中
                gir.TellPlayerBeSelected(token, name);
            }
            else
            {
                // 改变一个人的权限 并且广播
                if(!sceneplaylist.ContainsKey(name))
                {
                    Console.WriteLine("服务器玩家不存在 : " + "token : " + name);
                    return;
                }

                // 学生可操作 并且广播给所有学生 但是 老师可介入
                //TODO
                // 1,特殊改变某一学生的操作开关
                PlayerInScene ps = sceneplaylist[name];
                if(ps == null)
                {
                    return;
                }

                ps.ChangePlayerCanOperate(this.leader.permission, true);
                ps.ChangePlayerCanSend(this.leader.permission, true);
                ps.ChangePlayerCanReceive(this.permission, true);

                // 通知学生他被选中
                hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "ret_choose_one_operate", token, name);
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
        public void SwitchTeachMode(string token, Int64 mode, Int64 isgroup, string target = null)
        {
            Console.WriteLine("客户端切换模式 UI " + "token" + token + "mode" + mode + "target" + target);

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("UI 教学模式切换操作 权限不够 : " + "token : " + token);
                return;
            }

            Enums.TeachingMode tm = (Enums.TeachingMode)mode;

            switch(tm)
            {
                case Enums.TeachingMode.WatchLearnModel_Sync:
                   // break;
                case Enums.TeachingMode.WatchLearnModel_Async:
                    this.model = Enums.ModelEnums.SynchronousOne;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylist.Values)
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
                    if(!sceneplaylist.ContainsKey(target))
                    {
                        Console.WriteLine("GuidanceMode_Personal UI 服务器玩家不存在 : " + "target : " + target);
                        return;
                    }
                    this.model = Enums.ModelEnums.SynchronousMultiple;
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylist.Values)
                        {
                            player.ChangePlayerModel(this.model);
                            if (player.token == target)
                            {
                                player.ChangePlayerCanSend(this.leader.permission, true);
                                player.ChangePlayerCanReceive(this.leader.permission, true);
                                player.ChangePlayerCanOperate(this.leader.permission, true);
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
                    if (grouplist.Count <= 0 || !grouplist.ContainsKey(target))
                    {
                        Console.WriteLine("GuidanceMode_Personal UI 服务器玩家不存在 : " + "target : " + target);
                        return;
                    }
                    this.model = Enums.ModelEnums.SynchronousMultiple;
                    GroupInRoom gir = grouplist[target];
                    try
                    {
                        foreach (PlayerInScene player in sceneplaylist.Values)
                        {
                            if(!gir.HasMember(player.token))
                            {
                                player.ChangePlayerModel(this.model);
                                gir.InjectiveViewer(player.token);
                            }
                            else
                            {
                                player.ChangePlayerModel(Enums.ModelEnums.Collaboration);
                                player.ChangePlayerCanSend(this.leader.permission, true);
                                player.ChangePlayerCanReceive(this.leader.permission, true);
                                player.ChangePlayerCanOperate(this.leader.permission, true);
                            }
                        }
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_Personal:
                    if (target == null)
                    {
                        break;
                    }
                    if (!sceneplaylist.ContainsKey(target))
                    {
                        Console.WriteLine("SelfTrain_Personal UI 服务器玩家不存在 : " + "target : " + target);
                        return;
                    }
                    this.model = Enums.ModelEnums.Collaboration;
                    PlayerInScene pl = sceneplaylist[target];
                    try
                    {
                        pl.ChangePlayerModel(this.model);
                        pl.ChangePlayerCanSend(this.leader.permission, true);
                        pl.ChangePlayerCanReceive(this.leader.permission, true);
                        pl.ChangePlayerCanOperate(this.leader.permission, true);
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_Group:
                    if (target == null)
                    {
                        break;
                    }
                    if (grouplist.Count <= 0 || !grouplist.ContainsKey(target))
                    {
                        Console.WriteLine("GuidanceMode_Personal UI 服务器玩家不存在 : " + "target : " + target);
                        return;
                    }
                    this.model = Enums.ModelEnums.Collaboration;
                    GroupInRoom gir2 = grouplist[target];
                    try
                    {
                        gir2.Switch_Model(token, Utilities.getInstance().convertEnumToModel(this.model), this.leader.uuid);
                    }
                    catch
                    {

                    }
                    break;
                case Enums.TeachingMode.SelfTrain_All:
                    this.Switch_Model(token, Utilities.getInstance().convertEnumToModel(Enums.ModelEnums.Collaboration), this.leader.uuid);
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

            // 回复客户端
            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retSwitchTeachMode", token, mode, target);
            }
        }

        /// <summary>
        /// 老师随堂测验的操作， 记得区分 在vr课件里 还是 在 学习大厅
        /// </summary>
        /// <param name="token">操作者token</param>
        /// <param name="typ">随堂测验的类型</param>
        /// <param name="question">具体问题信息id</param>
        /// <param name="other">额外的参数信息</param>
        public void InClassTest(string token, Int64 typ, Int64 questionid, string other = null)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("UI 教学模式切换操作 权限不够 : " + "token : " + token);
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

            // 广播 所有学生 测验题
            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "retInClassTest", token, typ, questionid, other);
            }
        }

        /// <summary>
        /// 重置场景相关
        /// </summary>
        /// <param name="token">操作者token</param>
        /// <param name="typ">重置对象类型：全部、组、个人</param>
        /// <param name="target">具体重置对象信息：名字</param>
        public void ResetScene(string token, Int64 typ, string target)
        {
            if(token == null)
            {
                return;
            }

            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("UI 教学模式切换操作 权限不够 : " + "token : " + token);
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

                    if (!sceneplaylist.ContainsKey(target))
                    {
                        Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + target);
                        return;
                    }
                    PlayerInScene ps = sceneplaylist[target];
                    hub.hub.gates.call_client(ps.uuid, "cMsgConnect", "retResetScene", token);
                    break;
                case Enums.OperatingRange.Team:
                    if (target == null)
                    {
                        return;
                    }

                    if (!grouplist.ContainsKey(target))
                    {
                        Console.WriteLine("UI 服务器小组不存在 : " + "token : " + target);
                        return;
                    }
                    GroupInRoom gir = grouplist[target];
                    hub.hub.gates.call_group_client(gir._uuid_of_player, "cMsgConnect", "retResetScene", token);
                    break;
                case Enums.OperatingRange.All:
                    if(_uuid_of_player.Count > 0)
                    {
                        hub.hub.gates.call_group_client(this._uuid_of_player, "cMsgConnect", "retResetScene", token);
                    }
                    break;
                default:
                    break;
            }
        }


        ////////////////////////////////////////     具体和学生互动有关的 rpc函数  学生点赞、举手  老师  接收答题反馈 等  /////////////////////////////////////
        // 学生答题反馈
        public void AnswerQuestion(string token, Int64 questionid, Int64 optionid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retAnswerQuestion", token, questionid, optionid);
        }
        // 点赞
        public void SendLikeToTeacher(string token)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retSendLike", token);
        }

        // 举手
        public void SendDoubtToTeacher(string token)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("UI 服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return;
            }

            hub.hub.gates.call_client(this.leader.uuid, "cMsgConnect", "retSendDoubt", token);
        }

    }
}
