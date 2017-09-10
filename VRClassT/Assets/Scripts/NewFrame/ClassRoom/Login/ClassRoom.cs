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
    }

    private void RemoveListener()
    {
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.QuestionList, QuestionList);
    }

    public void EnterClassRoom(int courseid)
    {
        // 只为测试  获取课程题目数据
        ArrayList q = new ArrayList();
        q.Add((Int64)CommandDefine.FirstLayer.Lobby);
        q.Add((Int64)CommandDefine.SecondLayer.QuestionList);
        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, q);

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlinePlayers, OnlinePlayers);
        // 玩家上线 通知老师
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OnlineOnePlayer, OnlineOnePlayer);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PlayerList, PlayerList);

        if (UserInfor.getInstance().isTeacher)
        {
            CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.MaterialList, MaterialList);

            // 初始化玩家列表
            UiDataManager.getInstance().InitPlayers();

            //// 获取在线玩家
            //ArrayList p = new ArrayList();
            //p.Add((Int64)CommandDefine.FirstLayer.Lobby);
            //p.Add((Int64)CommandDefine.SecondLayer.OnlinePlayers);
            //CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, p);

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
        ClassManager.getInstance().Playersonline(userids);
    }

    private void OnlineOnePlayer(int userid, ArrayList msg)
    {
        ArrayList userids = (ArrayList)msg[2];
        ClassManager.getInstance().Playeronline(userids);
    }

    // 获取玩家在线列表 及班级课程信息
    private void PlayerList(int userid, ArrayList msg)
    {
        ArrayList data = (ArrayList)msg[2];
        // onlinplayers
        ArrayList online = null;
        string jsondata = null;
        if(data.Count > 1)
        {
            online = (ArrayList)data[0];
            jsondata = (string)data[1];
        }
        else
        {
            online = (ArrayList)data[0];
        }

        if(jsondata != null)
        {
            jsondata = JsonDataHelp.getInstance().DecodeBase64(null, jsondata);

            Debug.Log(" -- PlayerList ------------- " + jsondata);

            DataType.CourseInforRetData course = JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseInforRetData>(jsondata);

            if (course != null)
            {
                // 获取了 学生列表 所以在这个地方进行学生列表初始化
                ClassManager.getInstance().InitClass(course.data);
            }
        }

        ClassManager.getInstance().InitOnlinePlayer(online);
    }
}
