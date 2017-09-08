using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfor {

    public static UserInfor getInstance()
    {
        return Singleton<UserInfor>.getInstance();
    }

    private string _username;
    public string UserName
    {
        get
        {
            return _username;
        }
        set
        {
            _username = value;
        }
    }

    private string _useruuid;
    public string UserUuid
    {
        get
        {
            return _useruuid;
        }
        set
        {
            _useruuid = value;
        }
    }

    private int _userid = -1;
    public Int64 UserId
    {
        get
        {
            return (Int64)_userid;
        }
        set
        {
            _userid = (int)value;
        }
    }

    private string _usertoken;
    public string UserToken
    {
        get
        {
            return _usertoken;
        }
        set
        {
            _usertoken = value;
        }
    }

    private string _teachername;
    public string TeacherName
    {
        get
        {
            return _teachername;
        }
        set
        {
            _teachername = value;
        }
    }

    private bool _isTeacher = false;
    public bool isTeacher
    {
        get
        {
            return _isTeacher;
        }
        set
        {
            _isTeacher = value;
        }
    }

    private string _duty;
    public string UserDuty
    {
        get
        {
            return _duty;
        }
        set
        {
            _duty = value;

            if(value == "teacher")
            {
                _isTeacher = true;
            }
        }
    }

    private int _dutyid;
    public int UserDutyId
    {
        get
        {
            if(_duty == "teacher")
            {
                _dutyid = 2;
            }
            else if(_duty == "student")
            {
                _dutyid = 1;
            }
            else
            {
                _dutyid = 0;
            }

            return _dutyid;
        }
        set
        {
            _dutyid = value;
        }
    }

    // 当前默认连接者
    private string _connector;
    public string Connector
    {
        get
        {
            return _connector;
        }
        set
        {
            _connector = value;
        }
    }

    // 房间连接者
    private string _roomconnector;
    public string RoomConnecter
    {
        get
        {
            return _roomconnector;
        }
        set
        {
            _roomconnector = value;
        }
    }

    private string _roomname;
    public string RoomName
    {
        get
        {
            return _roomname;
        }
        set
        {
            _roomname = value;
        }
    }

    private Int64 _roomid;
    public Int64 RoomId
    {
        get
        {
            return _roomid;
        }
        set
        {
            _roomid = value;
        }
    }

    public string avatar = string.Empty;

    // 真正的数据开始   服务器数据

    //课程列表
    public DataType.CourseListData[] courselist;
    public string courselist_rooturl;
    public DataType.CourseInfor courseinfor;
    public string courseinfor_rooturl;
    public static string RootUrl = "http://www.hdmooc.com:5557";

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // 模式相关
    // 权限控制相关 
    public string groupname; // 所属小组名称
    public bool isleader = false;  //如果为true 则相应等级需要提升
    public ComonEnums.TeachingMode model = ComonEnums.TeachingMode.WatchLearnModel_Sync;
    // 根据模式控制收发状态
    public bool isCanReceive = false;
    public bool isCanSend = false;
    public bool isCanOperate = false;   //是否可操作物体  用于请求释放操作权限

    public bool CheckCanSend()
    {
        bool ret = this.isCanSend;

        return ret;
    }

    public bool CheckCanRecive()
    {
        bool ret = this.isCanReceive;

        return ret;
    }

    public bool CheckCanSync3DInfor()
    {
        // 排除物理属性等的同步
        bool ret = true;

        return ret;
    }

    public bool CheckCanOperate()
    {
        bool ret = this.isCanOperate;

        return ret;
    }

    // 接收自身模式改变  默认情况的收、发、操作权限
    public void ChangePlayerModel(Int64 userid, ComonEnums.TeachingMode tomodel, string target)
    {
        int selfid = UserInfor.getInstance()._userid;
        int targetid = -1;
        if (target != null && target != "all")
        {
            targetid = Convert.ToInt32(target);
        }
        
        bool isself = false;
        switch (tomodel)
        {
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
                isself = (selfid == targetid || userid == selfid) || false;
                isCanReceive = true;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
                isself = (userid == targetid || userid == selfid) || false;
                isCanReceive = true;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                isself = (selfid == targetid || selfid == userid) || false;
                isCanReceive = isself;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                isCanReceive = true;
                isCanSend = true;
                isCanOperate = true;
                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                isCanReceive = false;
                isCanSend = false;
                isCanOperate = true;
                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
                isCanReceive = true;
                isCanSend = true;
                isCanOperate = true;
                break;
            case ComonEnums.TeachingMode.SelfTrain_All:
                isCanReceive = true;
                isCanSend = true;
                isCanOperate = true;
                break;
            case ComonEnums.TeachingMode.VideoOnDemand_General:
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
            case ComonEnums.TeachingMode.VideoOnLive_General:
            case ComonEnums.TeachingMode.VideoOnLive_Full:
                break;
            default:
                break;
        }

        this.model = tomodel;

        Debug.Log(this.model);
        Debug.Log("isCanReceive : " + this.isCanReceive);
        Debug.Log("isCanSend : " + this.isCanSend);
        Debug.Log("isCanOperate : " + this.isCanOperate);
    }

    // 动态提升学生收、发、操作权限
    public void ChangePlayerCanReceive(bool change)
    {
        // 后期加入权限管理验证
        isCanReceive = change;
    }

    public void ChangePlayerCanSend(bool change)
    {
        isCanSend = change;
    }

    public void ChangePlayerCanOperate(bool change)
    {
        isCanOperate = change;
    }
}
