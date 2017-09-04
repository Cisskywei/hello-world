using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TinyFrameWork;

/// <summary>
/// 此类里的连接者是RoomConnecter
/// </summary>
public class ClassRoom : MonoBehaviour, msg_req_ret
{
    //   // Use this for initialization
    //   void Start () {

    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            MsgModule.getInstance().reqDivideGroup("kkk");
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Operate_permissions", UserInfor.getInstance().RoomId, 101, "guanshu", "ooo");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Release_permissions", UserInfor.getInstance().RoomId, 101, "guanshu", "ooo");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Operate_permissions", UserInfor.getInstance().RoomId, 102, "guanshu", "ooo");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Release_permissions", UserInfor.getInstance().RoomId, 102, "guanshu", "ooo");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Operate_permissions", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, "guanshu", "ooo");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "Req_Object_Release_permissions", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, "guanshu", "ooo");
        }

        // 切换模式
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //观学
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.WatchLearnModel_Sync, false, null);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //指导人
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.GuidanceMode_Personal, false, "101");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //指导组
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.GuidanceMode_Group, true, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //自主训练组
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.SelfTrain_Group, true, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //自主训练全部
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.SelfTrain_All, false, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //自主训练个人
            MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.SelfTrain_Personal, false, "0"); // 组0
        }
    }

    public void EnterClassRoom(int courseid)
    {
        MsgModule.getInstance().registerMsgHandler(this);

        // 只为测试  获取课程题目数据
        MsgModule.getInstance().reqAcquireQuestionList();

        //      CollectionObject.getInstance().SyncSceneToService(true);   // 在vr 课件打开才需要初始化

        //      MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "BeginClass", UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId); // // 在vr 课件打开才需要

        if (UserInfor.getInstance().isTeacher)
        {
            // 初始化玩家列表
            UiDataManager.getInstance().InitPlayers();

            // 开启ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Prepare);

            //         MsgModule.getInstance().reqSwitchTeachMode(Enums.TeachingMode.WatchLearnModel_Sync, false, null);  // 在vr 课件打开才需要

            // 获取在线玩家
            MsgModule.getInstance().reqOnlinePlayers();

            // 获取课程资料列表 老师端需要显示课程资料列表 学生端 根据 老师推送显示
            MsgModule.getInstance().reqMaterialItemList(courseid);
        }
        else
        {
            // 开启学生ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.StudentUI);
        }

    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        
    }
}
