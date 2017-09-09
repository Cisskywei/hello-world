using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleOrder : MonoBehaviour, NetPlayerInterFace.IPlayerOrder
{
    public enum RoleOrderType
    {
        None = -1,

        // 手柄操作指令
        Palm,
        Power,
        PointTo,
        HideSkin,

        // 瞬移
        Teleport,
    }

    public enum LeftRight
    {
        Left = 0,
        Right,
    }

    public HandCtrl[] handsctrl;
    public SyncPlayer sp;

    // msg : 一级指令 二级指令 消息体: 左右手/RoleOrderType/具体值
    public void PlayerOrder(int userid, ArrayList msg)
    {
        if(msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 leftright = (Int64)msg[2];

        Int64 typ = (Int64)msg[3];
        RoleOrderType ordertype = (RoleOrderType)typ;

        if(handsctrl == null || handsctrl.Length <= leftright || leftright < 0)
        {
            return;
        }

        HandCtrl hc = handsctrl[leftright];

        switch (ordertype)
        {
            case RoleOrderType.Palm:
                hc.Palm();
                break;
            case RoleOrderType.Power:
                hc.Power();
                break;
            case RoleOrderType.PointTo:
                hc.PointTo();
                break;
            case RoleOrderType.HideSkin:
                if (msg.Count >= 4)
                {
                    Int64 v = (Int64)msg[3];
                    hc.ShowHideSkin(v == 1);  // 0 隐藏手 1 显示手
                }
                break;
            case RoleOrderType.Teleport:
                // 瞬移
                sp.TeleportManHead(msg.GetRange(4, msg.Count-4));
                break;
            default:
                break;
        }
    }

    // 发送瞬移数据
    public void SendTeleport(Vector3 pos)
    {
        Hashtable msg = new Hashtable();
        msg.Add("rx", pos.x);
        msg.Add("ry", pos.y);
        msg.Add("rz", pos.z);

        ArrayList a = new ArrayList();
        a.Add((Int64)CommandDefine.FirstLayer.Lobby);
        a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
        a.Add((Int64)4);
        a.Add((Int64)RoleOrderType.Teleport);
        a.Add(msg);
        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
    }
}
