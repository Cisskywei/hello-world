﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class Login
    {
        public static Login getInstance()
        {
            return Singleton<Login>.getInstance();
        }

        public static Dictionary<int, UserInfor> allplayerlogin = new Dictionary<int, UserInfor>();

        public void player_login(string password, string name)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            Console.WriteLine("login" + name + password);

            // 判断是否重复登陆
            bool isrelogin = false;
            foreach (UserInfor u in allplayerlogin.Values)
            {
                if (u.name == name)
                {
                    Console.WriteLine("用户重复登陆 " + name + password + client_uuid);
                    isrelogin = true;
                    u.uuid = client_uuid;
                    // 重复登陆
                    hub.hub.gates.call_client(client_uuid, "cMsgConnect", "ret_reLogin", client_uuid);
                    break;
                }
            }

            if (!isrelogin)
            {
                BackDataService.getInstance().CheckUser(name, password, this.Login_Succeed, this.Login_Failure, client_uuid);
            }

        }

        public void Login_Succeed(BackDataType.PlayerLoginRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                UserInfor user = new UserInfor();
                user.InitLoginRetData(v, jsondata);
                user.uuid = tag;

                int id = Convert.ToInt32(user.id);
                if (allplayerlogin.ContainsKey(id))
                {
                    allplayerlogin[id] = user;
                }
                else
                {
                    allplayerlogin.Add(id, user);
                }

                player_base_infor(v.data.access_token, tag);
            }
            catch
            {

            }
        }

        public void Login_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_ErrorMsg, errormsg.message);

                hub.hub.gates.call_client(tag, "cMsgConnect", "ret_msg", h);
            }
            catch
            {

            }
        }

        // 用户从大厅进入课件 课件请求进入教室
        public void player_login_courseware(string password, string name)
        {
          //  var client_uuid = hub.hub.gates.current_client_uuid;

            //// 判断是否重复登陆
            //foreach (UserInfor u in allplayerlogin.Values)
            //{
            //    if (u.name == name)
            //    {
            //        u.uuid = client_uuid;
            //        Int64 id = Convert.ToInt64(u.user_id);
            //        if (!StartupExeManager.getInstance().CheckPerson((int)id))
            //        {
            //            break;
            //        }

            //        int roomid = u.roomid;
            //        RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);
            //        if (rr != null)
            //        {
            //            rr.player_login_courseware(id, client_uuid);
            //        }
            //        // 重复登陆
            //        break;
            //    }
            //}
        }

        // 获取用户基本信息
        public void player_base_infor(string token, string uuid)
        {
            BackDataService.getInstance().GetPlayerBaseInfor(token, player_base_infor_Succeed, player_base_infor_Failure, uuid);
        }

        public void player_base_infor_Succeed(BackDataType.PlayerBaseInforRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(v.data.user_id);
                if (!allplayerlogin.ContainsKey(id))
                {
                    return;
                }

                UserInfor user = null;
                allplayerlogin[id].InitBaseInforRetData(v, jsondata);
                user = allplayerlogin[id];

                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Token, user.access_token);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Name, user.name);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Uuid, user.uuid);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Id, user.id);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Duty, user.identity);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Avatar, user.avatar);

                hub.hub.gates.call_client(tag, "cMsgConnect", "ret_msg", h);
            }
            catch
            { }
        }

        public void player_base_infor_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_ErrorMsg, errormsg.message);

                hub.hub.gates.call_client(tag, "cMsgConnect", "ret_msg", h);
            }
            catch
            {

            }
        }

        public void EnterLobby(string token, Int64 userid, Int64 duty)
        {
            switch (duty)
            {
                case 1:
                    // 获取学生课程
                    BackDataService.getInstance().GetStudentCourseList(token, Enter_Lobby_Succeed, Enter_Lobby_Failure, userid.ToString());
                    break;
                case 2:
                    // 获取老师课程
                    BackDataService.getInstance().GetTeacherCourseList(token, "expe", Enter_Lobby_Succeed, Enter_Lobby_Failure, userid.ToString());
                    break;
                default:
                    // 获取学生课程
                    BackDataService.getInstance().GetStudentCourseList(token, Enter_Lobby_Succeed, Enter_Lobby_Failure, userid.ToString());
                    break;
            }
        }

        public void Enter_Lobby_Succeed(BackDataType.CourseListRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allplayerlogin.ContainsKey(id))
                {
                    return;
                }

                allplayerlogin[id].InitCourseListRetData(v, jsondata);

                // 初始化 服务器端的 课程列表数据
                //TODO

                // 转换编码格式
                if (jsondata != null)
                {
                    jsondata = JsonDataHelp.getInstance().EncodeBase64(null, jsondata);
                }

                Hashtable h = new Hashtable(); //BackDataType.CourseListRetData_Serialize(v);  // 直接返回json数据
                if (h != null)
                {
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");

                    if (jsondata != null)
                    {
                        h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_JsonData, jsondata);
                    }

                    hub.hub.gates.call_client(allplayerlogin[id].uuid, "cMsgConnect", "ret_msg", h);
                }

            }
            catch
            {

            }
        }

        public void Enter_Lobby_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allplayerlogin.ContainsKey(id))
                {
                    return;
                }

                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_ErrorMsg, errormsg.message);

                hub.hub.gates.call_client(allplayerlogin[id].uuid, "cMsgConnect", "ret_msg", h);
            }
            catch
            {

            }
        }

        // 进入课程 创建房间 老师信息 学生信息等
        public void EnterCourse(Int64 userid, string uuid, Int64 courseid)
        {
            ClassRoom rr = RoomManager.getInstance().CreateRoomById((int)courseid);

            if (rr == null)
            {
                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_ErrorMsg, "nothing");

                hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", h);
            }
            else
            {
                if (!allplayerlogin.ContainsKey((int)userid))
                {
                    return;
                }

                UserInfor user = allplayerlogin[(int)userid];
                int ret = rr.Enter(user);
                if (ret == -1)
                {
                    // 重复进入房间
                    return;
                }

                user.roomid = ret;

                // 如果是老师 要返回给老师 当前课程的学生列表信息
                if (user.identity == "teacher")
                {
                    // 老师 返回学生列表数据
                    BackDataService.getInstance().GetCourseStudentList(user.access_token, courseid.ToString(), Student_List_Succeed, Student_List_Failure, userid.ToString());
                }
                else
                {
                    // 登陆列表移除
                    if (allplayerlogin.ContainsKey((int)userid))
                    {
                        allplayerlogin.Remove((int)userid);
                    }

                    // 学生这直接返回
                    Hashtable h = new Hashtable();
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Class_id, rr.selfid.ToString());
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Teacher_model, rr._modeltype.ToString());
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Connector, NetworkMessage.selfmodelname);

                    hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", h);
                }
            }
        }

        public void Student_List_Succeed(BackDataType.CourseInforRetData v, string jsondata, string tag, string url)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allplayerlogin.ContainsKey(id))
                {
                    return;
                }

                UserInfor user = allplayerlogin[id];

                // 初始化服务器端 课程详细信息数据
                int roomid = user.roomid;
                ClassRoom rr = RoomManager.getInstance().FindRoomById(roomid);
                if (rr != null)
                {
         //           rr.classinfor.InitAllStudents(v, jsondata);
                }

                // 转换编码格式
                jsondata = JsonDataHelp.getInstance().EncodeBase64(null, jsondata);

                Hashtable h = new Hashtable();// BackDataType.CourseInforRetData_Serialize(v);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "success");
                if (url != null)
                {
                    h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_RootUrl, url);
                }
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Class_id, v.data.course_id);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Connector, NetworkMessage.selfmodelname);
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_JsonData, jsondata);

                // 登陆列表移除
                if (allplayerlogin.ContainsKey(id))
                {
                    allplayerlogin.Remove(id);
                }

                hub.hub.gates.call_client(user.uuid, "cMsgConnect", "ret_msg", h);
            }
            catch
            {

            }
        }

        public void Student_List_Failure(BackDataType.MessageRetHead errormsg, string tag)
        {
            if (tag == null)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(tag);
                if (!allplayerlogin.ContainsKey(id))
                {
                    return;
                }

                UserInfor user = allplayerlogin[id];

                Hashtable h = new Hashtable();
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_Result, "failed");
                h.Add(ConstantsDefine.HashTableKeyEnum.Net_Ret_ErrorMsg, errormsg.message);

                hub.hub.gates.call_client(user.uuid, "cMsgConnect", "ret_msg", h);
            }
            catch
            {

            }
        }

        public void player_exit(Int64 userid, Int64 roomid)
        {
            Console.WriteLine("用户退出 : " + " uuid:");

            //int id = (int)userid;
            //if (StartupExeManager.getInstance().CheckPerson((int)id))
            //{
            //    // 启动课件的退出 不移除玩家 和 房间
            //    Console.WriteLine("启动课件的退出 不移除玩家 和 房间" + userid);
            //    //           return;
            //}

            // 退出场景
            ClassRoom rr = RoomManager.getInstance().FindRoomById((int)roomid);

            if (rr != null)
            {
                rr.Leave((int)userid);
            }

            //向服务器发送用户的个人数据
            // TODO

            // 登陆列表移除
            if (allplayerlogin.ContainsKey((int)userid))
            {
                allplayerlogin.Remove((int)userid);
            }
        }
    }
}