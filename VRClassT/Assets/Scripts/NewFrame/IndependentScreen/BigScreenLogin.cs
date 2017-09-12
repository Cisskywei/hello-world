using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigScreenLogin : MonoBehaviour {

    public EnterLobby el;

    //private void Awake()
    //{
    //    EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
    //}

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B))
        {
            NetworkCommunicate.getInstance().PlayerLogin("-1", "-1", Enums.LoginType.Screen);
        }
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
            Int64 duty = (Int64)msg["duty"];
            string rooturl = (string)msg["rooturl"];

            UserInfor.getInstance().UserToken = token;
            UserInfor.getInstance().UserName = name;
            UserInfor.getInstance().UserUuid = uuid;
            UserInfor.getInstance().UserId = id;
            UserInfor.getInstance().UserDuty = (ComonEnums.DutyEnum)duty;
            RoleManager.getInstance().selfplayerid = id;

            if (rooturl != null)
            {
                ClassDataManager.getInstance().courseinfor_rooturl = rooturl;
            }

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
