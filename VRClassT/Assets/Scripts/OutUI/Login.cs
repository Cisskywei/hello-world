using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class Login : OutUIBase, msg_req_ret
{
    public InputField nameTxt;
    public InputField passwordTxt;
    public EnterLobby el;

    private void Awake()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
    }

    //// Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //       BackFromVrExe.getInstance().InitSaveData("古风", "123");   // "江燕", "123"  "古风", "123"

            MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", "123", "zheng", "cMsgConnect", "ret_msg");
        }
    }

    private void connectedhub()
    {
        MsgModule.getInstance().registerMsgHandler(this);
    }

    public void LoginClick()
    {
        if(nameTxt == null || passwordTxt == null)
        {
            // 提示输入用户名 密码
            return;
        }

        if (nameTxt.text == string.Empty || passwordTxt.text == string.Empty)
        {
            // 提示输入用户名 密码
            return;
        }

        string name = nameTxt.text;
        string password = passwordTxt.text;

        BackFromVrExe.getInstance().InitSaveData(name, password);

        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", password, name, "cMsgConnect", "ret_msg");
    }

    public void GoToLogin(string name, string password)
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", password, name, "cMsgConnect", "ret_msg");
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];

        DebugZero.Log("登陆结果: " + result);

        if (result == "success")
        {
            string token = (string)msg["token"];
            string name = (string)msg["name"];
            string uuid = (string)msg["uuid"];
            int id = Convert.ToInt32(msg["id"]);
            string duty = (string)msg["duty"];
            string avatar = (string)msg["avatar"];

            UserInfor.getInstance().UserToken = token;
            UserInfor.getInstance().UserName = name;
            UserInfor.getInstance().UserUuid = uuid;
            UserInfor.getInstance().UserId = id;
            UserInfor.getInstance().UserDuty = duty;
            UserInfor.getInstance().avatar = avatar;

            if (UserInfor.getInstance().UserDuty == "teacher")
            {
                UserInfor.getInstance().isleader = true;
            }
            //
            if (el != null)
            {
                el.PlayerEnterLobby();
            }
        }
        else if (result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            DebugZero.Log("错误信息: " + errormsg);
        }
    }
}