using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class GuidanceModePersonal : BaseModel
    {
        public PlayerInScene teacher;
        public PlayerInScene student; // 被指导的学生
        public Dictionary<int, PlayerInScene> allstudents = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();

        private ArrayList _uuid_of_player = new ArrayList();

        public void InitModel(PlayerInScene t, PlayerInScene s, Dictionary<int, PlayerInScene> all, Dictionary<int, ObjectInScene> allo)
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

            if (!allstudents.ContainsKey(teacher.selfid))
            {
                allstudents.Add(teacher.selfid, teacher);
            }

            if (!allstudents.ContainsKey(student.selfid))
            {
                allstudents.Add(student.selfid, student);
            }

            if (!_uuid_of_player.Contains(teacher.uuid))
            {
                _uuid_of_player.Add(teacher.uuid);
            }

            if (!_uuid_of_player.Contains(student.uuid))
            {
                _uuid_of_player.Add(student.uuid);
            }
        }

        public override void CheckOperationHold<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if (!(this.teacher.selfid == userid||this.student.selfid == userid))
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
                        if(id == teacher.selfid)
                        {
                            Console.WriteLine("token : " + teacher.selfid + "权限较高夺取了 token  " + o.locker + " 的物体 " + oid);

                            o.locker = teacher.selfid;

                            ret = true;
                        }
                        else if(id == student.selfid)
                        {
                            Console.WriteLine("token : " + student.selfid + "无权操作 token  " + o.locker + " 的物体 " + oid);
                        }
                    }

                    break;
                }

            } while (false);

            ArrayList uuids = new ArrayList();
            uuids.Add(teacher.uuid);
            uuids.Add(student.uuid);
            hub.hub.gates.call_group_client(uuids, "cMsgConnect", "ret_operation_permissions", userid, objecid, "hold", ret ? "yes" : "no");
        }

        public override void CheckOperationRelease<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            if (!(this.teacher.selfid == userid || this.student.selfid == userid))
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

            ArrayList uuids = new ArrayList();
            uuids.Add(teacher.uuid);
            uuids.Add(student.uuid);
            hub.hub.gates.call_group_client(uuids, "cMsgConnect", "ret_operation_permissions", userid, objecid, "release", ret ? "yes" : "no");
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

                if (objects != null && objects.Count > 0 && (teacher.selfid == id || student.selfid == id))
                {
                    foreach (DictionaryEntry de in objects)
                    {
                        if (!allobjects.ContainsKey((int)(de.Key)))
                        {
                            // 服务器不包含该物体
                            continue;
                        }

                        var o = allobjects[(int)de.Key];

                        if (o.locker < 0 || !(teacher.selfid == id || student.selfid == id))
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
                msgObject.Add(so.selfid, so.Get3DInfor());
            }

            foreach (PlayerInScene sp in allstudents.Values)
            {
                if (!sp._ischange)
                {
                    continue;
                }

                // 同步客户端
                msgPlayer.Add(sp.selfid, sp.Get3DInfor());
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

            if (student.selfid == (userid))
            {
                //
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
            _syncstate = true;
            SyncClient(0);
        }

        public override void StopSynclient()
        {
            _syncstate = false;
        }

        public override void ClearData()
        {
            allstudents.Clear();
            allobjects.Clear();
            _uuid_of_player.Clear();
        }
    }
}
