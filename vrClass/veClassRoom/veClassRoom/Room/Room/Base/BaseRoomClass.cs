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
        public Int64 sceneid;

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
        public Dictionary<string, PlayerInScene> sceneplaylist = new Dictionary<string, PlayerInScene>();
        public Dictionary<Int64, PlayerInScene> sceneplaylistbyid = new Dictionary<Int64, PlayerInScene>();

        // 场景中的玩家uuid列表
        public ArrayList _uuid_of_player = new ArrayList();

        //场景的指令
        public List<OrderInScene> sceneorderlist = new List<OrderInScene>();

        // 场景的当前模式  默认各自独立
        public Enums.ModelEnums model = Enums.ModelEnums.Separate;
 //       public BaseModelClass realmodel; // 负责分发处理各个模式

        // 当前场景的权限
        public Enums.PermissionEnum permission = Enums.PermissionEnum.Group;

        // 领导者  当前场景权限最高者
        public PlayerInScene leader;

        // 收发数据uuidlist 缓存列表
        public ArrayList _uuid_sync_cache = new ArrayList();
        //private ArrayList _uuid_commond_cache = new ArrayList();
        //private ArrayList _uuid_operate_cache = new ArrayList();

        ~BaseRoomClass()
        {
            ClearScene();
        }

        public void ClearScene()
        {
            try
            {
                isinitclass = false;

                StopSyncClient();
                moveablesceneobject.Clear();
                sceneplaylist.Clear();
                sceneorderlist.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="name"></param>
        public virtual void CreateScene(string name, imodule model, bool isaddmodel = false)
        {
            this.scenename = name;
            this.isinitclass = false;

            if(isaddmodel)
            {
                if(model == null)
                {
                    model = this;
                }

                server.add_Hub_Model(this.scenename, model);
            }
        }

        /// <summary>
        /// 初始化服务器场景
        /// </summary>
        /// <param name="data"></param>
        public virtual void InitScenes(Hashtable data)
        {
            if (isinitclass)
            {
                Console.WriteLine("服务器场景数据已经初始化 无需重复初始化");
                return;
            }

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

            isinitclass = true;

            Console.WriteLine("初始化服务器场景数据完毕");
        }

        /// <summary>
        /// 关闭场景
        /// </summary>
        /// <param name="name"></param>
        public virtual void CloseScenesByName(string name = null)
        {

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

            foreach (PlayerInScene sp in sceneplaylist.Values)
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

            foreach (PlayerInScene sp in sceneplaylist.Values)
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

        public virtual void sceneDataConvert()
        {
        }

        /// <summary>
        /// 验证token与所在房间的权限 是否满足操作房间的权限
        /// </summary>
        /// <param name="token"></param>
        public virtual Enums.PermissionVerifyStatus VerifyPermission(string token)
        {
            Enums.PermissionVerifyStatus ret = Enums.PermissionVerifyStatus.None;

            do
            {
                if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
                {
                    ret = Enums.PermissionVerifyStatus.None;
                    break;
                }

                PlayerInScene pis = sceneplaylist[token];

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
        public virtual Enums.PermissionVerifyStatus InterveneOperate<T>(string token, T param, Enums.ModelEnums model = Enums.ModelEnums.None)
        {
            // 权限验证
            return VerifyPermission(token);
        }

        /// <summary>
        /// 切换模式
        /// </summary>
        /// <param name="tomodel"></param>
        public virtual void SwitchModelTo(Enums.ModelEnums tomodel)
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

                    foreach (PlayerInScene player in sceneplaylist.Values)
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

            Thread t = RoomManager.getInstance().FindThreadOfRoom(this.scenename);
            if (t == null)
            {
                t = RoomManager.getInstance().ApplyThreadForRoom(this.scenename);
            }
            else
            {
                RoomManager.getInstance().StartThreadForRoom(this.scenename);
            }
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
                    break;
                }

                if (!player.isleader)
                {
                    break;
                }

                if (player.permission < this.permission)
                {
                    // 权限不够
                    break;
                }
                else if (player.permission == this.permission && player.token != this.leader.token)
                {
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
                    case Enums.ModelEnums.Separate:
                        ret = true;
                        break;
                    case Enums.ModelEnums.SynchronousOne:   // 同步leader(可能是老师)  只有当前 leader 可操作
                        if (player.isleader && player.token == this.leader.token)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.ModelEnums.SynchronousOne_Fixed: // 在观学模式下的 同步模式  不允许 学生自由走动 操作 等等
                        if (player.isleader && player.token == this.leader.token)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    case Enums.ModelEnums.SynchronousMultiple:
                        ret = true;
                        break;
                    case Enums.ModelEnums.Collaboration:
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
                    case Enums.ModelEnums.Separate:
                        ret = true;
                        break;
                    case Enums.ModelEnums.SynchronousOne:   // 同步leader(可能是老师)  只有当前 leader 可操作
                        if (player.isleader && player.token == this.leader.token)
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
                    case Enums.ModelEnums.SynchronousMultiple:
                        ret = player.isCanOperate;   // 学生是被选中的 在观学模式下被老师选中的  
                        break;
                    case Enums.ModelEnums.Collaboration:
                        ret = true;
                        break;
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

                // 第一步 根据当前房间模式进行操作是否有效判别
                switch (this.model)
                {
                    case Enums.ModelEnums.Separate:
                        ret = false;
                        break;
                    case Enums.ModelEnums.SynchronousOne:
                        if (player.isleader && player.token == this.leader.token)
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
                    case Enums.ModelEnums.SynchronousMultiple:
                        if (player.isleader && player.token == this.leader.token)
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

                        ret = ret || player.isCanSend;   // 玩家权限 可能是被选中的
                        break;
                    case Enums.ModelEnums.Collaboration:
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
                    case Enums.ModelEnums.Separate:
                        ret = false;
                        break;
                    case Enums.ModelEnums.SynchronousOne:
                        if (player.isleader && player.token == this.leader.token)
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
                    case Enums.ModelEnums.SynchronousMultiple:
                        if (player.isleader && player.token == this.leader.token)
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
                        ret = (ret || player.isCanSend);
                        break;
                    case Enums.ModelEnums.Collaboration:
                        ret = player.isCanSend;
                        break;
                    default:
                        ret = false;                         // 其他情况不允许玩家操作修改当前服务器数据
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
                    case Enums.ModelEnums.None:
                        ret = false;
                        break;
                    case Enums.ModelEnums.Separate:
                        ret = false;
                        break;
                    default:
                        ret = true;
                        break;
                }

            } while (false);

            return ret;
        }

        ///                          以上函数是模式切换、以及玩家随意模式的核心        模式切换受权限级别控制                          /////////////////////////////

        // 根据token获取玩家
        public virtual PlayerInScene findPlayerByToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
            {
                Console.WriteLine("服务器玩家不存在 : " + "token : " + token + " sceneplaylist count : " + sceneplaylist.Count);
                return null;
            }

            return sceneplaylist[token];
        }

        public virtual void Switch_Model(string token, string tomodel, string uuid)
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

            if (!checkLeaderFeasible(sceneplaylist[token]))
            {
                // 权限不够
                Console.WriteLine("切换模式权限不够 : " + "token : " + token + " tomodel: " + tomodel);
                return;
            }

            Console.WriteLine("切换模式成功 : " + "token : " + token + " tomodel: " + tomodel + "  开始更改玩家状态");

            Enums.ModelEnums m = Utilities.getInstance().convertModelToEnum(tomodel);

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

        public virtual void Req_Object_Operate_permissions(string token, string objectname, string uuid)
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

                    if (o.lockpermission < ps.permission)
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

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "hold", ret ? "yes" : "no");
            }
        }

        public virtual void Req_Object_Release_permissions(string token, string objectname, string uuid)
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

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "release", ret ? "yes" : "no");
            }
        }

        public virtual void ChangeObjectAllOnce(string token, Hashtable clientallonce)
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

        public virtual void ChangePlayerAllOnce(string token, Hashtable clientallonce)
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

            if(!sceneplaylist.ContainsKey(token))
            {
                return;
            }

            do
            {
                sceneplaylist[token].Conversion(clientallonce);

            } while (false);
        }

        public virtual void ret_sync_commond(string typ, string commond, string token, string other, string uuid)
        {
            if (this.leader == null || this.leader.uuid == null)
            {
                return;
            }

            if (sceneplaylist.Count <= 0 || !sceneplaylist.ContainsKey(token))
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
            sceneorderlist.Add(new OrderInScene(0, token, typ, commond, other));  //后期加入时间机制
        }

        // 单独线程定时同步客户端数据
        public bool _syncstate = true;
        public virtual void SyncClient()
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
