using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnterLobby : MonoBehaviour, msg_req_ret
{
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void PlayerEnterLobby(string token = null)
    {
        if(token == null)
        {
            token = UserInfor.getInstance().UserToken;
        }

        if (token == null)
        {
            return;
        }

        MsgModule.getInstance().registerMsgHandler(this);

        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "EnterLobby", token, UserInfor.getInstance().UserId, (Int64)UserInfor.getInstance().UserDutyId);
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];
        Debug.Log(result);
        if (result == "success")
        {
            string jsondata = (string)msg["jsondata"];
            DataType.CourseListRetData course = null;
            if (jsondata != null)
            {
                jsondata = JsonDataHelp.getInstance().DecodeBase64(null, jsondata);

                Debug.Log(jsondata);

                course = JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseListRetData>(jsondata);
            }

            if(course != null)
            {
                UserInfor.getInstance().courselist = course.data;
            }

            bool check = false;
            // 检测是否是从课件返回登陆
   //         check = BackFromVrExe.getInstance().EnterLobby();
            if (!check)
            {
                //显示课程列表
                //TODO
                OutUiManager.getInstance().ShowUI(OutUiManager.UIList.CourseList);
            }
            else
            {
                // 
            }
        }
        else if (result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }
    }
}
