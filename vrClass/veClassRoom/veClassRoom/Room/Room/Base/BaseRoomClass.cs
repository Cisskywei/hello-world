using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 场景同步的基类
    /// </summary>
    class BaseRoomClass : imodule, BaseRoomInterface
    {
        // 场景名称 亦作为module名字
        public string scenename;

        // 场景名称 亦作为场景唯一索引
        public int sceneid;

        // 场景是否开始  即课程是否开始
        public bool istartclass = false;

        // 场景是否初始化
        public bool isinitclass = false;

        // 场景是否是继续使用的
        public bool isactive = true;

        // 场景中人数
        public int playercount = 0;  // 小组分组算法会用到

        // 场景中需要同步的物体
        public Dictionary<string, ObjectInScene> moveablesceneobject = new Dictionary<string, ObjectInScene>();

        // 场景中的玩家列表
        public Dictionary<int, PlayerInScene> sceneplaylistbyid = new Dictionary<int, PlayerInScene>();

        // 场景中的玩家uuid列表
        public ArrayList _uuid_of_player = new ArrayList();

        //场景的指令
        public List<OrderInScene> sceneorderlist = new List<OrderInScene>();

        // 场景的当前模式  默认各自独立
        public Enums.TeachingMode model = Enums.TeachingMode.WatchLearnModel_Sync;
 //       public BaseModelClass realmodel; // 负责分发处理各个模式

        // 当前场景的权限
        public Enums.PermissionEnum permission = Enums.PermissionEnum.Group;

        // 领导者  当前场景权限最高者
        public PlayerInScene leader;

        // 收发数据uuidlist 缓存列表
        public ArrayList _uuid_sync_cache = new ArrayList();

        ~BaseRoomClass()
        {
            ClearScene();
        }

        public void ClearScene()
        {
            try
            {
                isinitclass = false;
                istartclass = false;
                isactive = false;
                model = Enums.TeachingMode.WatchLearnModel_Sync;
                permission = Enums.PermissionEnum.Group;

                StopSyncClient();
                moveablesceneobject.Clear();
                sceneplaylistbyid.Clear();
                sceneorderlist.Clear();
                _uuid_sync_cache.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="name"></param>
        public virtual void CreateScene(string name)
        {
            this.scenename = name;
            this.isinitclass = false;
        }

        /// <summary>
        /// 初始化服务器场景
        /// </summary>
        /// <param name="data"></param>
        public virtual void InitScenes(Hashtable data)
        {
            Console.WriteLine("初始化服务器场景数据" + data.Count);

            if (data.Count <= 0)
            {
                Console.WriteLine("服务器场景数据参数为空" + data.Count);
                return;
            }

            foreach (DictionaryEntry de in data)
            {
                if (moveablesceneobject.ContainsKey((string)de.Key))
                {
                    moveablesceneobject[(string)de.Key].Deserialization((Hashtable)de.Value);
                }
                else
                {
                    ObjectInScene s = new ObjectInScene();
                    s.name = (string)de.Key;
                    s.Deserialization((Hashtable)de.Value);
                    moveablesceneobject.Add(s.name, s);
                }
            }

            Console.WriteLine("初始化服务器场景数据完毕");
        }

        /// <summary>
        /// 同步所有玩家场景
        /// </summary>
        public virtual void SceneSynchronizationAll()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            Hashtable msgOrder = new Hashtable();

            // 同步数据
            foreach (ObjectInScene so in moveablesceneobject.Values)
            {
                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (PlayerInScene sp in sceneplaylistbyid.Values)
            {
                // 同步客户端
                msgPlayer.Add(sp.name, sp.Serialize());
            }

            foreach (OrderInScene so in sceneorderlist)
            {
                // 同步客户端
                msgPlayer.Add(so.name, so.Serialize());
            }

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0 || msgOrder.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "SyncClientAndCommond", msgObject, msgPlayer, msgOrder);
            }
        }

        /// <summary>
        /// 同步指定玩家场景
        /// </summary>
        /// <param name="uuid"></param>
        public virtual void SceneSynchronizationPlayer(string uuid)
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            Hashtable msgOrder = new Hashtable();

            // 同步数据
            foreach (ObjectInScene so in moveablesceneobject.Values)
            {
                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (PlayerInScene sp in sceneplaylistbyid.Values)
            {
                // 同步客户端
                msgPlayer.Add(sp.name, sp.Serialize());
            }

            foreach (OrderInScene so in sceneorderlist)
            {
                // 同步客户端
                msgPlayer.Add(so.name, so.Serialize());
            }

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0 || msgOrder.Count > 0)
            {
                hub.hub.gates.call_client(uuid, "cMsgConnect", "SyncClientAndCommond", msgObject, msgPlayer, msgOrder);
            }
        }

        /// <summary>
        /// 验证token与所在房间的权限 是否满足操作房间的权限
        /// </summary>
        /// <param name="token"></param>
        public virtual Enums.PermissionVerifyStatus VerifyPermission(int userid)
        {
            Enums.PermissionVerifyStatus ret = Enums.PermissionVerifyStatus.None;

            do
            {
                if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
                {
                    ret = Enums.PermissionVerifyStatus.None;
                    break;
                }

                PlayerInScene pis = sceneplaylistbyid[userid];

                if (pis.permission < this.permission)
                {
                    ret = Enums.PermissionVerifyStatus.Lower;
                    break;
                }

                if (pis.permission == this.permission)
                {
                    ret = Enums.PermissionVerifyStatus.Equal;
                    break;
                }

                if (pis.permission > this.permission)
                {
                    ret = Enums.PermissionVerifyStatus.Higher;
                    break;
                }

            } while (false);

            return ret;
        }

        /// <summary>
        /// 提供房间的介入操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="param"></param>
        /// <param name="model"></param>
        public virtual Enums.PermissionVerifyStatus InterveneOperate<T>(int userid, T param, Enums.ModelEnums model = Enums.ModelEnums.None)
        {
            // 权限验证
            return VerifyPermission(userid);
        }

        /// <summary>
        /// 切换模式
        /// </summary>
        /// <param name="tomodel"></param>
        public virtual void SwitchModelTo(Enums.TeachingMode tomodel)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            bool isusecache = false;

            if(_uuid_of_player.Count <= 0)
            {
                isusecache = true;
            }

            if (tomodel != this.model)
            {
                this.model = tomodel;

                try
                {
                    if(isusecache)
                    {
                        if (_uuid_sync_cache.Count > 0)
                        {
                            _uuid_sync_cache.Clear();
                        }
                    }

                    foreach (PlayerInScene player in sceneplaylistbyid.Values)
                    {
                        player.ChangePlayerModel(this.model);

                        if(isusecache)
                        {
                            _uuid_sync_cache.Add(player.uuid);
                        }
                    }
                }
                catch
                {

                }

            }

            if (isusecache && _uuid_sync_cache.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_sync_cache, "cMsgConnect", "ret_change_one_model", this.leader.token, tomodel);
            }
            else if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_change_one_model", this.leader.token, tomodel);
            }
        }

        public void StopSyncClient()
        {
            _syncstate = false;
        }

        public virtual void StartSyncClient()
        {
            _syncstate = true;
            // 时间服务同步
            hub.hub.timer.addticktime(400, SyncClient);
        }

        // *************************************************    以下是基础场景所拥有的基本函数 和客户端通信所需    ****************************************//

        // 权限验证   只对模式和权限进行判别
        // 基本权限判别： 该房间仅有当前leader 和 更高级别player可操作   用于模式切换等操作 不用于 玩家同步场景等的判别
        public virtual bool checkLeaderFeasible(PlayerInScene player)
        {
            bool ret = false;

            do
            {
                if (player == null)
                {
                    Console.WriteLine("player == null");
                    break;
                }

                if (!player.isleader)
                {
                    Console.WriteLine("!player.isleader" + player.isleader);

                    break;
                }

                if (player.permission < this.permission)
                {
                    Console.WriteLine(this.permission + "player.permission < this.permission" + player.permission);

                    // 权限不够
                    break;
                }
                else if (player.permission == this.permission && player.token != this.leader.token)
                {
                    Console.WriteLine("player.permission == this.permission && player.token != this.leader.token" + player.token);
                    // 同一小组级别不可互相操作
                    break;
                }

                ret = true;

            } while (false);

            return ret;
        }

        // 是否能够操作物体 1,独立模式是可以自由操作的 但是不更改服务器状态 2,根据模式进行判别
        // 1，独立模式可操作
        // 2，观学模式 异步 可操作
        // 3，观学模式被选中学生可操作
        // 4，多人协同模式可操作
        // 5，老师一直可操作
        public virtual bool checkOperateFeasible(PlayerInScene player)
        {
            bool ret = false;

            do
            {
                if (player == null)
                {
                    break;
                }

                // 第一步 根据当前房间模式进行操作是否可行判别
                switch (this.model)
                {
                    case Enums.TeachingMode.WatchLearnModel_Sync:
                       // ret = true;
                        //break;
                    case Enums.TeachingMode.WatchLearnModel_Async:   // 同步leader(可能是老师)  只有当前 leader 可操作
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.TeachingMode.GuidanceMode_Personal:
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else if (player.isbechoosed)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.TeachingMode.GuidanceMode_Group:
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else if (player.isbechoosed)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.TeachingMode.SelfTrain_Personal:
                    case Enums.TeachingMode.SelfTrain_Group:
                    case Enums.TeachingMode.SelfTrain_All:
                        ret = true;
                        break;
                    default:
                        break;
                }

            } while (false);

            // 第二步 根据个人模式进行操作可行性判别 受房间模式影响
            if (ret)
            {
                switch (player.model)
                {
                    default:
                        break;
                }
            }

            return ret;
        }

        // 判断操作对服务器的数据是否造成有效修改
        public virtual bool checkOperateEffective(PlayerInScene player)
        {
            bool ret = false;

            do
            {
                if (player == null)
                {
                    break;
                }

                if(this.leader == null)
                {
                    break;
                }

                // 第一步 根据当前房间模式进行操作是否有效判别
                switch (this.model)
                {
                    case Enums.TeachingMode.WatchLearnModel_Sync:
                    case Enums.TeachingMode.WatchLearnModel_Async:
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else if (player.permission > this.leader.permission)
                        {
                            ret = true;                        // 玩家权限 可能被提升 大于 房间leader 即可操作
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.TeachingMode.GuidanceMode_Personal:
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else if (player.permission > this.leader.permission)
                        {
                            ret = true;                        // 玩家权限 可能被提升 大于 房间leader 即可操作
                        }
                        else if (player.isbechoosed)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }

                        ret = ret || player.isCanSend;   // 玩家权限 可能是被选中的
                        break;
                    case Enums.TeachingMode.GuidanceMode_Group:
                        if (player.isleader && player.selfid == this.leader.selfid)
                        {
                            ret = true;
                        }
                        else if (player.permission > this.leader.permission)
                        {
                            ret = true;                        // 玩家权限 可能被提升 大于 房间leader 即可操作
                        }
                        else if (player.isbechoosed)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        ret = ret || player.isCanSend;   // 玩家权限 可能是被选中的
                        break;
                    case Enums.TeachingMode.SelfTrain_Personal:
                    case Enums.TeachingMode.SelfTrain_Group:
                    case Enums.TeachingMode.SelfTrain_All:
                        ret = true;
                        break;
                    default:
                        break;
                }

            } while (false);

            // 第二步 根据个人模式进行操作权限判别 受房间模式影响
            if (ret)
            {
                switch (player.model)
                {
                    default:
                        ret = true;                         // 其他情况不允许玩家操作修改当前服务器数据
                        break;
                }
            }

            return ret;
        }

        // 判断玩家是否可接收服务器指令、同步数据等，限定比较宽  可根据服务器性能有选择适当缩紧限制范围   目前 可除了独立模式 皆可接收
        public virtual bool checkPlayerReceive(PlayerInScene player)
        {
            bool ret = false;

            do
            {
                if (player == null)
                {
                    break;
                }

                switch (this.model)
                {
                    case Enums.TeachingMode.SelfTrain_Personal:
                        ret = false;
                        break;
                    default:
                        ret = true;
                        break;
                }

            } while (false);

            return ret;
        }

        // 判断是否是组内操作
        public bool checkIsInGroup()
        {
            bool ret = false;
            switch(this.model)
            {
                case Enums.TeachingMode.SelfTrain_Group:
                    ret = true;
                    break;
                default:
                    ret = false;
                    break;
            }

            return ret;
        }

        ///                          以上函数是模式切换、以及玩家随意模式的核心        模式切换受权限级别控制                          /////////////////////////////

        // 根据userid获取玩家
        public virtual PlayerInScene findPlayerById(int id)
        {
            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(id))
            {
                return null;
            }

            return sceneplaylistbyid[id];
        }

        public virtual void Switch_Model(int userid, Int64 tomodel, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylistbyid.Count <= 0 || !sceneplaylistbyid.ContainsKey(userid))
            {
                Console.WriteLine("服务器玩家不存在 : " + " sceneplaylist count : " + sceneplaylistbyid.Count);
                return;
            }

            if (!checkLeaderFeasible(sceneplaylistbyid[userid]))
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

        public virtual void Req_Object_Operate_permissions(Int64 id, string objectname, string uuid)
        {
            int userid = (int)id;

            if (this.leader == null || this.leader.uuid == null)
            {
                Console.WriteLine("小组长暂时不存在");
   //             return;
            }

            PlayerInScene ps = findPlayerById(userid);
            if (ps == null)
            {
                return;
            }

            //具体的操作控制
            bool ret = false;

            do
            {
                if (!(checkOperateFeasible(ps) || ps.isCanOperate))
                {
                    Console.WriteLine("Req_Object_Operate_permissions 无权操作 : " + "token : " + userid);
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

                    if (o.lockpermission < ps.permission)
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

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objectname, "hold", ret ? "yes" : "no");
            }
        }

        public virtual void Req_Object_Release_permissions(Int64 id, string objectname, string uuid)
        {
            int userid = (int)id;

            if (this.leader == null || this.leader.uuid == null)
            {
    //            return;
            }

            PlayerInScene ps = findPlayerById(userid);
            if (ps == null)
            {
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                if (!(checkOperateFeasible(ps) || ps.isCanOperate))
                {
                    Console.WriteLine("Req_Object_Release_permissions 无权操作 : " + "token : " + userid);
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

                    o.physical.useGravity = true;

                    o.changeorno = true;

                    ret = true;
                }

            } while (false);

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objectname, "release", ret ? "yes" : "no");
            }
        }

        public virtual void ChangeObjectAllOnce(Int64 id, Hashtable clientallonce)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            int userid = (int)id;

            PlayerInScene ps = findPlayerById(userid);
            if (ps == null)
            {
                return;
            }

            if (!(checkOperateEffective(ps) || ps.isCanSend))
            {
                Console.WriteLine("无权操作 : " + "token : " + userid);
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

                    if (!(o.locker == ps.token || o.lockpermission < ps.permission))
                    {
                        continue;
                    }

                    o.Conversion((Hashtable)de.Value);
                }

            } while (false);
        }

        public virtual void ChangePlayerAllOnce(Int64 userid, Hashtable clientallonce)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            int id = (int)userid;

            if (!sceneplaylistbyid.ContainsKey(id))
            {
                return;
            }

            do
            {
                sceneplaylistbyid[id].Conversion(clientallonce);

            } while (false);
        }

        public virtual void ChangeClientAllOnce(Int64 userid, Hashtable clientallonce)
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

            if (!(checkOperateEffective(ps) || ps.isCanSend || ps.isbechoosed))
            {
                Console.WriteLine("无权操作 : " + "token : " + userid);
                return;
            }

            do
            {
                Hashtable player = (Hashtable)clientallonce["player"];
                Hashtable objects = (Hashtable)clientallonce["objects"];

                if(player != null && player.Count > 0)
                {
                    sceneplaylistbyid[id].InitPlayerHeadHand(player);
                }

                if (objects != null && objects.Count > 0)
                {
                    ChangeObjectAllOnce(id, objects);
                }

            } while (false);
        }

        public virtual void ret_sync_commond(string typ, string commond, Int64 userid, string other, string uuid)
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

            if (_uuid_of_player == null || _uuid_of_player.Count <= 0)
            {
                return;
            }

            try
            {
                _uuid_of_player.Remove(uuid);
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_sync_commond", typ, commond, id, other);
                _uuid_of_player.Add(uuid);
            }
            catch
            {

            }

            // 指令缓存
            sceneorderlist.Add(new OrderInScene(0, id, typ, commond, other));  //后期加入时间机制
        }

        // 单独线程定时同步客户端数据
        public bool _syncstate = true;
        public virtual void SyncClient(long tick)
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
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
                msgPlayer.Add(sp.selfid.ToString(), sp.Serialize());
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

        ////////////////////////////////////////////////      其他功能函数       ///////////////////////////////////////////////////////////////////////////////////
        public void TellClientMsg(ArrayList listenuuids, string msg, string selfuuid = null)
        {
            if(selfuuid != null)
            {
                var uuid = string.Empty;
                for(int i=0; i< listenuuids.Count; i++)
                {
                    uuid = (string)listenuuids[i];
                    if (uuid == selfuuid)
                    {
                        continue;
                    }
                    hub.hub.gates.call_client((string)listenuuids[i], "cMsgConnect", "ListenerServerMsg", msg);
                }
            }
            else
            {
                hub.hub.gates.call_group_client(listenuuids, "cMsgConnect", "ListenerServerMsg", msg);
            }
        }

        // 通知玩家自己被选中 作为 操作主题（观学模式）
        public void TellPlayerBeSelected(string token, string name, ArrayList uuids = null)
        {
            if(uuids == null || uuids.Count <= 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_choose_one_operate", token, name);
            }
            else
            {
                hub.hub.gates.call_group_client(uuids, "cMsgConnect", "ret_choose_one_operate", token, name);
            }
        }
    }
}
