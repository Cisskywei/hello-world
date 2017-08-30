using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class SelfTrainGroup : BaseModel
    {
        public PlayerInScene teacher;
        public Dictionary<int, Team> groups = new Dictionary<int, Team>();

        public override void InitModel(params object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return;
            }

            PlayerInScene t = (PlayerInScene)args[0];
            Dictionary<int, Team> a = (Dictionary< int, Team>)args[1];
            InitModel(t, a);
        }

        public void InitModel(PlayerInScene t, Dictionary<int, Team> all)
        {
            teacher = t;
            groups = all;
        }

        public override void CheckOperationHold<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            Team t = findgroupbystudentid(id);
            if (t == null)
            {
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                int oid = (int)objecid;

                if (!t.CheckIsHaveObject(oid))
                {
                    Console.WriteLine("token : " + userid + "请求操作物体" + oid + "失败 因为服务器不存在该物体");
                    break;
                }

                ret = t.LockObject(id, oid);

            } while (false);

            hub.hub.gates.call_group_client(t._uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objecid, "hold", ret ? "yes" : "no");
        }

        public override void CheckOperationRelease<T>(long userid, long objecid, T extrainfor, object[] param)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            Team t = findgroupbystudentid(id);
            if (t == null)
            {
                return;
            }

            //具体的操作控制
            bool ret = false;
            do
            {
                int oid = (int)objecid;

                if (!t.CheckIsHaveObject(oid))
                {
                    Console.WriteLine("token : " + userid + "请求释放物体" + oid + "失败 因为服务器不存在该物体");
                    break;
                }

                ret = t.ReleaseObject(id, oid);

            } while (false);

            hub.hub.gates.call_group_client(t._uuid_of_player, "cMsgConnect", "ret_operation_permissions", userid, objecid, "release", ret ? "yes" : "no");
        }

        public override void CheckSyncCommond<T>(long userid, string typ, string commond, T extrainfor, object[] param = null)
        {
            int id = (int)userid;

            if (this.teacher == null || this.teacher.uuid == null)
            {
                return;
            }

            Team t = findgroupbystudentid(id);
            if (t == null)
            {
                return;
            }

            hub.hub.gates.call_group_client(t._uuid_of_player, "cMsgConnect", "ret_sync_commond", userid, typ, commond, extrainfor);

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

            Team t = findgroupbystudentid(id);
            if (t == null)
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
                    t.ChangeStudentData(id, player);
                }

                if (objects != null && objects.Count > 0)
                {
                    foreach (DictionaryEntry de in objects)
                    {
                        int idkey = Convert.ToInt32(de.Key);
                        if (!t.CheckIsHaveObject(idkey))
                        {
                            // 服务器不包含该物体
                            continue;
                        }

                        t.ChangeObjectData(idkey, (Hashtable)de.Value);
                    }
                }

            } while (false);
        }

        public bool _syncstate = false;
        public void SyncClient(long tick)
        {
            Hashtable msgObject = null;
            Hashtable msgPlayer = null;
            foreach (Team g in groups.Values)
            {
                msgObject = g.GetChangedObject();
                msgPlayer = g.GetChangedPlayer();

                if (msgPlayer.Count > 0 || msgObject.Count > 0)
                {
                    if (g._uuid_of_player.Count > 0)
                    {
                        hub.hub.gates.call_group_client(g._uuid_of_player, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
                    }
                }

                msgObject.Clear();
                msgPlayer.Clear();
            }

            if (_syncstate)
            {
                hub.hub.timer.addticktime(400, SyncClient);
            }
        }

        public override void PlayerLeave(int userid)
        {
            Team t = findgroupbystudentid(userid);
            if(t!=null)
            {
                t.PlayerLeave(userid);
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
            groups.Clear();
        }

        //////////////////////////////////////  私有辅助函数   ////////////////////////////////////
        private Team findgroupbygroupid(int id)
        {
            Team t = null;

            if(groups.ContainsKey(id))
            {
                t = groups[id];
            }

            return t;
        }

        private Team findgroupbystudentid(int id)
        {
            Team t = null;

            foreach(Team g in groups.Values)
            {
                if(g.CheckIsHaveStudent(id))
                {
                    t = g;
                    break;
                }
            }

            return t;
        }
    }
}
