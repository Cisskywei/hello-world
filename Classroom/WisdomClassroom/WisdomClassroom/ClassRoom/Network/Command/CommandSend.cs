using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class CommandSend
    {
        public void SendCommand(string uuid, ArrayList msg)
        {
            hub.hub.gates.call_client(uuid, NetConfig.client_module_name, NetConfig.Command_func, (msg));
        }

        public void SendCommand(ArrayList uuids, ArrayList msg)
        {
            hub.hub.gates.call_group_client(uuids, NetConfig.client_module_name, NetConfig.Command_func, (msg));
        }
    }
}
