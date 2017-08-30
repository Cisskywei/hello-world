using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class GuidanceModeGroup : BaseModel
    {
        public PlayerInScene teacher;
        public Dictionary<int, PlayerInScene> student = new Dictionary<int, PlayerInScene>(); // 被指导的小组
        public Dictionary<int, PlayerInScene> allstudents = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();

        private ArrayList _uuid_of_player = new ArrayList();
        private ArrayList _uuid_of_group = new ArrayList();

        public override void InitModel(params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return;
            }

            PlayerInScene t = (PlayerInScene)args[0];
            Dictionary<int, PlayerInScene> s = (Dictionary<int, PlayerInScene>)args[1];
            Dictionary<int, PlayerInScene> a = (Dictionary<int, PlayerInScene>)args[2];
            Dictionary<int, ObjectInScene> o = (Dictionary<int, ObjectInScene>)args[3];
            InitModel(t, s, a, o);
        }

        public void InitModel(PlayerInScene t, Dictionary<int, PlayerInScene> s, Dictionary<int, PlayerInScene> all, Dictionary<int, ObjectInScene> allo)
        {
            // 初始化人物
            if (allstudents.Count <= 0)
            {
                foreach (KeyValuePair<int, PlayerInScene> ps in all)
                {
                    allstudents.Add(ps.Key, ps.Value);
                    if (!_uuid_of_player.Contains(ps.Value.uuid))
                    {
                        _uuid_of_player.Add(ps.Value.uuid);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, PlayerInScene> ps in all)
                {
                    if (allstudents.ContainsKey(ps.Key))
                    {
                        allstudents[ps.Key] = ps.Value;
                    }
                    else
                    {
                        allstudents.Add(ps.Key, ps.Value);
                    }

                    if (!_uuid_of_player.Contains(ps.Value.uuid))
                    {
                        _uuid_of_player.Add(ps.Value.uuid);
                    }
                }
            }

            // 初始化物体
            if (allobjects.Count <= 0)
            {
                foreach (KeyValuePair<int, ObjectInScene> ps in allo)
                {
                    allobjects.Add(ps.Key, ps.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<int, ObjectInScene> ps in allo)
                {
                    if (allobjects.ContainsKey(ps.Key))
                    {
                        allobjects[ps.Key] = ps.Value;
                    }
                    else
                    {
                        allobjects.Add(ps.Key, ps.Value);
                    }
                }
            }

            teacher = t;
            student = s;

            foreach(KeyValuePair<int,PlayerInScene> gs in student)
            {
                _uuid_of_group.Add(gs.Value.uuid);
            }

            _uuid_of_group.Add(teacher.uuid);

            if (!allstudents.ContainsKey(teacher.selfid))
            {
                allstudents.Add(teacher.selfid, teacher);
            }

            if (!_uuid_of_player.Contains(teacher.uuid))
            {
                _uuid_of_player.Add(teacher.uuid);
            }
        }

        public override void CheckOperationHold<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if (!(checklegal(id)))
            {
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                int oid = (int)objecid;

                if (!allobjects.ContainsKey(oid))
                {
                    Console.WriteLine("token : " + userid + "请求操作物体" + oid + "失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = allobjects[oid];
                if (o.locker < 0)
                {
                    o.locker = id;
                    ret = true;
                }
                else
                {
                    if (o.locker == id)
                    {
                        Console.WriteLine("token : " + teacher.selfid + "重复请求操作物体" + oid);
                    }
                    else
                    {
                        if (id == teacher.selfid)
                        {
                            Console.WriteLine("token : " + teacher.selfid + "权限较高夺取了 token  " + o.locker + " 的物体 " + oid);

                            o.locker = teacher.selfid;

                            ret = true;
                        }
                        else
                        {
                            Console.WriteLine("token : " + id + "无权操作 token  " + o.locker + " 的物体 " + oid);

                            ret = false;
                        }
                    }

                    break;
                }

            } while (false);

            hub.hub.gates.call_group_client(_uuid_of_group, "cMsgConnect", "ret_operation_permissions", userid, objecid, "hold", ret ? "yes" : "no");
        }

        public override void CheckOperationRelease<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if (!(checklegal(id)))
            {
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                int oid = (int)objecid;

                if (!allobjects.ContainsKey(oid))
                {
                    Console.WriteLine("token : " + userid + "请求释放物体" + oid + "失败 因为服务器不存在该物体");
                    break;
                }

                ObjectInScene o = allobjects[oid];
                if (o.locker < 0)
                {
                    ret = false;
                }
                else
                {
                    if (o.locker == id)
                    {
                        o.locker = -1;

                        ret = true;

                        Console.WriteLine("token : " + teacher.selfid + "请求释放物体成功" + oid);
                    }
                    else
                    {
                        Console.WriteLine("token : " + teacher.selfid + "无权释放 token  " + o.locker + " 的物体 " + oid);

                        ret = false;
                    }

                    break;
                }

            } while (false);

            hub.hub.gates.call_group_client(_uuid_of_group, "cMsgConnect", "ret_operation_permissions", userid, objecid, "release", ret ? "yes" : "no");
        }

        public override void CheckSyncCommond<T>(long userid, string typ, string commond, T extrainfor, object[] param = null)
        {
            if (_uuid_of_player.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "ret_sync_commond", userid, typ, commond, extrainfor);
            }

            //指令缓存
            //TODO
        }

        public override void CheckChangeObjectAllOnce<T>(long userid, Hashtable clientallonce, T extrainfor, object[] param)
        {
            if (clientallonce == null || clientallonce.Count <= 0)
            {
                return;
            }

            int id = (int)userid;

            if (teacher == null || teacher.uuid == null)
            {
                return;
            }

            if (!allstudents.ContainsKey(id))
            {
                return;
            }

            do
            {
                Hashtable player = null;
                Hashtable objects = null;

                if (clientallonce.ContainsKey("player"))
                {
                    player = (Hashtable)clientallonce["player"];
                }

                if (clientallonce.ContainsKey("objects"))
                {
                    objects = (Hashtable)clientallonce["objects"];
                }

                if (player != null && player.Count > 0)
                {
                    allstudents[id].Change3DInfor(player);
                }

                if (objects != null && objects.Count > 0 && (checklegal(id)))
                {
                    foreach (DictionaryEntry de in objects)
                    {
                        int idkey = Convert.ToInt32(de.Key);
                        if (!allobjects.ContainsKey(idkey))
                        {
                            // 服务器不包含该物体
                            continue;
                        }

                        var o = allobjects[idkey];

                        if (o.locker < 0 || !(checklegal(o.locker)))
                        {
                            continue;
                        }

                        o.Change3DInfor((Hashtable)de.Value);
                    }
                }

            } while (false);
        }

        public bool _syncstate = false;
        public void SyncClient(long tick)
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            // 同步数据
            foreach (ObjectInScene so in allobjects.Values)
            {
                if (!so._ischange)
                {
                    continue;
                }

                // 同步客户端
                msgObject.Add(so.selfid.ToString(), so.Get3DInfor());
            }

            foreach (PlayerInScene sp in allstudents.Values)
            {
                if (!sp._ischange)
                {
                    continue;
                }

                // 同步客户端
                msgPlayer.Add(sp.selfid.ToString(), sp.Get3DInfor());
            }

            // 同步指令
            //TODO

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0)
            {
                if (_uuid_of_player.Count > 0)
                {
                    hub.hub.gates.call_group_client(_uuid_of_player, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
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

        public override void PlayerLeave(int userid)
        {
            if (allstudents.ContainsKey(userid))
            {
                string uuid = allstudents[userid].uuid;
                if (uuid != null && _uuid_of_player.Contains(uuid))
                {
                    _uuid_of_player.Remove(uuid);
                }

                allstudents.Remove(userid);
            }

            if (student.ContainsKey(userid))
            {
                string uuid = student[userid].uuid;
                if (uuid != null && _uuid_of_group.Contains(uuid))
                {
                    _uuid_of_group.Remove(uuid);
                }

                student.Remove(userid);
            }

            foreach (ObjectInScene os in allobjects.Values)
            {
                if (os.locker == userid)
                {
                    os.locker = -1;
                }
            }
        }

        public override void StartSynclient()
        {
            if (_syncstate)
            {
                Console.WriteLine("同步已经开启 ");
                return;
            }

            _syncstate = true;
            SyncClient(0);
        }

        public override void StopSynclient()
        {
            _syncstate = false;
        }

        public override void ClearData()
        {
            student.Clear();
            allstudents.Clear();
            allobjects.Clear();
            _uuid_of_player.Clear();
            _uuid_of_group.Clear();
        }

        //////////////////////////////////////  私有辅助函数   ////////////////////////////////////
        private bool checklegal(int id)
        {
            bool ret = false;

            ret = (teacher.selfid == id) || (student.ContainsKey(id));

            return ret;
        }
    }
}
