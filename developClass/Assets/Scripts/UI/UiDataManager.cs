using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class UiDataManager {

    public static UiDataManager getInstance()
    {
        return Singleton<UiDataManager>.getInstance();
    }

    // 初始化数据
    public void InitGroupPlayer(Hashtable players)
    {
        if(players == null || players.Count <= 0)
        {
            return;
        }

        int i = 0;
        GroupInfor g1 = new GroupInfor("组1");
        GroupInfor g2 = new GroupInfor("组2");
        foreach (DictionaryEntry v in players)
        {
            PlayerInfor p = new PlayerInfor((string)v.Value, Enums.SexEnum.Male, Enums.DutyEnum.Student, true);
            playerlist.Add((string)v.Key, p);

            if(i%2==0)
            {
                g2.AddMember(p);
                
            }
            else
            {
                g2.AddMember(p);
            }

            i++;
        }

        grouplist.Add("g1", g1);
        grouplist.Add("g2", g2);
    }

    // 提供界面调用接口

    // 计算出勤率
    public string CalculateTheAttendance()
    {
        string rate = null;

        rate = studentonlinecount.ToString() + "/" + studentallcount.ToString();

        return rate;
    }

    // 获取点赞次数显示
    public string GetLikeCount()
    {
        string ret = null;

        ret = likecount.ToString() + "/" + studentonlinecount.ToString();

        return ret;
    }

    // 获取小组信息
    public Dictionary<string, GroupInfor> GetGroupList()
    {
        return grouplist;
    }

    // 获取小组信息
    public List<string> GetGroupNameList()
    {
        List<string> groupname = new List<string>();

        foreach(string name in grouplist.Keys)
        {
            groupname.Add(name);
        }
        return groupname;
    }

    // 获取指定小组
    public GroupInfor GetGroupByToken(string token)
    {
        GroupInfor p = null;

        try
        {
            p = grouplist[token];
        }
        catch
        {

        }

        return p;
    }

    // 获取小组成员信息
    public Dictionary<string, PlayerInfor> GetGroupMemeber(string groupname)
    {
        if(grouplist.Count <= 0 || !grouplist.ContainsKey(groupname))
        {
            return null;
        }

        if(groupname == null)
        {
            //默认获取第一个小组信息
            return null;
        }

        return grouplist[groupname].members;
    }

    // 获取指定成员
    public PlayerInfor GetPlayerByToken(string token)
    {
        PlayerInfor p = null;

        try
        {
            p = playerlist[token];
        }catch
        {

        }

        return p;
    }

    // 获取所有成员列表
    public Dictionary<string, PlayerInfor> GetAllPlayerList()
    {
        return playerlist;
    }

    // 界面改变当前模式
    public void SwitchTeachingMode(Enums.TeachingMode mode)
    {
        if (!IsTeacher)
        {
            return;
        }

        operateattribute = Enums.OperatingAttribute.SwitchMode;

        if (this.mode != mode)
        {
            this.mode = mode;
        }

        switch (mode)
        {
            case Enums.TeachingMode.WatchLearnModel_Sync:
            case Enums.TeachingMode.WatchLearnModel_Async:
            case Enums.TeachingMode.SelfTrain_All:
            case Enums.TeachingMode.VideoOnDemand_General:
            case Enums.TeachingMode.VideoOnDemand_Full:
            case Enums.TeachingMode.VideoOnLive_General:
            case Enums.TeachingMode.VideoOnLive_Full:
                MsgModule.getInstance().reqSwitchTeachMode(mode, false, null);
                break;
            default:
                break;
        }

    }

    // 提供见面调用 和服务器交互的接口

    // 主动通知界面更新数据

    // 界面 和 服务器的交互

    public void ChooseGroup(string token)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (token == null)
        {
            return;
        }

        switch (operateattribute)
        {
            case Enums.OperatingAttribute.ResetScene:
                MsgModule.getInstance().reqResetScene(Enums.ResetSceneType.Group, token);
                break;
            case Enums.OperatingAttribute.SwitchMode:
                MsgModule.getInstance().reqSwitchTeachMode(this.mode, true, token);
                break;
            default:
                break;
        }
    }

    public void ChoosePerson(string token)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (token == null)
        {
            return;
        }

        switch (operateattribute)
        {
            case Enums.OperatingAttribute.ResetScene:
                MsgModule.getInstance().reqResetScene(Enums.ResetSceneType.Student, token);
                break;
            case Enums.OperatingAttribute.SwitchMode:
                MsgModule.getInstance().reqSwitchTeachMode(this.mode, false, token);
                break;
            default:
                break;
        }
    }

    // 重置场景
    public void ResetScene(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        operateattribute = Enums.OperatingAttribute.ResetScene;

        //switch (id)
        //{
        //    case 0:
        //        // 重置全部
        //        break;
        //    case 1:
        //        // 重置组
        //        break;
        //    case 2:
        //        // 重置学生
        //        break;
        //    default:
        //        break;
        //}
    }

    // 重置所有人的场景
    public void ResetSceneAll()
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
        MsgModule.getInstance().reqResetScene(Enums.ResetSceneType.All, null);
    }

    //接收学生点赞 举手
    public void ReceiveLike(string token)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
    }

    public void ReceiveDoubt(string token)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
    }

    // 学生点赞 举手
    public void SendLike()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        MsgModule.getInstance().reqSendLike();
    }

    public void SendDoubt()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        MsgModule.getInstance().reqSendDoubt();
    }

    // 随堂测试相关  ---- --- -- -随堂测试相关
    public void TestInClass(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
        MsgModule.getInstance().reqInClassTest(Enums.InClassTestType.Test, id, null);
    }

    public void FastTestInClass(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
    }

    public void AskTestInClass(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
    }

    // 学生答题按钮的函数
    public void AnswerQuestion(Enums.InClassTestType category, Enums.QuestionType typ, int questionid)
    {
        // TODO
    }

    // 网络数据返回
    public void TestFeedBack(string token, int questionid, int optionid)
    {
        if (!IsTeacher)
        {
            return;
        }

        //
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ResetScene, this.ResetScene);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string, int, int>(EventId.TestFeedBack, this.TestFeedBack);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ResetScene, this.ResetScene);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string, int, int>(EventId.TestFeedBack, this.TestFeedBack);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    public Enums.TeachingMode mode;
    public Dictionary<string, GroupInfor> grouplist = new Dictionary<string, GroupInfor>(); // 根据名字或者token的字典索引小组列表
    public Dictionary<string, PlayerInfor> playerlist = new Dictionary<string, PlayerInfor>(); // 所有学生列表
    public int groupcount;
    public int studentallcount;
    public int studentonlinecount;
    public int likecount;

    // 操作状态信息
    public Enums.OperatingAttribute operateattribute = Enums.OperatingAttribute.None;

    // 控制变量 标记是否是 老师
    private bool _isTecaher = false;
    public bool IsTeacher
    {
        get
        {
            return UserInfor.getInstance().isTeacher;
        }

        set
        {
            _isTecaher = value;
        }
    }
}
