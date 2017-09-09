using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class PlayerInfor : PlayerBaseInfor
{
	public PlayerInfor()
    {

    }

    public PlayerInfor(DataType.StudentInfor student)
    {
        this.name = student.user_name;
        this.student_name = student.student_name;
        this.sexdesc = student.sex;
        this.duty = ComonEnums.DutyEnum.Student;
        this.userid = Convert.ToInt32(student.user_id);
        this.avatar = student.avatar;
        this.classes = student.classes;
        this.school = student.school;
    }

    public string GetDuty()
    {
        string ret = null;

        switch(duty)
        {
            case ComonEnums.DutyEnum.Student:
                ret = this.groupname;
                break;
            case ComonEnums.DutyEnum.GroupLeader:
                break;
            case ComonEnums.DutyEnum.Assistant:
                ret = "助教";
                break;
            case ComonEnums.DutyEnum.Teacher:
                ret = "老师";
                break;
            default:
                break;
        }

        return ret;
    }

    // 被选中 发送 通知消息
    public void OnClick()
    {
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string>(EventId.ChoosePerson, this.token);
    }

    // 玩家头像链接
    public string avatar;
    public string student_name;
    public DataType.Classes classes;
    public DataType.College college;
    public DataType.School school;

    public string groupname;
    public string grouptoken;
    public int iconid;
    public bool isonline = false;
}
