using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TinyFrameWork;

/// <summary>
/// 此类里的连接者是RoomConnecter
/// </summary>
public class ClassRoom
{
    public static ClassRoom getInstance()
    {
        return Singleton<ClassRoom>.getInstance();
    }

    public ClassRoom()
    {
        RegListener();
    }

    // 注册监听函数
    private void RegListener()
    {
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.QuestionList, QuestionList);
        //CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.MaterialList, MaterialList);
        //CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlinePlayers, OnlinePlayers);
    }

    private void RemoveListener()
    {
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.QuestionList, QuestionList);
        //CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.MaterialList, MaterialList);
        //CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlinePlayers, OnlinePlayers);
    }

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
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.WatchLearnModel_Sync, false, null);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //指导人
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.GuidanceMode_Personal, false, "101");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //指导组
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.GuidanceMode_Group, true, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //自主训练组
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.SelfTrain_Group, true, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //自主训练全部
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.SelfTrain_All, false, "0"); // 组0
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //自主训练个人
            MsgModule.getInstance().reqSwitchTeachMode(ComonEnums.TeachingMode.SelfTrain_Personal, false, "0"); // 组0
        }
    }

    public void EnterClassRoom(int courseid)
    {
        // 只为测试  获取课程题目数据
        ArrayList q = new ArrayList();
        q.Add((Int64)CommandDefine.FirstLayer.Lobby);
        q.Add((Int64)CommandDefine.SecondLayer.QuestionList);
        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, q);

        if (UserInfor.getInstance().isTeacher)
        {
            CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.MaterialList, MaterialList);
            CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlinePlayers, OnlinePlayers);
            // 玩家上线 通知老师
            CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlineOnePlayer, OnlineOnePlayer);

            // 初始化玩家列表
            UiDataManager.getInstance().InitPlayers();

            // 获取在线玩家
            ArrayList p = new ArrayList();
            p.Add((Int64)CommandDefine.FirstLayer.Lobby);
            p.Add((Int64)CommandDefine.SecondLayer.OnlinePlayers);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, p);

            // 获取课程资料列表 老师端需要显示课程资料列表 学生端 根据 老师推送显示
            ArrayList m = new ArrayList();
            m.Add((Int64)CommandDefine.FirstLayer.Lobby);
            m.Add((Int64)CommandDefine.SecondLayer.MaterialList);
            m.Add((Int64)courseid);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, m);

            // 开启ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Prepare);
        }
        else
        {
            // 开启学生ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.StudentUI);
        }

    }

    private void QuestionList(int userid, ArrayList msg)
    {
        string questiondata = (string)msg[2];
        QuestionManager.getInstance().InitQuestionList(questiondata);
    }

    private void MaterialList(int userid, ArrayList msg)
    {
        string materialdata = (string)msg[2];
        DownLoadDataManager.getInstance().InitMaterialList(materialdata);
    }

    private void OnlinePlayers(int userid, ArrayList msg)
    {
        ArrayList userids = (ArrayList)msg[2];
        UiDataManager.getInstance().ChangePlayerOnline(userids);
    }

    private void OnlineOnePlayer(int userid, ArrayList msg)
    {
        Int64 id = (Int64)msg[2];
        UiDataManager.getInstance().ChangePlayerOnline((int)id);
    }
}
