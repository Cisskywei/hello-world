using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class RoomManager
    {
        public static RoomManager getInstance()
        {
            return Singleton<RoomManager>.getInstance();
        }

        private Dictionary<int, ClassRoom> allscenesbyid = new Dictionary<int, ClassRoom>();

        ~RoomManager()
        {
            allscenesbyid.Clear();
        }

        public ClassRoom CreateRoomById(int id)
        {
            ClassRoom ret = null;

            do
            {
                if (allscenesbyid.ContainsKey(id))
                {
                    ret = allscenesbyid[id];
                    break;
                }

                ClassRoom s = new ClassRoom();
                //int roomid = (int)id;
                //s.CreateScene(roomid);
                allscenesbyid.Add(id, s);

                ret = s;

            } while (false);

            return ret;
        }

        public void DeleteRoomByNameById(int id)
        {
            try
            {
                ClassRoom rr = allscenesbyid[id];
                if (rr != null)
                {
            //        rr.ResetClassRoom();
                }
                allscenesbyid.Remove(id);
            }
            catch
            {

            }

            Console.WriteLine("RoomManager 清除房间 : " + id);
        }

        public ClassRoom FindRoomById(int id)
        {
            ClassRoom s = null;

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
                foreach (ClassRoom rr in allscenesbyid.Values)
                {
             //       rr.StopSyncClient();
                }
                allscenesbyid.Clear();
            }
            catch
            {

            }
        }
    }
}
