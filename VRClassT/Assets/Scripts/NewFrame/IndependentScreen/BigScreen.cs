using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大屏显示 负责接受指令 解析 然后调整大屏显示摄像机位置  以及发送大屏可操作指令
/// </summary>
public class BigScreen {

    public static BigScreen getInstance()
    {
        return Singleton<BigScreen>.getInstance();
    }

    public ScreenCameraCtrl screenctrl;  // 屏幕摄像机控制器

    public void Init(ScreenCameraCtrl screenctrl)
    {
        this.screenctrl = screenctrl;
    }

    public void Clear()
    {
        screenctrl = null;
    }

    public void AddListener()
    {
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.BigScreen, Receiver);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.BigScreen, Receiver);
    }

    public void RemoveListener()
    {
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.BigScreen, Receiver);
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.BigScreen, Receiver);
    }

    // 大屏显示指令分发
    private void Receiver(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 orderid = (Int64)msg[2];
        ScreenOrder order = (ScreenOrder)orderid;

        switch (order)
        {
            case ScreenOrder.WhiteBoard:
            case ScreenOrder.VideoView:
            case ScreenOrder.ImageView:
            case ScreenOrder.PPTView:
            case ScreenOrder.PushShow:
                if (screenctrl != null)
                {
                    screenctrl.ChangePlace((int)orderid);
                }
                break;

            case ScreenOrder.TeacherView:
            case ScreenOrder.StudentView:
                Int64 targetid = (Int64)msg[3];
                if (screenctrl != null)
                {
                    screenctrl.ChangeView((int)targetid);
                }
                break;

            case ScreenOrder.TeamView:
                break;

            case ScreenOrder.ScreenPos:
                break;
            default:
                break;
        }
    }

    // 大屏显示发送指令
    private ArrayList sendorder = new ArrayList();
    public void MoveBigScreen(ScreenOrder moveto)
    {
        if(sendorder == null)
        {
            sendorder = new ArrayList();
            sendorder.Add((Int64)CommandDefine.FirstLayer.Lobby);
            sendorder.Add((Int64)CommandDefine.SecondLayer.BigScreen);
            sendorder.Add((Int64)ScreenOrder.None);
        }

        sendorder[2] = (Int64)moveto;

        CommandSend.getInstance().Send(UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, sendorder);
    }

    // 大屏显示接受指令

    // 大屏显示 指令相关 摄像头位置

    // 辅助私有函数


    // 独立显示摄像机指令
    public enum ScreenOrder
    {
        None = -1,

        TeacherView,
        StudentView,
        TeamView,
        PPTView,
        WhiteBoard,
        VideoView,
        ImageView,
        PushShow,

        ScreenPos, // 万能指令 指令含有位置角度信息
    }
}
