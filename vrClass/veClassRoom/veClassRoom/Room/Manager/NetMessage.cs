using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class NetMessage : imodule
    {
        public static NetMessage getInstance()
        {
            return Singleton<NetMessage>.getInstance();
        }

        public static string selfmodelname = "NetMessage";

        public NetMessage()
        {
            server.add_Hub_Model(selfmodelname, this);
        }

        // 房间协议
        public void InitScenes(string roomname, Hashtable data)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.InitScenes(data);
        }

        public void BeginClass(string roomname, string token)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.BeginClass(token);
        }

        public void Switch_Model(string roomname, string token, string tomodel, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if(rr == null)
            {
                return;
            }

            rr.Switch_Model(token, tomodel, uuid);
        }

        public void Req_Object_Operate_permissions(string roomname, string token, string objectname, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.Req_Object_Operate_permissions(token, objectname, uuid);
        }

        public void Req_Object_Release_permissions(string roomname, string token, string objectname, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.Req_Object_Release_permissions(token, objectname, uuid);
        }

        public void ChangeObjectAllOnce(string roomname, string token, Hashtable clientallonce)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ChangeObjectAllOnce(token, clientallonce);
        }

        public void ret_sync_commond(string roomname, string typ, string commond, string token, string other, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ret_sync_commond(typ,commond,token,other,uuid);
        }

        public void ret_sync_group_commond(string roomname, string typ, string commond, string token, string other, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ret_sync_group_commond(typ, commond, token, other, uuid);
        }

        public void Change_One_Model(string roomname, string token, string tomodel, string uuid, string onetoken)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.Change_One_Model(token, tomodel, uuid, onetoken);
        }

        public void Change_Some_Model(string roomname, string token, string tomodel, string uuid, ArrayList sometoken)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.Change_Some_Model(token, tomodel, uuid, sometoken);
        }

        public void Divide_Group(string roomname, string token, string rules, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.DivideGroup(token, rules, uuid);
        }

        public void ChooseOneOrGroupOperate(string roomname, string token, string name, bool isgroup = false)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ChooseOneOrGroupOperate(token, name, isgroup);
        }


        // 具体和客户端的界面操作有关的 rpc函数
        //TODO
    }
}
