using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 智慧教授指令发动相关
/// </summary>
public class CommandSend {

    public static CommandSend getInstance()
    {
        return Singleton<CommandSend>.getInstance();
    }

    public void Send(int roomid, int userid, ArrayList msg)
    {
        NetworkCommunicate.getInstance().ReqCommand(roomid, userid, msg);
    }

    // 课件相关函数
    // 可能并不是一个好方法
    // 只是在这个地方去拼接消息
    // TODO
}
