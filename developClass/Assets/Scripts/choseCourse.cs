using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class choseCourse : MonoBehaviour , msg_req_ret
{
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.L))
        {
            MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().RoomConnecter, "BeginClass", UserInfor.getInstance().RoomName, UserInfor.getInstance().UserToken);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // 初始化服务器场景
            CollectionObject.getInstance().SyncSceneToService(true);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("hold cube");
            MsgModule.getInstance().req_operation_permission(UserInfor.getInstance().UserToken, "Cube", UserInfor.getInstance().UserUuid);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("release cube");
            MsgModule.getInstance().req_operation_release(UserInfor.getInstance().UserToken, "Cube", UserInfor.getInstance().UserUuid);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("hold sphere");
            MsgModule.getInstance().req_operation_permission(UserInfor.getInstance().UserToken, "sphere", UserInfor.getInstance().UserUuid);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("release sphere");
            MsgModule.getInstance().req_operation_release(UserInfor.getInstance().UserToken, "sphere", UserInfor.getInstance().UserUuid);
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            MsgModule.getInstance().req_switch_model(UserInfor.getInstance().UserToken, "Collaboration", UserInfor.getInstance().UserUuid);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            MsgModule.getInstance().req_switch_model(UserInfor.getInstance().UserToken, "Separate", UserInfor.getInstance().UserUuid);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            MsgModule.getInstance().req_switch_model(UserInfor.getInstance().UserToken, "SynchronousOne", UserInfor.getInstance().UserUuid);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MsgModule.getInstance().req_switch_model(UserInfor.getInstance().UserToken, "SynchronousMultiple", UserInfor.getInstance().UserUuid);
        }
    }

    public void enterCourse(string name)
    {
        MsgModule.getInstance().registerMsgHandler(this);

        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_enter_room", UserInfor.getInstance().UserToken, UserInfor.getInstance().UserName, UserInfor.getInstance().UserUuid, name, "cMsgConnect", "ret_msg");
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];

        Debug.Log("进入课程返回 " + result);

        if (result == "success")
        {
            string modelname = (string)msg["connector"];
            string scenename = (string)msg["scenename"];

            UserInfor.getInstance().RoomConnecter = modelname;
            UserInfor.getInstance().RoomName = scenename;
        }
        else
        {
            Debug.Log("进入课程失败");
        }
    }
}
