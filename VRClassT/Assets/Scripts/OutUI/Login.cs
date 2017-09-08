using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class Login : OutUIBase
{
    public InputField nameTxt;
    public InputField passwordTxt;
    public EnterLobby el;

    private void Awake()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            NetworkCommunicate.getInstance().PlayerLogin("zheng", "123", Enums.LoginType.Player);
        }
    }

    private void connectedhub()
    {
        Debug.Log(" connectedhub ");
    }

    private void OnEnable()
    {
        // 注册hashtable msg 监听函数
        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retLogin, ret_msg);
    }

    private void OnDisable()
    {
        CommandReceive.getInstance().RemoveHashMsgListener(CommandDefine.HashTableMsgType.retLogin, ret_msg);
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

        // 登陆
        NetworkCommunicate.getInstance().PlayerLogin(name, password, Enums.LoginType.Player);

        //MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", password, name, "cMsgConnect", "ret_msg");
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
            RoleManager.getInstance().selfplayerid = id;

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