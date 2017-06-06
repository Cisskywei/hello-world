using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TinyFrameWork;

public class DefaultLogin : MonoBehaviour, msg_req_ret
{
    private void Awake()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    private void connectedhub()
    {
        MsgModule.getInstance().registerMsgHandler(this);
        Login(null, null);
    }

    private void Login(string password, string name)
    {
        if (name == null)
        {
            name = "lixin";
        }

        if (password == null)
        {
            password = "1";
        }

        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", password, name, "cMsgConnect", "ret_msg");
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];

        if (result == "success")
        {
            string token = (string)msg["token"];
            string name = (string)msg["name"];
            string uuid = (string)msg["uuid"];

            UserInfor.getInstance().UserToken = token;
            UserInfor.getInstance().UserName = name;
            UserInfor.getInstance().UserUuid = uuid;

            Debug.Log("登陆返回: " + token);
        }
        else if(result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }
    }
}
