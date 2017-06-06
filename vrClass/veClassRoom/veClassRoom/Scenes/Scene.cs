using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using veClassRoom.Room;

namespace veClassRoom
{
    // 权限等级
    public enum AuthorityEnum
    {
        None = 0,
    }

    public class SceneObject
    {
        public Structs.sVector3 position;
        public Structs.sVector4 rotation;
        public Structs.sVector3 scale;

        public int state;
        public string locker;
        public bool locked;
        public bool changeorno;

        public string name;

        // 序列化的信息
        public Hashtable serializedata = new Hashtable();

        public SceneObject()
        {
            position = new Structs.sVector3();
            rotation = new Structs.sVector4();
            scale = new Structs.sVector3();

            locker = null;
            locked = false;
            changeorno = false;

            serializedata.Add("posx",position.x);
            serializedata.Add("posy",position.y);
            serializedata.Add("posz",position.z);

            serializedata.Add("rotx", rotation.x);
            serializedata.Add("roty", rotation.y);
            serializedata.Add("rotz", rotation.z);
            serializedata.Add("rotw", rotation.w);

            serializedata.Add("scalx", scale.x);
            serializedata.Add("scaly", scale.y);
            serializedata.Add("scalz", scale.z);
        }

        public void changeposition(float x, float y, float z)
        {
            position.x = x;
            position.y = y;
            position.z = z;

            changeorno = true;
        }

        public void changerotation(float x, float y, float z, float w)
        {
            rotation.x = x;
            rotation.y = y;
            rotation.z = z;
            rotation.w = w;

            changeorno = true;
        }

        public void changescale(float x, float y, float z)
        {
            scale.x = x;
            scale.y = y;
            scale.z = z;

            changeorno = true;
        }

        // 序列化自己
        public virtual Hashtable Serialize()
        {
            serializedata["posx"] = position.x;
            serializedata["posy"] = position.y;
            serializedata["posz"] = position.z;

            serializedata["rotx"] = rotation.x;
            serializedata["roty"] = rotation.y;
            serializedata["rotz"] = rotation.z;
            serializedata["rotw"] = rotation.w;

            serializedata["scalx"] = scale.x;
            serializedata["scaly"] = scale.y;
            serializedata["scalz"] = scale.z;

            changeorno = false;

            return serializedata;
        }

        //反序列化自己
        public virtual void Deserialization(Hashtable t)
        {
            if(t == null || t.Count <= 0)
            {
                return;
            }

            position.x = (float)Convert.ToDouble(t["posx"]);
            position.y = (float)Convert.ToDouble(t["posy"]);
            position.z = (float)Convert.ToDouble(t["posz"]);

            rotation.x = (float)Convert.ToDouble(t["rotx"]);
            rotation.y = (float)Convert.ToDouble(t["roty"]);
            rotation.z = (float)Convert.ToDouble(t["rotz"]);
            rotation.w = (float)Convert.ToDouble(t["rotw"]);

            scale.x = (float)Convert.ToDouble(t["scalx"]);
            scale.y = (float)Convert.ToDouble(t["scaly"]);
            scale.z = (float)Convert.ToDouble(t["scalz"]);
        }

        public virtual void Conversion(Hashtable t)
        {
            if (t == null || t.Count <= 0)
            {
                return;
            }

            if(t["posx"] != null)
            {
                position.x = (float)Convert.ToDouble(t["posx"]);
            }
            if (t["posy"] != null)
            {
                position.y = (float)Convert.ToDouble(t["posy"]);
            }
            if (t["posz"] != null)
            {
                position.z = (float)Convert.ToDouble(t["posz"]);
            }

            if (t["rotx"] != null)
            {
                rotation.x = (float)Convert.ToDouble(t["rotx"]);
            }
            if (t["roty"] != null)
            {
                rotation.y = (float)Convert.ToDouble(t["roty"]);
            }
            if (t["rotz"] != null)
            {
                rotation.z = (float)Convert.ToDouble(t["rotz"]);
            }
            if (t["rotw"] != null)
            {
                rotation.w = (float)Convert.ToDouble(t["rotw"]);
            }

            if (t["scalx"] != null)
            {
                scale.x = (float)Convert.ToDouble(t["scalx"]);
            }
            if (t["scaly"] != null)
            {
                scale.y = (float)Convert.ToDouble(t["scaly"]);
            }
            if (t["scalz"] != null)
            {
                scale.z = (float)Convert.ToDouble(t["scalz"]);
            }

            changeorno = true;
        }
    }

