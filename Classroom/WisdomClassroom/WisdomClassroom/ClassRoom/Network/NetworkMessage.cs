using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    /// <summary>
    /// 只对当前hub智慧教室服务
    /// </summary>
    class NetworkMessage : imodule
    {
        public static string selfmodelname = "vrClass";

        // 测试
        public void Enter(Int64 roomid, Int64 userid)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                cr = RoomManager.getInstance().CreateRoomById((int)roomid);
            }

            UserInfor user = new UserInfor();
            user.selfid = (int)userid;
            user.uuid = client_uuid;
            user.roomid = (int)roomid;

            cr.Enter(user);
        }

        public void InitScene(Int64 roomid, Hashtable data)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if(cr == null)
            {
                Console.WriteLine("cr == null --- InitScene");
                return;
            }
            Console.WriteLine("cr != null --- InitScene" + cr.selfid);
            cr.InitScene(data);
        }

        public void ChangeModel(Int64 roomid, Int64 model)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.ChangeModel((int)model);
        }

        public void Hold(Int64 roomid, Int64 userid, Int64 ibjectid)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.Hold((int)userid,(int)ibjectid);
        }

        public void Release(Int64 roomid, Int64 userid, Int64 ibjectid)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.Release((int)userid, (int)ibjectid);
        }

        public void Sync(Int64 roomid, Int64 userid, Hashtable data)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.Sync((int)userid, data);
        }
    }
}
