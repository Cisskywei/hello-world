using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnterCourse : MonoBehaviour, msg_req_ret, msg_req_ret_json
{

    public ClassRoom cr;

    private int courseid;

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void PlayerEnterCourse(int courseid)
    {
        MsgModule.getInstance().registerMsgHandler(this);

        this.courseid = courseid;

        Debug.Log("进入教室" + courseid);
        MainThreadClient._client.call_hub("lobby", "WisdomLogin", "EnterCourse", UserInfor.getInstance().UserId, UserInfor.getInstance().UserUuid, (Int64)courseid);
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        string result = (string)msg["result"];

        if (result == "success")
        {
            Int64 classid = Convert.ToInt64(msg["classid"]);
            string room = (string)(msg["connector"]);
            string jsondata = (string)msg["jsondata"];
            string rooturl = (string)msg["rooturl"];

            UserInfor.getInstance().RoomId = classid;
            UserInfor.getInstance().RoomConnecter = room;

            if(rooturl != null)
            {
                UserInfor.getInstance().courseinfor_rooturl = rooturl;
            }

            if (jsondata != null)  // 老师会有jsondata返回
            {
                //1, 获取课程信息 以及该课程的学生列表

                jsondata = JsonDataHelp.getInstance().DecodeBase64(null, jsondata);

                Debug.Log(jsondata);
                
                DataType.CourseInforRetData course = JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseInforRetData>(jsondata);

                if(course != null)
                {
                    UserInfor.getInstance().courseinfor = course.data;
                }

                UserInfor.getInstance().isTeacher = true;

                // 获取了 学生列表 所以在这个地方进行学生列表初始化  目前只是默认 后期根据班级初始化 !!!!!!
                ClassManager.getInstance().InitDefault();
            }
            else
            {
                UserInfor.getInstance().isTeacher = false;
            }

            // 真正的智慧教室开始  进入智慧教室 某个课程的场景里  需有脚本接受消息 接收老师操作 接收指令  (数据同步等操作是具体的vr课件所具有的功能) !!!!!!!!!!!!
            // TODO
            // 
            if (cr != null)
            {
                // 多此步骤主要是为了获取课程的题目数据
                cr.EnterClassRoom(this.courseid);
            }
        }
        else if (result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }
    }

    public void ret_msg_json(string msg)
    {
        Debug.Log("msg" + msg);
    }
}
