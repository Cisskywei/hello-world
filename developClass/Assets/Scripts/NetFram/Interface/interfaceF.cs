using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    //public class interfaceF 
    //{
        public interface sync_commond
        {
        void ret_sync_commond(string typ, string commond, string name, string objectname);
        }

    public interface sync_string_message
    {
        void listen_string_msg(string msg);
    }

    //}
}

