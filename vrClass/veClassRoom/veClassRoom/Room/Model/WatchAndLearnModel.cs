using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class WatchAndLearnModel : BaseModelClass
    {
        public override void CheckOperationHold<T>(string token, string objectname, string uuid, T extrainfor, object[] param)
        {
            Console.WriteLine("拿起" + objectname);

            if (param == null)
            {
                return;
            }

            if (param.Length < 2 || param[1] == null)
            {
                return;
            }

            ArrayList allplayer = extrainfor as ArrayList;

            string leaderuuid = param[0] as string;

            bool ret = false;

            do
            {
                if(leaderuuid != uuid)
                {
                    break;
                }

                Dictionary<string, ObjectInScene> moveablesceneobject = param[1] as Dictionary<string, ObjectInScene>;

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

            if(allplayer != null && allplayer.Count > 0)
            {
                hub.hub.gates.call_group_client(allplayer, "cMsgConnect", "ret_operation_permissions", token, objectname, "hold", ret ? "yes" : "no");
            }

        }

        public override void CheckOperationRelease<T>(string token, string objectname, string uuid, T extrainfor, object[] param)
        {
            Console.WriteLine("释放" + objectname);

            if (param == null)
            {
                return;
            }

            if (param.Length < 2 || param[1] == null)
            {
                return;
            }

            ArrayList allplayer = extrainfor as ArrayList;

            string leaderuuid = param[0] as string;

            bool ret = false;
            do
            {
                if (leaderuuid != uuid)
                {
                    break;
                }

                Dictionary<string, ObjectInScene> moveablesceneobject = param[1] as Dictionary<string, ObjectInScene>;

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

                    ret = true;
                }

            } while (false);

            if (allplayer.Count > 0)
            {
                hub.hub.gates.call_group_client(allplayer, "cMsgConnect", "ret_operation_permissions", token, objectname, "release", ret ? "yes" : "no");
            }

        }

        public override void CheckChangeObjectAllOnce<T>(string token, Hashtable clientallonce, T extrainfor, object[] param)
        {
            Dictionary<string, ObjectInScene> clientall = extrainfor as Dictionary<string, ObjectInScene>;

            do
            {
                if (param == null)
                {
                    break;
                }

                if (param.Length < 2 || param[1] == null)
                {
                    break;
                }

                string leaderuuid = param[0] as string;
                string uuid = param[1] as string;

                if (leaderuuid != uuid)
                {
                    break;
                }

                foreach (DictionaryEntry de in clientallonce)
                {
                    if (!clientall.ContainsKey((string)de.Key))
                    {
                        // 服务器不包含该物体
                        continue;
                    }

                    var o = clientall[(string)de.Key];

                    if (!o.locked || o.locker == null)
                    {
                        continue;
                    }

                    if (o.locker != token)
                    {
                        continue;
                    }

                    o.Conversion((Hashtable)de.Value);
                }

            } while (false);
        }

        public override void CheckSyncCommond<T>(string typ, string commond, string name, string objectname, string uuid, T extrainfor, object[] param)
        {
            Console.WriteLine("检测指令" + typ + commond + name);

            do
            {
                if (param == null)
                {
                    break;
                }

                if (param.Length <= 0 || param[0] == null)
                {
                    break;
                }

                PlayerInScene p = param[0] as PlayerInScene;

                string leaderuuid = p.uuid;

                if(leaderuuid != uuid)
                {
                    break;
                }

                ArrayList a = extrainfor as ArrayList;

                foreach (string uid in a)
                {
                    if(uid == leaderuuid)
                    {
                        continue;
                    }

                    hub.hub.gates.call_client(uid, "cMsgConnect", "ret_sync_commond", typ, commond, name, objectname);
                }

      //          hub.hub.gates.call_group_client(a, "cMsgConnect", "ret_sync_commond", typ, commond, name, objectname);

            } while (false);

        }

        public override void CheckSyncClient<T>(T extrainfor, object[] param)
        {
            do
            {
                ArrayList allplayer = extrainfor as ArrayList;

                if (param == null)
                {
                    break;
                }

                if (param.Length < 3)
                {
                    break;
                }

                try
                {
                    string leaduuid = param[0] as string;

                    Hashtable msgObject = param[1] as Hashtable;
                    Hashtable msgPlayer = param[2] as Hashtable;

                    Dictionary<string, ObjectInScene> moveablesceneobject = param[3] as Dictionary<string, ObjectInScene>;
                    Dictionary<string, PlayerInScene> sceneplaylist = param[4] as Dictionary<string, PlayerInScene>;

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

                    allplayer.Remove(leaduuid);

                    // 同步客户端
                    if ((msgPlayer.Count > 0 || msgObject.Count > 0) && allplayer.Count > 0)
                    {
                        Console.WriteLine("真的同步客户端了哟");

                        hub.hub.gates.call_group_client(allplayer, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
                    }

                    allplayer.Add(leaduuid);

                    // 同步之后清除
                    msgObject.Clear();
                    msgPlayer.Clear();
                }
                catch
                {

                }

            } while (false);

        }
    }
}
