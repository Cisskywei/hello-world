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
        void Req_Object_Operate_permissions(string token, string objectname, string uuid);

        void Req_Object_Release_permissions(string token, string objectname, string uuid);

        void ret_sync_commond(string typ, string commond, string name, string objectname, string uuid);

        void ChangeObjectAllOnce(string token, Hashtable clientallonce);

        void SyncClient();

        // 切换操作
        void SwitchModelTo(Enums.ModelEnums tomodel);
    }
}
