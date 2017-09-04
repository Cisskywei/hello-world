using common;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ko.NetFram
{
    public class MsgModule : imodule
    {

        public static MsgModule getInstance()
        {
            return Singleton<MsgModule>.getInstance();
        }

        private msg_req_ret _handler;
        private msg_req_ret_json _handler_json;
        private sync_req_ret _synchamdler;
        private sync_commond _synccommond;
        private sync_string_message _listenmsg;

        public void registerSyncCommond(sync_commond synccommond)
        {
            _synccommond = synccommond;
        }

        public void registerMsgHandler(msg_req_ret msghandler)
        {
            _handler = msghandler;
        }

        public void registerJsonMsgHandler(msg_req_ret_json msghandler)
        {
            _handler_json = msghandler;
        }

        public void registerSyncMsgHandler(sync_req_ret syncmsghandler)
        {
            _synchamdler = syncmsghandler;
        }

        public void registerListenMsgHandler(sync_string_message listenmsghandler)
        {
            _listenmsg = listenmsghandler;
        }

        // 向服务器发送消息
        public void req_msg(Hashtable msg, string contactmodule = null, string contactfunc = null)
        {
            // sMsgConnect ret_msg 默认
            if (contactmodule == null)
            {
                contactmodule = "sMsgConnect";
            }

            if (contactfunc == null)
            {
                contactfunc = "ret_msg";
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomId, msg);
        }

        // 接收服务器返回
        public void ret_msg(Hashtable msg)
        {
            if (_handler != null)
            {
                _handler.ret_msg(msg);
            }

        }

        // 用户重复登陆
        public void ret_reLogin(string uuid)
        {
            Debug.Log(UserInfor.getInstance().UserUuid + "用户重复登录" + uuid);
            UserInfor.getInstance().UserUuid = uuid;
        }

        // 接收服务器json数据返回
        public void ret_msg_json(string msg)
        {
            if (_handler_json != null)
            {
                _handler_json.ret_msg_json(msg);
            }

        }

        // 指令同步
        public void req_sync_commond(string typ, string commond, string token, string other, string uuid, string contactmodule = null, string contactfunc = null)
        {
            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().Connector;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "ret_sync_commond";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomName, typ, commond, token, other, uuid);
        }

        //指令同步
        public void ret_sync_commond(string typ, string commond, string token, string other)
        {
            Debug.Log("收到同步指令" + typ + " commond " + commond);
            if (_synccommond != null)
            {
                _synccommond.ret_sync_commond(typ, commond, token, other);
            }
        }

        // 接收服务器string消息
        public void ListenerServerMsg(string msg)
        {
            Debug.Log("服务器通知消息" + msg);

            if(_listenmsg != null)
            {
                _listenmsg.listen_string_msg(msg);
            }
        }

        // 同步服务器场景数据
        public void SyncClient(Hashtable msgObject, Hashtable msgPlayer)
        {
            Debug.Log("客户端收到服务器同步物体玩家信息" + " msgObject " + msgObject.Count + "msgPlayer " + msgPlayer.Count);

            ReciveObjectOnce.getInstance().ReciveSyncObject(msgObject, msgPlayer);
        }

        // 同步服务器场景信息和指令等
        public void SyncClientAndCommond(Hashtable msgObject, Hashtable msgPlayer, Hashtable msgCommond)
        {
            Debug.Log("收到场景同步指令 包括 数据 " + msgObject.Count + " 和 玩家 " + msgPlayer.Count + "和 指令 " + msgCommond.Count);
        }

        // 一次性同步此客户端所有改变数据
        public void req_all_change_once(Hashtable msg, string contactmodule = null, string contactfunc = null)
        {
            if(contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "ChangeClientAllOnce";
            }

            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, msg);

        }

        public void req_player_change_once(Hashtable msg, string token = null, string contactmodule = null, string contactfunc = null)
        {
            if (token == null)
            {
                token = UserInfor.getInstance().UserToken;
            }

            if (token == null)
            {
                return;
            }

            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "ChangePlayerAllOnce";
            }

            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomName, token, msg);

        }

        // 操作物体锁请求
        public void req_operation_permission(Int64 userid, string objectname, string uuid, string contactmodule = null, string contactfunc = null)
        {
            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "Req_Object_Operate_permissions";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            if (uuid == null)
            {
                return;
            }

            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomId, userid, objectname, uuid);
        }

        public void req_operation_release(Int64 userid, string objectname, string uuid, string contactmodule = null, string contactfunc = null)
        {
            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "Req_Object_Release_permissions";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            if (uuid == null)
            {
                return;
            }

            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomId, userid, objectname, uuid);
        }

        public void ret_operation_permissions(Int64 userid, string objectname, string typ, string result)
        {
            Debug.Log(userid + "请求操作返回" + objectname);

            if(objectname == null || result == null)
            {
                return;
            }

            SceneObject go = CollectionObject.getInstance().getSceneObjectByKey(objectname);
            if (go == null)
            {
                Debug.Log("该物体不存在 " + objectname);
            }

            go.listen_operate_result(userid, typ, result);
        }

        // 模式相关 ----------------------------   模式相关
        public void req_switch_model(Int64 userid, string tomodel, string uuid, string contactmodule = null, string contactfunc = null)
        {
            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "Switch_Model";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomName, userid, tomodel, uuid);
        }

        // 改变某一学生的模式状态 
        public void req_change_one_model(string token, string tomodel, string uuid, string onetoken, string contactmodule = null, string contactfunc = null)
        {
            if (token == null)
            {
                token = UserInfor.getInstance().UserToken;
            }

            if (token == null)
            {
                return;
            }

            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "Change_One_Model";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            if(onetoken == null)
            {
                return;
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomName, token, tomodel, uuid, onetoken);
        }

        // 改变某些学生的模式状态 
        public void req_change_some_model(string token, string tomodel, string uuid, ArrayList sometoken, string contactmodule = null, string contactfunc = null)
        {
            if (token == null)
            {
                token = UserInfor.getInstance().UserToken;
            }

            if (token == null)
            {
                return;
            }

            if (contactmodule == null)
            {
                contactmodule = UserInfor.getInstance().RoomConnecter;
            }

            if (contactmodule == null)
            {
                return;
            }

            if (contactfunc == null)
            {
                contactfunc = "Change_Some_Model";
            }

            if (uuid == null)
            {
                uuid = UserInfor.getInstance().UserUuid;
            }

            if(sometoken == null || sometoken.Count <= 0)
            {
                return;
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", contactmodule, contactfunc, UserInfor.getInstance().RoomName, token, tomodel, uuid, sometoken);
        }

        // 登陆获取学生列表信息

        // 具体的 和ui 相关的 向服务器发送消息的接口
        // 模式切换
        public void reqSwitchTeachMode(Enums.TeachingMode mode, bool isgroup, string target)
        {
            if(target == null)
            {
                target = "all";
            }
            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "SwitchTeachMode", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)mode, isgroup ?1:0, target);
        }

        public void retSwitchTeachMode(Int64 userid, Int64 mode, string target)
        {
            // 更新 客户端状态信息
            // TODO
            Debug.Log(target + "切换模式 返回" + mode);
            
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64, Int64, string>(EventId.SwitchModeFeedBack, userid, mode,target);

            UserInfor.getInstance().ChangePlayerModel(userid, (Enums.TeachingMode)mode, target);
        }
        // 分组
        public void reqDivideGroup(string rules)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Divide_Group", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, rules);
        }
        public void retDivideGroup(Int64 userid, Hashtable group)
        {
            Debug.Log("分组返回");
            UiDataManager.getInstance().DivideGroupBack(group);
        }
        //重置场景
        public void reqResetScene(Enums.ResetSceneType typ, string target)
        {
            if (target == null)
            {
                target = "all";
            }
            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "ResetScene", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)typ, target);
        }

        public void retResetScene(Int64 userid)
        {
            Debug.Log(userid + " 重置场景返回");
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.ResetScene, userid);
        }

        // 获取题目数据
        public void reqAcquireQuestionList()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "AcquireQuestionList", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        public void retAcquireQuestionList(Int64 userid, string questiondata)
        {
            if(questiondata != "null")
            {
                QuestionManager.getInstance().InitQuestionList(questiondata);
            }
        }

        // 获取课程资料列表
        public void reqMaterialItemList(int courseid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "AcquireMaterialItemList", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)courseid);
        }

        public void retMaterialItemList(Int64 userid, string materialdata)
        {
            if(materialdata != null)
            {
                DownLoadDataManager.getInstance().InitMaterialList(materialdata);
            }
        }

        // 随堂测试
        public void reqInClassTest(Enums.InClassTestType typ, Int64 question, string other = null)
        {
            if(other == null)
            {
                other = "n";
            }

            // 调用 hub 层
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "InClassTest", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)typ, question, other);
        }

        public void retInClassTest(Int64 userid, Int64 typ, Int64 question, string other = null)
        {
            // 学生端要做相应的处理
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int,int,int,string>(EventId.StudentReciveQuestion, (int)userid, (int)typ, (int)question, other);
        }

        // 学生回答题目 包括选择 判断 简答
        public void reqAnswerQuestion(int questionid, int optionid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "AnswerQuestion", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)questionid, (Int64)optionid);
        }

        public void retAnswerQuestion(Int64 userid, Int64 questionid, Int64 optionid)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int, int, int>(EventId.TestFeedBack, (int)userid, (int)questionid, (int)optionid);
        }

        // 学生回答抢答题目
        public void reqAnswerFastQuestion(int questionid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "AnswerFastQuestion", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)questionid);
        }

        public void retAnswerFastQuestion(Int64 userid, Int64 questionid, string teachername)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int, int, string>(EventId.StudentFastQuestion, (int)userid, (int)questionid, teachername);
        }

        public void reqEndFastQuestion(Int64 userid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "EndFastQuestion", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, userid);
        }

        public void retEndFastQuestion(Int64 userid)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.EndFastQuestion, (int)userid);
        }

        public void reqOnlinePlayers()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "GetOnlinePlayers", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        public void retOnlinePlayers(ArrayList userids)
        {
            UiDataManager.getInstance().ChangePlayerOnline(userids);
        }

        public void retOnlinePlayer(Int64 userid)
        {
            UiDataManager.getInstance().ChangePlayerOnline((int)userid);
        }

        // 学生点赞 和 举手
        public void reqSendLike()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "SendLikeToTeacher", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        public void reqSendDoubt()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "SendDoubtToTeacher", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        // 点赞和举手 只有老师能收得到
        public void retSendLike(Int64 userid)
        {
            Debug.Log("点赞返回" + userid);
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.LikeFeedBack, userid);
        }

        public void retSendDoubt(Int64 userid)
        {
            Debug.Log("疑问返回" + userid);
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.DoubtFeedBack, userid);
        }

        // 老师推送电子白板  0 是关 1是开
        public void reqWhiteBoard(int openclose)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "SwitchWhiteBoard", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)openclose);
        }

        public void retWhiteBoard(Int64 userid, Int64 openclose)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64,int>(EventId.SwitchWhiteBoard, userid, (int)openclose);
        }

        // 返回大厅
        public void reqBackLobby()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "BackToLobby", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        public void retBackToLobby(Int64 userid)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.BackToLobby, userid);
        }

        // 下载资料
        public void reqDownLoadFileOne(string filename, string fileurl, string filetyp, int fileid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "PushCourseDataOne", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, filename, fileurl, filetyp, (Int64)fileid);
        }

        public void retDownLoadFileOne(Int64 userid, string filename, string fileurl, string filetyp, Int64 fileid)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string, string, string, int>(EventId.DownLoadFileOne, filename, fileurl, filetyp, (int)fileid);
        }

        public void reqDownLoadFileAll(Hashtable files)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "PushCourseDataAll", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, files);
        }

        public void retDownLoadFileAll(Int64 userid, Hashtable files)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Hashtable>(EventId.DownLoadFileAll, files);
        }

        public void reqOpenContent(int fileid)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "OpenContent", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)fileid);
        }

        public void retOpenContent(Int64 userid, Int64 fileid)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.OpenContentStudent, (int)fileid);
        }

        // 视频控制相关 typ 1 开关 2 进度 3 音量 4 亮度 5 全景普通切换 value 开关 1 开 0 关 进度*100 全景普通切换 1 切到全景 2 切到普通
        public void reqCtrlVideo(int typ, int value)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "VideoCtrl", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)typ, (Int64)value);
        }

        public void retCtrlVideo(Int64 typ, Int64 value)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int, int>(EventId.VideoCtrl, (int)typ, (int)value);
        }

        // ppt 控制相关 typ 1 ppt 翻页控制 value 翻页页数
        public void reqPPtCtrl(int typ, int value)
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "PPtCtrl", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, (Int64)typ, (Int64)value);
        }

        public void retPPtCtrl(Int64 typ, Int64 value)
        {
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int, int>(EventId.PPTCtrl, (int)typ, (int)value);
        }

        ////提供一个接口 用于ui指令操作
        //public void req_ui_commond()
        //{

        //}

        // 用户从大厅启动vr课件

        // 用户进入课件之后请求服务器
        public void playlogincoursewave(string name, string password)
        {
            MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login_courseware", password, name);
        }

        public void retPlayerLoginCourseware(Hashtable h)
        {
            // TODO
            BackFromVrExe.getInstance().ret_msg(h);
        }

        public void reqPlayerEnterCourseware()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "player_enter_courseware", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        // ret = 1 返回成功
        public void retPlayerEnterCourseware(Int64 ret)
        {
            if(ret == 1)
            {
                EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.PlayerEnterCourseware, (int)ret);
            }
        }

        public void reqPlayerBackLobby()
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "player_back_lobby", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId);
        }

        // ret = 1 返回成功
        public void retPlayerBackLobby(Int64 ret)
        {
            if (ret == 1)
            {
                EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.PlayerBackLobby, (int)ret);
            }
        }
    }
}

