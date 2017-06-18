using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class RoomManager : BaseManager
    {
        public static RoomManager getInstance()
        {
            return Singleton<RoomManager>.getInstance();
        }

        private Dictionary<string, RealRoom> allscenes = new Dictionary<string, RealRoom>();
        private Dictionary<Int64, RealRoom> allscenesbyid = new Dictionary<Int64, RealRoom>();

        private Dictionary<string, Thread> scenesthread = new Dictionary<string, Thread>();
        private Dictionary<Int64, Thread> scenesthreadbyid = new Dictionary<Int64, Thread>();

        ~RoomManager()
        {
            allscenes.Clear();
            scenesthread.Clear();
        }

        public RealRoom CreateRoomByName(string name)
        {
            RealRoom ret = null;

            do
            {
                if (allscenes.ContainsKey(name))
                {
                    ret = allscenes[name];
                    break;
                }

                RealRoom s = new RealRoom();
                s.CreateScene(name);
                allscenes.Add(name, s);

                ret = s;

            } while (false);

            return ret;
        }

        public RealRoom CreateRoomById(Int64 id)
        {
            RealRoom ret = null;

            do
            {
                if (allscenesbyid.ContainsKey(id))
                {
                    ret = allscenesbyid[id];
                    break;
                }

                RealRoom s = new RealRoom();
                int roomid = (int)id;
                s.CreateScene(roomid);
                allscenesbyid.Add(id, s);

                ret = s;

            } while (false);

            return ret;
        }

        public void DeleteRoomByName(string name)
        {
            try
            {
                RealRoom rr = allscenes[name];
                if(rr != null)
                {
                    rr.ClearScene();
                }
                allscenes.Remove(name);
                scenesthread.Remove(name);
            }
            catch
            {

            }

            Console.WriteLine("RoomManager 清除房间 : " + name);
        }

        public void DeleteRoomByNameById(Int64 id)
        {
            try
            {
                RealRoom rr = allscenesbyid[id];
                if (rr != null)
                {
                    rr.ResetClassRoom();
                }
                allscenesbyid.Remove(id);
                scenesthreadbyid.Remove(id);
            }
            catch
            {

            }

            Console.WriteLine("RoomManager 清除房间 : " + id);
        }

        public RealRoom FindRoomByName(string name)
        {
            RealRoom s = null;

            if (allscenes.ContainsKey(name))
            {
                s = allscenes[name];
            }

            return s;
        }

        public RealRoom FindRoomById(Int64 id)
        {
            RealRoom s = null;

            if (allscenesbyid.ContainsKey(id))
            {
                s = allscenesbyid[id];
            }

            return s;
        }

        public void ClearAllRoom()
        {
            try
            {
                foreach(RealRoom rr in allscenes.Values)
                {
                    rr.StopSyncClient();
                }
                allscenes.Clear();
                scenesthread.Clear();

                foreach (RealRoom rr in allscenesbyid.Values)
                {
                    rr.StopSyncClient();
                }
                allscenesbyid.Clear();
                scenesthreadbyid.Clear();
            }
            catch
            {

            }
        }
    }
}
