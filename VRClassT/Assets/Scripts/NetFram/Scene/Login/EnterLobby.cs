using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnterLobby : MonoBehaviour
{
    private void OnEnable()
    {
        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retEnterLab, ret_msg);
    }

    private void OnDisable()
    {
        CommandReceive.getInstance().RemoveHashMsgListener(CommandDefine.HashTableMsgType.retEnterLab, ret_msg);
    }

    public void PlayerEnterLobby()
    {
        string token = UserInfor.getInstance().UserToken;
        int userid = (int)UserInfor.getInstance().UserId;
        int duty = UserInfor.getInstance().UserDutyId;

        if(token == null || userid < 0 || duty < 0)
        {
            Debug.Log("token == null || userid < 0 || duty < 0");
            return;
        }

        NetworkCommunicate.getInstance().PlayerEnterLab(token, userid, duty);

        //MainThreadClient._client.call_hub("lobby", "WisdomLogin", "EnterLobby", token, UserInfor.getInstance().UserId, (Int64)UserInfor.getInstance().UserDutyId);
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];

        Debug.Log(result + " -- EnterLobby ");

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

            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.CourseList);
        }
        else if (result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }
    }
}
