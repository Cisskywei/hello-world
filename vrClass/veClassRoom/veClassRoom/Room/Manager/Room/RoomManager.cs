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
                s.CreateScene(name,s);
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
                s.CreateScene(null, s);
                allscenesbyid.Add(id, s);

                ret = s;

            } while (false);

            return ret;
        }

        public Thread ApplyThreadForRoom(string name, bool istart = true)
        {
            Thread t = null;

            do
            {
                if (!allscenes.ContainsKey(name))
                {
                    break;
                }

                if(scenesthread.ContainsKey(name))
                {
                    t = scenesthread[name];
                    try
                    {
                        if (istart && t.ThreadState == ThreadState.Unstarted)
                        {
                            t.Start();
                        }
                    }catch
                    {

                    }

                    break;
                }

                RealRoom s = allscenes[name];

                Thread th = new Thread(s.SyncClient);
                scenesthread.Add(name, t);

                if(istart)
                {
                    th.Start();
                }

                t = th;

            } while (false);

            return t;
        }

        public Thread StartThreadForRoom(string name)
        {
            Thread t = null;

            do
            {
                if (!allscenes.ContainsKey(name))
                {
                    break;
                }

                if (scenesthread.ContainsKey(name))
                {
                    t = scenesthread[name];
                    try
                    {
                        if (t.ThreadState == ThreadState.Unstarted || t.ThreadState == ThreadState.Stopped || t.ThreadState == ThreadState.Aborted)
                        {
                            t.Start();
                        }
                    }
                    catch
                    {

                    }

                    break;
                }

            } while (false);

            return t;
        }

        public Thread FindThreadOfRoom(string name)
        {
            Thread t = null;

            try
            {
                t = scenesthread[name];
            }
            catch
            {

            }

            return t;
        }

        public void DeleteRoomByName(string name)
        {
            try
            {
                RealRoom rr = allscenes[name];
                if(rr != null)
                {
                    rr.ClearScene();
      //              server.clear_Hub_Model(rr.scenename, rr);
                }
                allscenes.Remove(name);
                scenesthread.Remove(name);
            }
            catch
            {

            }

            Console.WriteLine("RoomManager 清除房间 : " + name);
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
            }
            catch
            {

            }
        }
    }
}
