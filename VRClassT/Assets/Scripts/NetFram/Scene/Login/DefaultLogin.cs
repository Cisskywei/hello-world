using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TinyFrameWork;

public class DefaultLogin : MonoBehaviour, msg_req_ret
{

    public EnterLobby el;

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
            name = "henery";  // "cc";// "henery";// "lixin";// "红色诺言";
        }

        if (password == null)
        {
            password = "111111";  // "9";// "111111";// "1";// "123456";
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
            int id = Convert.ToInt32(msg["id"]);
            string duty = (string)msg["duty"]; 

            UserInfor.getInstance().UserToken = token;
            UserInfor.getInstance().UserName = name;
            UserInfor.getInstance().UserUuid = uuid;
            UserInfor.getInstance().UserId = id;
            UserInfor.getInstance().UserDuty = duty;

            if(UserInfor.getInstance().UserDuty == "teacher")
            {
                UserInfor.getInstance().isleader = true;
            }
            //
            if (el != null)
            {
                el.PlayerEnterLobby();
            }
        }
        else if(result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }
    }
}
