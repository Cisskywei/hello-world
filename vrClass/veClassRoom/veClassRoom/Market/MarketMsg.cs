using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Market
{
    class MarketMsg : imodule
    {
        public static string selfmodelname = "market";

        public static Dictionary<Int64, string> allplayerlogin = new Dictionary<Int64, string>();

        private static int _leader = -1;

        public void login(Int64 name, Int64 isleader)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            if (isleader == 0)
            {
                // 不是leader
                if (allplayerlogin.ContainsKey(name))
                {
                    allplayerlogin[name] = client_uuid;
                }
                else
                {
                    allplayerlogin.Add(name, client_uuid);
                }
            }
            else if(isleader == 1)
            {
                // 是 leader
                _leader = (int)name;
            }

            Console.WriteLine(client_uuid + " -- " + allplayerlogin.Count);

            hub.hub.gates.call_client(client_uuid, "cMsgConnect", "ret_login", name, client_uuid);

        }

        public void sync_commond_vector3(Int64 userid, string typ, Double x, Double y, Double z)
        {
            if (userid != _leader)
            {
                return;
            }

            if (allplayerlogin.Count > 0)
            {
                ArrayList a = new ArrayList();
                foreach (string v in allplayerlogin.Values)
                {
                    a.Add(v);

                    Console.WriteLine(v);
                }

                Console.WriteLine(a.Count);

                hub.hub.gates.call_group_client(a, "cMsgConnect", "ret_sync_commond", typ, x, y, z);
            }
        }

        public void sync_vector3(Int64 userid, Double x, Double y, Double z)
        {
            if(userid != _leader)
            {
                return;
            }

            if(allplayerlogin.Count > 0)
            {
                ArrayList a = new ArrayList();
                foreach(string v in allplayerlogin.Values)
                {
                    a.Add(v);

                    Console.WriteLine(v);
                }

                Console.WriteLine(a.Count);

                hub.hub.gates.call_group_client(a, "cMsgConnect", "ret_sync_vector3", x, y, z);
            }
        }

        public void player_exit(Int64 userid)
        {
            if(userid == _leader)
            {
                allplayerlogin.Clear();
                _leader = -1;
            }
            else
            {
                if(allplayerlogin.ContainsKey(userid))
                {
                    allplayerlogin.Remove(userid);
                }
            }
        }
    }
}