    public class ScenePlayer : SceneObject
    {
        public string token;
        public string uuid;

        public AuthorityEnum authority = AuthorityEnum.None;

        public ScenePlayer():base()
        { }

        public ScenePlayer(string token, string name, string uuid) : base()
        {
            this.token = token;
            this.name = name;
            this.uuid = uuid;
        }

        // 序列化自己
        public override Hashtable Serialize()
        {
            base.Serialize();

            return serializedata;
        }
    }

    public class SceneOrder
    {
        public int name;
        public int commond;

        // 序列化的信息
        public Hashtable serializedata = new Hashtable();

        public SceneOrder()
        {
            serializedata.Add("commond", commond);
        }

        public virtual Hashtable Serialize()
        {
            serializedata["commond"] = commond;

            return serializedata;
        }

        public virtual void Deserialization(Hashtable t)
        {
            if (t == null || t.Count <= 0)
            {
                return;
            }

            commond = (int)serializedata["commond"];
        }
    }

    /// <summary>
    /// 场景类
    /// </summary>
    class Scene : imodule
    {
        // 场景名称 亦作为module名字
        public string scenename;

        // 场景是否开始  即课程是否开始
        public bool istartclass = false;

        // 场景中需要同步的物体
        public Dictionary<string, SceneObject> moveablesceneobject = new Dictionary<string, SceneObject>();

        // 场景中的玩家列表
        public Dictionary<string, ScenePlayer> sceneplaylist = new Dictionary<string, ScenePlayer>();

        // 场景中的玩家uuid列表
        ArrayList _uuid_of_player = new ArrayList();

        //场景的指令
        public List<SceneOrder> sceneorderlist = new List<SceneOrder>();

        ~Scene()
        {
            _syncstate = false;

            moveablesceneobject.Clear();
            sceneplaylist.Clear();
            sceneorderlist.Clear();
        }
        
        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="name"></param>
        public void CreateScene(string name)
        {
            this.scenename = name;
            server.add_Hub_Model(this.scenename, this);
        }

        /// <summary>
        /// 初始化服务器场景
        /// </summary>
        /// <param name="data"></param>
        public void initScenes(Hashtable data)
        {
    //        if(istartclass)
            //{
            //    Console.WriteLine("服务器场景数据已经初始化 无需重复初始化");
            //    return;
            //}

            Console.WriteLine("初始化服务器场景数据" + data.Count);

            if(data.Count <= 0)
            {
                Console.WriteLine("服务器场景数据参数为空" + data.Count);
                return;
            }

            foreach(DictionaryEntry de in data)
            {
                if(moveablesceneobject.ContainsKey((string)de.Key))
                {
                    moveablesceneobject[(string)de.Key].Deserialization((Hashtable)de.Value);
                }
                else
                {
                    SceneObject s = new SceneObject();
                    s.name = (string)de.Key;
                    s.Deserialization((Hashtable)de.Value);
                    moveablesceneobject.Add(s.name, s);
                }
            }

    //        istartclass = true;

            Console.WriteLine("初始化服务器场景数据完毕");
        }

        /// <summary>
        /// 同步所有玩家场景
        /// </summary>
        public void SceneSynchronizationAll()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            Hashtable msgOrder = new Hashtable();

