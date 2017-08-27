using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class Team
    {
        public int selfid = -1;

        public int teacherid = -1;
        public Dictionary<int, PlayerInScene> allstudents = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();

        public ArrayList _uuid_of_player = new ArrayList();

        public void InitModel(int tid, Dictionary<int, PlayerInScene> all, Dictionary<int, ObjectInScene> allo)
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

            teacherid = tid;
        }

        public bool CheckIsHaveStudent(int id)
        {
            bool ret = false;

            ret = allstudents.ContainsKey(id);

            return ret;
        }

        public bool CheckIsHaveObject(int id)
        {
            bool ret = false;

            ret = allobjects.ContainsKey(id);

            return ret;
        }

        public void ChangeStudentData(int id, Hashtable data)
        {
            if (data == null || data.Count <= 0)
            {
                return;
            }

            if (!allstudents.ContainsKey(id))
            {
                return;
            }

            allstudents[id].Change3DInfor(data);
        }

        public void ChangeObjectData(int id, Hashtable data)
        {
            if (data == null || data.Count <= 0)
            {
                return;
            }

            if (!allobjects.ContainsKey(id))
            {
                return;
            }

            allobjects[id].Change3DInfor(data);
        }

        public bool LockObject(int locker, int objectid)
        {
            bool ret = false;

            if(!allobjects.ContainsKey(objectid))
            {
                return ret;
            }

            ObjectInScene o = allobjects[objectid];

            if (o.locker < 0)
            {
                o.locker = locker;
                ret = true;
            }
            else
            {
                if (o.locker == locker)
                {
                    Console.WriteLine("token : " + locker + "重复请求操作物体" + objectid);
                }
                else
                {
                    if (locker == teacherid)
                    {
                        Console.WriteLine("token : " + teacherid + "权限较高夺取了 token  " + o.locker + " 的物体 " + objectid);

                        o.locker = teacherid;

                        ret = true;
                    }
                    else
                    {
                        Console.WriteLine("token : " + locker + "无权操作 token  " + o.locker + " 的物体 " + objectid);

                        ret = false;
                    }
                }
            }

            return ret;
        }

        public bool ReleaseObject(int locker, int objectid)
        {
            bool ret = false;

            if (!allobjects.ContainsKey(objectid))
            {
                return ret;
            }

            ObjectInScene o = allobjects[objectid];

            if (o.locker < 0)
            {
                ret = false;
            }
            else
            {
                if (o.locker == locker)
                {
                    o.locker = -1;

                    ret = true;

                    Console.WriteLine("token : " + locker + "请求释放物体成功" + objectid);
                }
                else
                {
                    Console.WriteLine("token : " + locker + "无权释放 token  " + o.locker + " 的物体 " + objectid);

                    ret = false;
                }
            }

            return ret;
        }


        public Hashtable GetChangedPlayer()
        {
            Hashtable msgPlayer = new Hashtable();

            foreach (PlayerInScene sp in allstudents.Values)
            {
                if (!sp._ischange)
                {
                    continue;
                }

                // 同步客户端
                msgPlayer.Add(sp.selfid, sp.Get3DInfor());
            }

            return msgPlayer;
        }

        public Hashtable GetChangedObject()
        {
            Hashtable msgObject = new Hashtable();

            foreach (ObjectInScene so in allobjects.Values)
            {
                if (!so._ischange)
                {
                    continue;
                }

                // 同步客户端
                msgObject.Add(so.selfid, so.Get3DInfor());
            }

            return msgObject;
        }

        //人员添加和离开
        public void AddPlayer(PlayerInScene player)
        {
            if(!allstudents.ContainsKey(player.selfid))
            {
                allstudents.Add(player.selfid, player);
            }
        }

        public void PlayerLeave(int userid)
        {
            if (allstudents.ContainsKey(userid))
            {
                string uuid = allstudents[userid].uuid;
                if(uuid != null && _uuid_of_player.Contains(uuid))
                {
                    _uuid_of_player.Remove(uuid);
                }

                allstudents.Remove(userid);
            }

            foreach (ObjectInScene os in allobjects.Values)
            {
                if (os.locker == userid)
                {
                    os.locker = -1;
                }
            }
        }
    }
}
