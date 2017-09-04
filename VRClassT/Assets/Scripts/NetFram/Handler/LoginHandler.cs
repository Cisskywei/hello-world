using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public interface msg_req_ret
    {
        void req_msg(Hashtable msg);
        void ret_msg(Hashtable msg);
    }

    public interface msg_req_ret_json
    {
        //void req_msg_json(string msg);
        void ret_msg_json(string msg);
    }

    public interface sync_req_ret
    {
        //void req_sync_vector3(float x, float y, float z, Config.SyncTransform prs, int selfid, string uuid, string contactmodule = null, string contactfunc = null);
        //void ret_sync_vector3(float x, float y, float z, Config.SyncTransform prs, int selfid);
    }

    public interface LoginHandler
    {
        void req_login(Hashtable msg);
        void ret_login(Hashtable msg);
    }

    public interface EnterRoomHandler
    {
        void req_EnterRoom(Hashtable msg);
        void ret_EnterRoom(Hashtable msg);
    }
}

