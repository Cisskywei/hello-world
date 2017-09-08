using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class SelfTrainPersonal : BaseModel
    {
        public PlayerInScene teacher;
        public PlayerInScene student; // 被指导的学生
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();

        private ArrayList _uuid_of_player = new ArrayList();

        public override void InitModel(params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return;
            }

            PlayerInScene t = (PlayerInScene)args[0];
            PlayerInScene s = (PlayerInScene)args[1];
            Dictionary<int, ObjectInScene> o = (Dictionary<int, ObjectInScene>)args[2];
            InitModel(t, s, o);
        }

        public void InitModel(PlayerInScene t, PlayerInScene s, Dictionary<int, ObjectInScene> allo)
        {
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

            _uuid_of_player.Add(teacher.uuid);
            _uuid_of_player.Add(student.uuid);
        }

        public override void CheckOperationHold<T>(long userid, long objecid, T extrainfor, object[] param)
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
                        else if (id == student.selfid)
                        {
                            Console.WriteLine("token : " + student.selfid + "无权操作 token  " + o.locker + " 的物体 " + oid);
                        }
                    }

                    break;
                }

            } while (false);

            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.CourseWave);
            msg.Add((Int64)CommandDefine.SecondLayer.ObjectOperate);
            msg.Add((Int64)objecid);
            msg.Add((Int64)Enums.ObjectOperate.Hold);
            msg.Add((Int64)userid);
            msg.Add((Int64)(ret ? 1 : 0));

            hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
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

            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.CourseWave);
            msg.Add((Int64)CommandDefine.SecondLayer.ObjectOperate);
            msg.Add((Int64)objecid);
            msg.Add((Int64)Enums.ObjectOperate.Release);
            msg.Add((Int64)userid);
            msg.Add((Int64)(ret ? 1 : 0));

            hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.Command_func, (Int64)userid, msg);
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

            if (!(this.teacher.selfid == userid || this.student.selfid == userid))
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
                    if(id == student.selfid)
                    {
                        student.Change3DInfor(player);
                    }
                    else if (id == teacher.selfid)
                    {
                        teacher.Change3DInfor(player);
                    }
                }

                if (objects != null && objects.Count > 0 && (teacher.selfid == id || student.selfid == id))
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

            if(student._ischange)
            {
                msgPlayer.Add(student.selfid.ToString(), student.Get3DInfor());
            }

            if (teacher._ischange)
            {
                msgPlayer.Add(teacher.selfid.ToString(), teacher.Get3DInfor());
            }

            // 同步指令
            //TODO

            // 同步客户端
            if (msgPlayer.Count > 0 || msgObject.Count > 0)
            {
                if (_uuid_of_player.Count > 0)
                {
                    hub.hub.gates.call_group_client(_uuid_of_player, NetConfig.client_module_name, NetConfig.ChangeAllOnce_func, msgObject, msgPlayer);
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
            if (student.selfid == (userid))
            {
                // TODO
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
            allobjects.Clear();
            _uuid_of_player.Clear();
        }
    }
}
