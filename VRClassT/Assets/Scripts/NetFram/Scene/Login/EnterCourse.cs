using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnterCourse
{
    public static EnterCourse getInstance()
    {
        return Singleton<EnterCourse>.getInstance();
    }

    public EnterCourse()
    {
        RegListener();
    }

    private void RegListener()
    {
        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retEnterCourse, ret_msg);
    }

    private void RemoveListener()
    {
        CommandReceive.getInstance().RemoveHashMsgListener(CommandDefine.HashTableMsgType.retEnterCourse, ret_msg);
    }

    private int courseid = -1;

    public void PlayerEnterCourse(int courseid)
    {
        this.courseid = courseid;

        Debug.Log("进入教室" + courseid);

        NetworkCommunicate.getInstance().PlayerEnterCourse((int)UserInfor.getInstance().UserId, UserInfor.getInstance().UserUuid, courseid);

        //MainThreadClient._client.call_hub("lobby", "WisdomLogin", "EnterCourse", UserInfor.getInstance().UserId, UserInfor.getInstance().UserUuid, (Int64)courseid);
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
            if(courseid < 0)
            {
                Debug.Log(" 课程id 为空 " + courseid);
            }
            else
            {
                ClassRoom.getInstance().EnterClassRoom(this.courseid);
            }
        }
        else if (result == "failed")
        {
            string errormsg = (string)msg["errormsg"];

            Debug.Log("错误信息: " + errormsg);
        }

        RemoveListener();
    }
}