            // 同步数据
            foreach (SceneObject so in moveablesceneobject.Values)
            {
                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (ScenePlayer sp in sceneplaylist.Values)
            {
                // 同步客户端
                msgPlayer.Add(sp.name, sp.Serialize());
            }

            foreach (SceneOrder so in sceneorderlist)
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
        public void SceneSynchronizationPlayer(string uuid)
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            Hashtable msgOrder = new Hashtable();

            // 同步数据
            foreach (SceneObject so in moveablesceneobject.Values)
            {
                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (ScenePlayer sp in sceneplaylist.Values)
            {
                // 同步客户端
                msgPlayer.Add(sp.name, sp.Serialize());
            }

            foreach (SceneOrder so in sceneorderlist)
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

        private void sceneDataConvert()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            // 同步数据
            foreach (SceneObject so in moveablesceneobject.Values)
            {
                if (!so.changeorno)
                {
                    continue;
                }

                // 同步客户端
                msgObject.Add(so.name, so.Serialize());
            }

            foreach (ScenePlayer sp in sceneplaylist.Values)
            {
                if (!sp.changeorno)
                {
                    continue;
                }

                // 同步客户端
                msgPlayer.Add(sp.name, sp.Serialize());
            }

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0)
            {
                Console.WriteLine("同步客户端了哟");

                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
            }
        }

        /// <summary>
        /// 玩家进入场景
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public string PlayerEnterScene(string token, string name, string uuid)
        {
            do
            {
                if(sceneplaylist.ContainsKey(token))
                {
                    ScenePlayer p = sceneplaylist[token];
                    p.token = token;
                    p.name = name;
                    p.uuid = uuid;

                    // 判断是否是由于玩家掉线导致重新进入房间
                    //TODO
                }
                else
                {
                    ScenePlayer sp = new ScenePlayer(token,name,uuid);
                    sceneplaylist.Add(token, sp);
                }

                if(!_uuid_of_player.Contains(uuid))
                {
                    _uuid_of_player.Add(uuid);
                }

            } while (false);


            // 进行一次场景同步
            if(istartclass)
            {
                SceneSynchronizationPlayer(uuid);
                Console.WriteLine("token : " + token + "玩家进入场景 " + scenename + "并进行一次场景完全同步");
            }

            return this.scenename;
        }

        /// <summary>
        /// 玩家离开房间
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
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

            // 玩家离开释放 玩家锁住的物体
            foreach(SceneObject so in moveablesceneobject.Values)
            {
                if(!so.locked)
                {
                    continue;
                }

                if(so.locker == token)
                {
                    so.locker = null;
                    so.locked = false;
                }
            }

            Console.WriteLine("token : " + token + "玩家离开场景 " + scenename);
        }

        public bool Req_Object_Operate_permissions(string token, string objectname, string uuid)
        {
            bool ret = false;
            do
            {
                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + token + "请求操作物体" + objectname + "失败 因为服务器不存在该物体");
                    break;
                }

