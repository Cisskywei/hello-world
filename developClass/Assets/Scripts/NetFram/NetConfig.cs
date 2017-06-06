using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class NetConfig
    {
        public static string service_ip = "192.168.0.129";//"192.168.0.101";//"58.213.74.230"; // 192.168.157.162
        public static short service_port = 1237;

        // rpc 调用相关
        public static string login_module_name = "cLogin";
        public static string enterroom_module_name = "cEnterRoom"; 
        public static string msgconnect_module_name = "cMsgConnect";

    }
}

