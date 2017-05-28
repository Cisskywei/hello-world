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

    public string groupname;
    public string grouptoken;
    public string token;    // 用于服务器识别
    public int iconid;
    public bool isonline;
}
