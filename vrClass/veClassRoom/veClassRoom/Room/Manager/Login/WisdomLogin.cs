using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class WisdomLogin : imodule
    {
        public static string selfmodelname = "WisdomLogin";

        public void player_login(string token, string name, string modelname, string callbackname)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            if (modelname == null)
            {
                modelname = "cMsgConnect";
            }

            if (callbackname == null)
            {
                callbackname = "ret_msg";
            }

            Console.WriteLine("用户登陆 : " + " name:" + name + " token:" + token + " uuid:" + client_uuid);

            //向服务器验证获取信息列表  基础的是学生的课程列表!!!
            UserInfor playerinfor = BackDataService.getInstance().CheckUser(token, name);

            //只为测试
            bool isleader = false;
            if (name.Contains("teacher"))
            {
                isleader = true;
            }

            Hashtable h = new Hashtable();
            if (playerinfor == null)
            {
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Token, token);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Name, name);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Uuid, client_uuid);
            }
            else
            {
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Token, token);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Name, name);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Uuid, client_uuid);
                // 学生的课程列表
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Course_List, playerinfor.GetCourseList());

                // 只为测试
                h.Add("isteacher", isleader);
            }

            hub.hub.gates.call_client(client_uuid, modelname, callbackname, h);
        }

        public void player_exit(string token, string name, string uuid, string roomname = null, string modelname = null, string callbackname = null)
        {
            if (modelname == null)
            {
                modelname = "cMsgConnect";
            }

            if (callbackname == null)
            {
                callbackname = "ret_msg";
            }

            Console.WriteLine("用户退出 : " + "name:" + name + " token:" + token + " uuid:" + uuid);

            if (roomname != null)
            {
                // 退出场景
                RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

                if(rr!=null)
                {
                    rr.PlayerLeaveScene(token, name, uuid);
                }
            }

            //向服务器发送用户的个人数据
            // TODO
        }

        // 玩家进入房间
        public void player_enter_room(string token, string name, string uuid, string roomname, string modelname, string callbackname)
        {
            Console.WriteLine("用户请求进入房间 : " + "name:" + name + " token:" + token + " uuid:" + uuid + " roomname:" + roomname);

            RealRoom rr = RoomManager.getInstance().CreateRoomByName(roomname);

            Hashtable h = new Hashtable();

            if (rr == null)
            {
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
            }
            else
            {
                rr.PlayerEnterScene(token, name, uuid);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Connector, NetMessage.selfmodelname);
            }

            if (modelname == null)
            {
                modelname = "cMsgConnect";
            }

            if (callbackname == null)
            {
                callbackname = "ret_msg";
            }

            hub.hub.gates.call_client(uuid, modelname, callbackname, h);

        }
    }
}
