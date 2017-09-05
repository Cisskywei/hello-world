using ko.NetFram;
using System;
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
    public void InitPlayers()
    {
        DataType.StudentInfor[] s = UserInfor.getInstance().courseinfor.students;

        if (s == null || s.Length <= 0)
        {
            return;
        }

        studentallcount = s.Length;

        for (int i=0;i<s.Length;i++)
        {
            PlayerInfor p = new PlayerInfor(s[i]);
            playerlist.Add(p.userid,p);
        }
    }

    public void ChangePlayerOnline(int userid)
    {
        if(!playerlist.ContainsKey(userid))
        {
            return;
        }

        if(!playerlist[userid].isonline)
        {
            studentonlinecount++;

            if (studentonlinecount > studentallcount)
            {
                studentonlinecount = studentallcount;
            }
        }

        playerlist[userid].isonline = true;
    }

    public void ChangePlayerOnline(ArrayList userids)
    {
        if (userids == null || userids.Count <= 0)
        {
            return;
        }

        for(int i=0;i<userids.Count;i++)
        {
            if (!playerlist.ContainsKey((int)userids[i]))
            {
                continue;
            }

            if (!playerlist[(int)userids[i]].isonline)
            {
                studentonlinecount++;

                if(studentonlinecount > studentallcount)
                {
                    studentonlinecount = studentallcount;
                }
            }

            playerlist[(int)userids[i]].isonline = true;
        }
    }

    public void DivideGroupBack(Hashtable players)
    {
        if(players == null || players.Count <= 0)
        {
            return;
        }

        GroupInfor gi = null;
        foreach (DictionaryEntry di in players)
        {
            if(grouplist.ContainsKey(Convert.ToString(di.Value)))
            {
                gi = grouplist[Convert.ToString(di.Value)];
            }
            else
            {
                gi = new GroupInfor(Convert.ToString(di.Value));
                grouplist.Add(Convert.ToString(di.Value), gi);
            }

            if(gi != null && playerlist.ContainsKey(Convert.ToInt64(di.Key)))
            {
                gi.AddMember(playerlist[Convert.ToInt64(di.Key)]);
            }
        }
    }

    // 提供界面调用接口

    // 计算出勤率
    public string CalculateTheAttendance()
    {
        string rate = null;

        rate = "出勤率：" + studentonlinecount.ToString() + " / " + studentallcount.ToString();

        return rate;
    }

    // 获取点赞次数显示
    public string GetLikeCount()
    {
        string ret = null;

        ret = "点赞：" + likecount.ToString() + "/" + studentonlinecount.ToString();

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
    public Dictionary<Int64, PlayerInfor> GetGroupMemeber(string groupname)
    {
        if(grouplist.Count <= 0 || !grouplist.ContainsKey(groupname))
        {
            return null;
        }

        if(groupname == null)
        {
            //默认获取第一个小组信息
            groupname = "组1";
        }

        return grouplist[groupname].members;
    }

    // 获取指定成员
    public PlayerInfor GetPlayerById(Int64 id)
    {
        PlayerInfor p = null;

        try
        {
            p = playerlist[id];
        }catch
        {

        }

        return p;
    }

    // 获取所有成员列表
    public Dictionary<Int64, PlayerInfor> GetAllPlayerList()
    {
        return playerlist;
    }

    // 界面改变当前模式
    public void SwitchTeachingMode(ComonEnums.TeachingMode mode)
    {
        Debug.Log("切换模式 == ==== === ");
        if (!IsTeacher)
        {
            return;
        }

        operateattribute = ComonEnums.OperatingAttribute.SwitchMode;

        if (this.mode != mode)
        {
            this.mode = mode;
        }

        switch (mode)
        {
            case ComonEnums.TeachingMode.SelfTrain_Group:
                MsgModule.getInstance().reqSwitchTeachMode(mode, true, null);
                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                MsgModule.getInstance().reqSwitchTeachMode(mode, false, null);
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
    //            MsgModule.getInstance().reqSwitchTeachMode(mode, true, null);
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
     //           MsgModule.getInstance().reqSwitchTeachMode(mode, false, null);
                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
            case ComonEnums.TeachingMode.SelfTrain_All:
            case ComonEnums.TeachingMode.VideoOnDemand_General:
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
            case ComonEnums.TeachingMode.VideoOnLive_General:
            case ComonEnums.TeachingMode.VideoOnLive_Full:
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
            case ComonEnums.OperatingAttribute.ResetScene:
                MsgModule.getInstance().reqResetScene(ComonEnums.ResetSceneType.Group, token);
                break;
            case ComonEnums.OperatingAttribute.SwitchMode:
                MsgModule.getInstance().reqSwitchTeachMode(this.mode, true, token);
                break;
            default:
                break;
        }
    }

    public void ChoosePerson(Int64 userid)
    {
        if (!IsTeacher)
        {
            return;
        }

        switch (operateattribute)
        {
            case ComonEnums.OperatingAttribute.ResetScene:
                MsgModule.getInstance().reqResetScene(ComonEnums.ResetSceneType.Student, userid.ToString());
                break;
            case ComonEnums.OperatingAttribute.SwitchMode:
                MsgModule.getInstance().reqSwitchTeachMode(this.mode, false, userid.ToString());
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

        operateattribute = ComonEnums.OperatingAttribute.ResetScene;

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
        MsgModule.getInstance().reqResetScene(ComonEnums.ResetSceneType.All, null);
    }

    //接收学生点赞 举手
    public void ReceiveLike(Int64 userid)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
        likecount++;
    }

    public void ReceiveDoubt(Int64 userid)
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
    public void TestInClass(ComonEnums.InClassTestType catage, int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
        MsgModule.getInstance().reqInClassTest(catage, id, null);
    }

    public void FastTestInClass(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // 测试
  //      MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().Connector, "FastQuestionTest");

        // TODO
    }

    public void AskTestInClass(int id)
    {
        if (!IsTeacher)
        {
            return;
        }

        // 测试
  //      MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().Connector, "QuestionTest", (Int64)16);

        // TODO
    }

    // 学生答题按钮的函数
    public void AnswerQuestion(ComonEnums.InClassTestType category, ComonEnums.QuestionType typ, int questionid)
    {
        // TODO
    }

    // 只为测试
    public void AnswerQuestionTest(int optionid)
    {
 //       MainThreadClient._client.call_hub("lobby", UserInfor.getInstance().Connector, "AnswerQuestion", (Int64)optionid);
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string, int, int>(EventId.TestFeedBack, this.TestFeedBack);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string, int, int>(EventId.TestFeedBack, this.TestFeedBack);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    public ComonEnums.TeachingMode mode;
    public Dictionary<string, GroupInfor> grouplist = new Dictionary<string, GroupInfor>(); // 根据名字或者token的字典索引小组列表
    public Dictionary<Int64, PlayerInfor> playerlist = new Dictionary<Int64, PlayerInfor>(); // 所有学生列表
    public int groupcount;
    public int studentallcount;
    public int studentonlinecount;
    public int likecount;

    // 操作状态信息
    public ComonEnums.OperatingAttribute operateattribute = ComonEnums.OperatingAttribute.None;

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

    // 分组算法

    // 转换模式枚举和文字
    public static string ConvertModeToString(ComonEnums.TeachingMode mode)
    {
        string modeTxt = string.Empty;

        switch (mode)
        {
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
                modeTxt = "观学模式-同步";
                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
                modeTxt = "观学模式-异步";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                modeTxt = "指导模式-指导人";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                modeTxt = "指导模式-指导组";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                modeTxt = "自主训练模式-独立";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
                modeTxt = "自主训练模式-小组";
                break;
            case ComonEnums.TeachingMode.SelfTrain_All:
                modeTxt = "自主训练模式-全部";
                break;
            case ComonEnums.TeachingMode.VideoOnDemand_General:
                modeTxt = "视频点播-普通";
                break;
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
                modeTxt = "视频点播-全景";
                break;
            case ComonEnums.TeachingMode.VideoOnLive_General:
                modeTxt = "视频直播-普通";
                break;
            case ComonEnums.TeachingMode.VideoOnLive_Full:
                modeTxt = "视频直播-全景";
                break;
            default:
                break;
        }

        return modeTxt;
    }
}
