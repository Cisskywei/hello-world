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
        public void InitScenes(Int64 roomid, Hashtable data)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.InitScenes(data);
        }

        // 玩家在场景里初始化自己三维信息
        public void PlayerInitSelf3DInfor(Int64 roomid, Int64 userid, Hashtable infor)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.PlayerInitSelf3DInfor(userid,infor);
        }

        public void BeginClass(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.BeginClass(userid);
        }

        public void Switch_Model(string roomname, string token, string tomodel, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if(rr == null)
            {
                return;
            }

     //       rr.Switch_Model(token, tomodel, uuid);
        }

        public void Req_Object_Operate_permissions(Int64 roomid, Int64 userid, string objectname, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.Req_Object_Operate_permissions(userid, objectname, uuid);
        }

        public void Req_Object_Release_permissions(Int64 roomid, Int64 userid, string objectname, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.Req_Object_Release_permissions(userid, objectname, uuid);
        }

        public void ChangeObjectAllOnce(string roomname, Int64 userid, Hashtable clientallonce)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ChangeObjectAllOnce(userid, clientallonce);
        }

        public void ChangePlayerAllOnce(string roomname, Int64 userid, Hashtable clientallonce)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ChangePlayerAllOnce(userid, clientallonce);
        }

        public void ChangeClientAllOnce(Int64 roomid, Int64 userid, Hashtable clientallonce)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.ChangeClientAllOnce(userid, clientallonce);
        }

        public void ret_sync_commond(string roomname, string typ, string commond, Int64 id, string other, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ret_sync_commond(typ,commond,id,other,uuid);
        }

        public void ret_sync_group_commond(string roomname, string typ, string commond, Int64 id, string other, string uuid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomByName(roomname);

            if (rr == null)
            {
                return;
            }

            rr.ret_sync_group_commond(typ, commond, id, other, uuid);
        }

        public void Change_One_Model(Int64 roomid, Int64 userid, Int64 tomodel, string uuid, Int64 oneid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.Change_One_Model(userid, tomodel, uuid, oneid);
        }

        public void Change_Some_Model(Int64 roomid, Int64 userid, Int64 tomodel, ArrayList someid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.Change_Some_Model(userid, tomodel, someid);
        }

        public void Divide_Group(Int64 roomid, Int64 userid, string rules)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.DivideGroup(userid, rules);
        }

        public void ChooseOneOrGroupOperate(Int64 roomid, Int64 userid, string name, bool isgroup = false)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.ChooseOneOrGroupOperate(userid, name, isgroup);
        }


        // 具体和客户端的界面操作有关的 rpc函数
        //TODO
        // 切换模式
        public void SwitchTeachMode(Int64 roomid, Int64 userid, Int64 mode, Int64 isgroup, string target = null)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.SwitchTeachMode(userid, mode, isgroup,target);
        }
        // 重置场景
        public void ResetScene(Int64 roomid, Int64 userid, Int64 typ, string target)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.ResetScene(userid, typ, target);
        }
        // 获取课程题目数据
        public void AcquireQuestionList(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.AcquireQuestionList(userid);
        }
        // 获取课程资料列表
        public void AcquireMaterialItemList(Int64 roomid, Int64 userid, Int64 courseid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.AcquireMaterialItemList(userid, courseid);
        }
        // 随堂测试
        public void InClassTest(Int64 roomid, Int64 userid, Int64 typ, Int64 questionid, string other = null)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.InClassTest(userid, typ, questionid, other);
        }
        // 随堂测试学生回答
        public void AnswerQuestion(Int64 roomid, Int64 userid, Int64 questionid, Int64 optionid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.AnswerQuestion(userid, questionid, optionid);
        }
        // 随堂测试学生抢答
        public void AnswerFastQuestion(Int64 roomid, Int64 userid, Int64 questionid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.AnswerFastQuestion(userid, questionid);
        }
        // 点赞
        public void SendLikeToTeacher(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.SendLikeToTeacher(userid);
        }
        // 举手
        public void SendDoubtToTeacher(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.SendDoubtToTeacher(userid);
        }
        //老师推送电子白板  0 是关 1是开
        public void SwitchWhiteBoard(Int64 roomid, Int64 userid, Int64 openclose)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.SwitchWhiteBoard(userid, openclose);
        }
        //返回大厅
        public void BackToLobby(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.BackToLobby(userid);
        }
        // 获取在线学生列表
        public void GetOnlinePlayers(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.GetOnlinePlayers(userid);
        }
        // 推送课程资料
        public void PushCourseDataOne(Int64 roomid, Int64 userid, string filename, string fileurl, string filetyp, Int64 fileid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.PushCourseDataOne(userid, filename, fileurl, filetyp, fileid);
        }

        public void PushCourseDataAll(Int64 roomid, Int64 userid, Hashtable files)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.PushCourseDataAll(userid, files);
        }

        public void OpenContent(Int64 roomid, Int64 userid, Int64 fileid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.OpenContent(userid, fileid);
        }

        public void EndFastQuestion(Int64 roomid, Int64 userid, Int64 targetid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.EndFastQuestion(userid, targetid);
        }

        public void VideoCtrl(Int64 roomid, Int64 userid, Int64 type, Int64 value)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.VideoCtrl(userid, type, value);
        }

        public void PPtCtrl(Int64 roomid, Int64 userid, Int64 type, Int64 value)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.PPtCtrl(userid, type, value);
        }

        // 大厅启动vr课件相关
        // 请求进入课件
        public void player_enter_courseware(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.player_enter_courseware(userid);
        }

        // 请求返回大厅
        public void player_back_lobby(Int64 roomid, Int64 userid)
        {
            RealRoom rr = RoomManager.getInstance().FindRoomById(roomid);

            if (rr == null)
            {
                return;
            }

            rr.player_back_lobby(userid);
        }
    }
}
