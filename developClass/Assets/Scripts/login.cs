using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using System;

public class login : MonoBehaviour , msg_req_ret
{
    public bool isstudent = false;
    public choseCourse er;

    private void Awake()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.ConnectedHub, this.connectedhub);
    }

    private void connectedhub()
    {
        MsgModule.getInstance().registerMsgHandler(this);

        string name = "teacher";
        string token = SystemInfo.deviceUniqueIdentifier;

        if(isstudent)
        {
            name = "pp";
            token = token + "px";
        }

        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_login", token, name, "cMsgConnect", "ret_msg");
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
            string modelname = (string)msg["connector"];
            

            bool isleader = (bool)msg["isteacher"];

            UserInfor.getInstance().TeacherName = modelname;
            UserInfor.getInstance().Connector = modelname;
            UserInfor.getInstance().UserToken = token;
            UserInfor.getInstance().UserName = name;
            UserInfor.getInstance().UserUuid = uuid;

            // 获取学生列表
            if (isleader)
            {
                Hashtable playerlist = msg["playerlist"] as Hashtable;
                UiDataManager.getInstance().InitGroupPlayer(playerlist);
            }

            // 只为测试
            QuestionManager.getInstance().initQuestionInfor();

            ArrayList courselist = msg["courselist"] as ArrayList;

            UserInfor.getInstance().isleader = isleader;
            UserInfor.getInstance().isTeacher = isleader;

            // 进入房间
            if (er != null)
            {
                er.enterCourse((string)courselist[0]);
            }
        }
        else
        {
            Debug.Log("登陆失败");
        }
    }
}