                SceneObject o = moveablesceneobject[objectname];
                if(o.locked)
                {
                    if(o.locker == token)
                    {
                        Console.WriteLine("token : " + token + "重复请求操作物体" + objectname);
                        ret = true;
                        break;
                    }

                    Console.WriteLine("token : " + token + "请求操作物体" + objectname + "失败 因为服务器该物体已被" + o.locker + "锁住");
                    break;
                }
                else
                {
                    o.locker = token;
                    o.locked = true;

                    ret = true;
                }

            } while (false);

            //if(uuid == null && sceneplaylist.ContainsKey(token))
            //{
            //    uuid = sceneplaylist[token].uuid;
            //}

            if(_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "hold", ret?"yes":"no");
            }

            return ret;
        }

        public bool Req_Object_Release_permissions(string token, string objectname, string uuid)
        {
            bool ret = false;
            do
            {
                if (!moveablesceneobject.ContainsKey(objectname))
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器不存在该物体");
                    break;
                }

                SceneObject o = moveablesceneobject[objectname];
                if (!o.locked)
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体锁已被释放");
                    break;
                }
                else if(o.locker != token)
                {
                    Console.WriteLine("token : " + token + "请求释放物体" + objectname + "操作权限失败 因为服务器该物体被 token : " + o.locker + "锁住");
                    break;
                }
                else
                {
                    o.locker = null;
                    o.locked = false;

                    ret = true;
                }

            } while (false);

            //if (uuid == null && sceneplaylist.ContainsKey(token))
            //{
            //    uuid = sceneplaylist[token].uuid;
            //}

            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_operation_permissions", token, objectname, "release", ret ? "yes" : "no");
            }

            return ret;
        }

        // 同步指令
        public void ret_sync_commond(string typ, string commond, string name, string objectname, string uuid)
        {
            if(_uuid_of_player == null || _uuid_of_player.Count <= 0)
            {
                return;
            }

            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_sync_commond", typ, commond, name, objectname);
        }

        // 客户端对物体的操作
        public void ChangeObjectPosition(string objectname, Double x, Double y, Double z)
        {
            if (!moveablesceneobject.ContainsKey(objectname))
            {
                return;
            }

            moveablesceneobject[objectname].changeposition((float)x, (float)y, (float)z);
        }

        public void ChangeObjectRotation(string objectname, Double x, Double y, Double z, Double w)
        {
            if (!moveablesceneobject.ContainsKey(objectname))
            {
                return;
            }

            moveablesceneobject[objectname].changerotation((float)x, (float)y, (float)z, (float)w);
        }

        public void ChangeObjectScale(string objectname, Double x, Double y, Double z)
        {
            if (!moveablesceneobject.ContainsKey(objectname))
            {
                return;
            }

            moveablesceneobject[objectname].changescale((float)x, (float)y, (float)z);
        }

        public void ChangePosRot(string objectname, Double x, Double y, Double z, Double sx, Double sy, Double sz, Double sw)
        {
            if (!moveablesceneobject.ContainsKey(objectname))
            {
                return;
            }

            moveablesceneobject[objectname].changeposition((float)x, (float)y, (float)z);
            moveablesceneobject[objectname].changerotation((float)sx, (float)sy, (float)sz, (float)sw);
        }

        // 客户端一次性把所有改动数据发送过来
        public void ChangeObjectAllOnce(string token, Hashtable clientallonce)
        {
            Console.WriteLine("客户端一次性把所有改动数据发送过来 " + clientallonce.Count);

            if(clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            foreach(DictionaryEntry de in clientallonce)
            {
                if(!moveablesceneobject.ContainsKey((string)de.Key))
                {
                    // 服务器不包含该物体
                    Console.WriteLine("服务器不包含该物体 " + de.Key);
                    continue;
                }

                var o = moveablesceneobject[(string)de.Key];

                if(!o.locked || o.locker == null)
                {
   //                 continue;
                }

                if (o.locker != token)
                {
 //                   continue;
                }

                o.Conversion((Hashtable)de.Value);
            }
        }

        // 单独线程定时同步客户端数据
        private bool _syncstate = true;
        public void SyncClient()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            while (_syncstate)
            {
                // 同步数据
                foreach(SceneObject so in moveablesceneobject.Values)
                {
                    if(!so.changeorno)
                    {
                        continue;
                    }

                    // 同步客户端
                    msgObject.Add(so.name, so.Serialize());
                }

                foreach(ScenePlayer sp in sceneplaylist.Values)
                {
                    if(!sp.changeorno)
                    {
                        continue;
                    }

                    // 同步客户端
                    msgPlayer.Add(sp.name, sp.Serialize());
                }

                // 同步客户端
                if(msgPlayer.Count > 0 || msgObject.Count > 0)
                {
                    Console.WriteLine("同步客户端了哟");

                    hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
                }

                // 同步之后清除
                msgObject.Clear();
                msgPlayer.Clear();

                Thread.Sleep(16);
            }
        }


        //只为推题测试
        public void QuestionTest(Int64 questionid)
        {
            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "QuestionTest", questionid);
        }

        public void FastQuestionTest()
        {
            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "FastQuestionTest");
        }

        public void ChangeModeState(Int64 modeid)
        {
            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ChangeModeState", modeid);
        }

        public void AnswerQuestion(Int64 optionid)
        {
            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "AnswerQuestion", optionid);
        }

        public void ShowWhiteBoard(Int64 openclose)
        {
            hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ShowWhiteBoard", openclose);
        }

    }
}
