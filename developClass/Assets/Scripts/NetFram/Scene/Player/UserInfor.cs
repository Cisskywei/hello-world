using ko.NetFram;
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

    // 模式相关
    // 权限控制相关 
    public string groupname; // 所属小组名称
    public bool isleader = false;  //如果为true 则相应等级需要提升
    public SceneConfig.ModelEnums model = SceneConfig.ModelEnums.None;
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
    public void ChangePlayerModel(string tomodel)
    {
        ChangePlayerModel(convertModelToEnum(tomodel));
    }

    public void ChangePlayerModel(SceneConfig.ModelEnums tomodel)
    {
        switch (tomodel)
        {
            case SceneConfig.ModelEnums.None:
                isCanReceive = false;
                isCanSend = false;
                isCanOperate = false;
                break;
            case SceneConfig.ModelEnums.Separate:
                isCanReceive = false;
                isCanSend = false;
                isCanOperate = true;
                break;
            case SceneConfig.ModelEnums.SynchronousOne:
                isCanReceive = true;
                isCanSend = false;
                isCanOperate = false;
                if (isleader)
                {
                    isCanSend = true;
                    isCanOperate = true;
                }
                break;
            case SceneConfig.ModelEnums.SynchronousMultiple:
                isCanReceive = true;
                isCanSend = false;
                isCanOperate = false;
                if (isleader)
                {
                    isCanSend = true;
                    isCanOperate = true;
                }
                break;
            case SceneConfig.ModelEnums.Collaboration:
                isCanReceive = true;
                isCanSend = true;
                isCanOperate = true;
                break;
            default:
                break;
        }

        this.model = tomodel;
    }

    public void ChangePlayerModel(string teacher, Enums.TeachingMode tomodel, string target)
    {
        string token = UserInfor.getInstance().UserToken;
        bool isself = false;
        switch (tomodel)
        {
            case Enums.TeachingMode.WatchLearnModel_Sync:
                isself = (token == target || token == teacher) || false;
                isCanReceive = true;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.WatchLearnModel_Async:
                isself = (token == target || token == teacher) || false;
                isCanReceive = true;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.GuidanceMode_Personal:
                isself = (token == target || token == teacher) || false;
                isCanReceive = isself;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.GuidanceMode_Group:
                isself = (groupname == target || token == teacher) || false;
                isCanReceive = isself;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.SelfTrain_Personal:
                isself = (token == target || token == teacher) || false;
                isCanReceive = true;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.SelfTrain_Group:
                isself = (groupname == target || token == teacher) || false;
                isCanReceive = isself;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.SelfTrain_All:
                isself = true;
                isCanReceive = isself;
                isCanSend = isself;
                isCanOperate = isself;
                break;
            case Enums.TeachingMode.VideoOnDemand_General:
            case Enums.TeachingMode.VideoOnDemand_Full:
            case Enums.TeachingMode.VideoOnLive_General:
            case Enums.TeachingMode.VideoOnLive_Full:
                break;
            default:
                break;
        }
        //switch (tomodel)
        //{
        //    case Enums.TeachingMode.None:
                
        //        break;
        //    case SceneConfig.ModelEnums.Separate:
        //        isCanReceive = false;
        //        isCanSend = false;
        //        isCanOperate = true;
        //        break;
        //    case SceneConfig.ModelEnums.SynchronousOne:
        //        isCanReceive = true;
        //        isCanSend = false;
        //        isCanOperate = false;
        //        if (isleader)
        //        {
        //            isCanSend = true;
        //            isCanOperate = true;
        //        }
        //        break;
        //    case SceneConfig.ModelEnums.SynchronousMultiple:
        //        isCanReceive = true;
        //        isCanSend = false;
        //        isCanOperate = false;
        //        if (isleader)
        //        {
        //            isCanSend = true;
        //            isCanOperate = true;
        //        }
        //        break;
        //    case SceneConfig.ModelEnums.Collaboration:
        //        isCanReceive = true;
        //        isCanSend = true;
        //        isCanOperate = true;
        //        break;
        //    default:
        //        break;
        //}

  //      this.model = tomodel;
    }

    private SceneConfig.ModelEnums convertModelToEnum(string modelname)
    {
        SceneConfig.ModelEnums m = this.model;
        switch (modelname)
        {
            case "Separate":
                m = SceneConfig.ModelEnums.Separate;
                break;
            case "SynchronousOne":
                m = SceneConfig.ModelEnums.SynchronousOne;
                break;
            case "SynchronousMultiple":
                m = SceneConfig.ModelEnums.SynchronousMultiple;
                break;
            case "Collaboration":
                m = SceneConfig.ModelEnums.Collaboration;
                break;
            default:
                break;
        }

        return m;
    }

    private string convertEnumToModel(SceneConfig.ModelEnums mudelenum)
    {
        string m = null;
        switch (mudelenum)
        {
            case SceneConfig.ModelEnums.Separate:
                m = "Separate";
                break;
            case SceneConfig.ModelEnums.SynchronousOne:
                m = "SynchronousOne";
                break;
            case SceneConfig.ModelEnums.SynchronousMultiple:
                m = "SynchronousMultiple";
                break;
            case SceneConfig.ModelEnums.Collaboration:
                m = "Collaboration";
                break;
            default:
                break;
        }

        return m;
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
