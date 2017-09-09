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
    public int UserId
    {
        get
        {
            return _userid;
        }
        set
        {
            _userid = value;
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

    private ComonEnums.DutyEnum _duty;
    public ComonEnums.DutyEnum UserDuty
    {
        get
        {
            return _duty;
        }
        set
        {
            _duty = value;

            if(value == ComonEnums.DutyEnum.Teacher)
            {
                _isTeacher = true;
            }
            else
            {
                _isTeacher = false;
            }
        }
    }

    public int UserDutyId
    {
        get
        {
            return (int)_duty;
        }
    }

    private int _roomid;
    public int RoomId
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
    #region 网络控制自身
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
    #endregion
*/
}
