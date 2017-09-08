using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using common;
using System;

public class NetworkCommunicate : imodule
{

    public static NetworkCommunicate getInstance()
    {
        return Singleton<NetworkCommunicate>.getInstance();
    }

    public static string selfmodelname = "cMsgConnect";

    public void PlayerLogin(string name, string password, Enums.LoginType logintyp)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Login_func, name, password, (Int64)logintyp);
    }

    public void retPlayerLogin(Hashtable msg)
    {
        CommandReceive.getInstance().ReceiveHashMsg(CommandDefine.HashTableMsgType.retLogin, msg);
    }

    public void PlayerEnterLab(string token, int userid, int duty)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Enter_Lab_func, token, (Int64)userid, (Int64)duty);
    }

    public void retPlayerEnterLab(Hashtable msg)
    {
        CommandReceive.getInstance().ReceiveHashMsg(CommandDefine.HashTableMsgType.retEnterLab, msg);
    }

    public void PlayerEnterCourse(int userid, string uuid, int courseid)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Enter_Course_func, (Int64)userid, uuid, (Int64)courseid);
    }

    public void retPlayerEnterCourse(Hashtable msg)
    {
        CommandReceive.getInstance().ReceiveHashMsg(CommandDefine.HashTableMsgType.retEnterCourse, msg);
    }

    public void PlayerExit(int roomid, int userid)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Exit_func, (Int64)roomid, (Int64)userid);
    }

    public void retPlayerExit(Hashtable msg)
    {
        CommandReceive.getInstance().ReceiveHashMsg(CommandDefine.HashTableMsgType.retExit, msg);
    }

    public void ReqCommand(int roomid, int userid, ArrayList msg)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Command_func, (Int64)roomid, (Int64)userid, msg);
    }

    public void RetCommand(Int64 userid, ArrayList msg)
    {
        CommandReceive.getInstance().Receive((int)userid, msg);
    }

    public void ReqChangeClientAllOnce(int roomid, int userid, Hashtable data)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.ChangeAllOnce_func, (Int64)roomid, (Int64)userid, data);
    }

    // 接收服务器的同步数据
    //public void RetChangeClientAllOnce(Hashtable data)
    //{
    //    UnifiedReceive.getInstance().Receive(data);
    //}

    public void RetChangeClientAllOnce(Hashtable objectdata, Hashtable playerdata)
    {
        UnifiedReceive.getInstance().Receive(objectdata, playerdata);
    }

    //public void SyncClient(Hashtable objectdata, Hashtable playerdata)
    //{
    //    Debug.Log("同步数据" + objectdata.Count + " -- " + playerdata.Count);

    //    Hashtable h = new Hashtable();
    //    h.Add("objects", objectdata);
    //    h.Add("player", playerdata);
    //    UnifiedReceive.getInstance().Receive(h);
    //}

    // 管道同步
    public void ReqPipe(int roomid, int fromid, int toid, Hashtable data)
    {
        Client._client.call_hub(NetConfig.lobby_module_name, NetConfig.class_module_name, NetConfig.Pipe_func, (Int64)roomid, (Int64)fromid, (Int64)toid, data);
    }

    public void RetPipe(Int64 userid, Hashtable data)
    {
        ContinuousPipe.getInstance().Receive((int)userid, data);
    }
}
