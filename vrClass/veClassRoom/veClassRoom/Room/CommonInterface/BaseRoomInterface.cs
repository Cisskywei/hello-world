using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    interface BaseRoomInterface
    {
        void Req_Object_Operate_permissions(Int64 userid, string objectname, string uuid);

        void Req_Object_Release_permissions(Int64 userid, string objectname, string uuid);

        void ret_sync_commond(string typ, string commond, Int64 userid, string objectname, string uuid);

        void SyncClient();
    }
}
