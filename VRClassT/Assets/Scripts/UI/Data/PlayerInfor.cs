﻿using ko.NetFram;
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

    public PlayerInfor(string name, Enums.SexEnum sex, Enums.DutyEnum duty, bool isonline = true)
    {
        this.name = name;
        this.sex = sex;
        this.duty = duty;
        this.isonline = isonline;

        this.token = SystemInfo.deviceUniqueIdentifier + "px";
    }

    public PlayerInfor(DataType.StudentInfor student)
    {
        this.name = student.student_name;
        this.sexdesc = student.sex;
        this.duty = Enums.DutyEnum.Student;
        this.userid = Convert.ToInt64(student.user_id);

        this.avatar = student.avatar;
    }

    public string GetDuty()
    {
        string ret = null;

        switch(duty)
        {
            case Enums.DutyEnum.Student:
                ret = this.groupname;
                break;
            case Enums.DutyEnum.GroupLeader:
                break;
            case Enums.DutyEnum.Assistant:
                ret = "助教";
                break;
            case Enums.DutyEnum.Teacher:
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

    public string groupname;
    public string grouptoken;
    public int iconid;
    public bool isonline = false;
}