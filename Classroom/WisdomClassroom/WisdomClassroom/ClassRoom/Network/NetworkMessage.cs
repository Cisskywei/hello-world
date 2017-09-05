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

        // 登陆相关
        public void PlayerLogin(string name, string password, Int64 playertype)
        {
            Login.getInstance().PlayerLogin(name, password, playertype);
        }

        public void PlayerEnterLab(string token, Int64 userid, Int64 duty)
        {
            Login.getInstance().EnterLobby(token, userid, duty);
        }

        public void PlayerEnterCourse(Int64 userid, string uuid, Int64 courseid)
        {
            Login.getInstance().EnterCourse(userid, uuid, courseid);
        }

        public void PlayerExit(Int64 roomid,Int64 userid)
        {
            Login.getInstance().player_exit(roomid,userid);
        }

        //还未进入房间的退出
        public void PlayerExitByName(string name)
        {
            Login.getInstance().play_exit_name(name);
        }

        // 指令消息接口
        public void Command(Int64 roomid, Int64 userid, ArrayList msg)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.Command(userid, msg);
        }

        public void ChangeClientAllOnce(Int64 roomid, Int64 userid, Hashtable data)
        {
            ClassRoom cr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (cr == null)
            {
                return;
            }

            cr.ChangeClientAllOnce((int)userid, data);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
