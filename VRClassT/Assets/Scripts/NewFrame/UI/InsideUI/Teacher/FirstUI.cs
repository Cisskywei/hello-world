using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class FirstUI : OutUIBase
{
    public enum UIListType
    {
        None = -1,

        Model,
        HandUpStudent,
        GuidePerson,
        GuideTeam,
        Question,
        QuestionBack,
        FastQuestion,
        PaletteCtrl,
    }

    public OutUIBase[] uilist;

    public override void ShowSelf(params object[] args)
    {
        if(args != null && args.Length > 0)
        {
            UIListType f = (UIListType)args[0];
            ShowUI(f);
        }
        base.ShowSelf(args);
    }

    public void ShowUI(UIListType id, params System.Object[] args)
    {
        if (uilist == null || uilist.Length <= 0)
        {
            return;
        }

        int uiid = (int)id;
        if (uiid > uilist.Length)
        {
            return;
        }

        for (int i = 0; i < uilist.Length; i++)
        {
            if (i == uiid)
            {
                uilist[i].ShowSelf(args);
            }
            else
            {
                uilist[i].HideSelf();
            }
        }
    }

    void OnEnable()
    {
        RegisterEvent();
        UiDataManager.getInstance().RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEvent();
        UiDataManager.getInstance().UnRegisterEventListener();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64, Int64, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);

        // student
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int, int, string>(EventId.StudentReciveQuestion, this.StudentReciveQuestion);

        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.BackToLobby, this.BackToLobby);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.ResetScene, this.ResetSceneBack);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64, Int64, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.OpenUI, this.OpenUI);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, int, string>(EventId.StudentReciveQuestion, this.StudentReciveQuestion);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.BackToLobby, this.BackToLobby);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.ResetScene, this.ResetSceneBack);

    }

    // 监听函数
    public void SwitchMode(ComonEnums.TeachingMode mode)
    {
        if (!IsTeacher)
        {
            return;
        }

        // 判断弹出对应界面
        switch (mode)
        {
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                //弹出学生列表界面
                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                //弹出小组选择界面
                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                //学生各自协同操作
                //players.ShowSelf();

                //groups.HideSelf();
                //question.HideSelf();
                //questionback.HideSelf();
                //fastquestion.HideSelf();

                //ShowFirstUI(false);


                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
                //小组各自协同操作
                //groups.ShowSelf();

                //players.HideSelf();
                //question.HideSelf();
                //questionback.HideSelf();
                //fastquestion.HideSelf();

                //ShowFirstUI(false);


                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
            case ComonEnums.TeachingMode.SelfTrain_All:
            case ComonEnums.TeachingMode.VideoOnDemand_General:
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
            case ComonEnums.TeachingMode.VideoOnLive_General:
            case ComonEnums.TeachingMode.VideoOnLive_Full:

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                break;
            default:
                break;
        }

    }

    // 切换模式返回 更新界面状态
    public void SwitchModeFeedBack(Int64 userid, Int64 modee, string targetid)
    {
        ComonEnums.TeachingMode mode = (ComonEnums.TeachingMode)modee;
        Debug.Log(mode + " -- 模式改变");
        // TODO
        string who = string.Empty;
        PlayerInfor pi = null;
        GroupInfor gi = null;

        // 判断弹出对应界面
        switch (mode)
        {
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
                modeTxt = "观学模式-同步";
                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
                modeTxt = "观学模式-异步";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                pi = UiDataManager.getInstance().GetPlayerById(Convert.ToInt64(targetid));
                if (pi != null)
                {
                    who = pi.name;
                }
                modeTxt = "指导模式-指导人 (" + who + ")";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                gi = UiDataManager.getInstance().GetGroupByToken(targetid);
                if (gi != null)
                {
                    who = gi.name;
                }
                modeTxt = "指导模式-指导组 (" + who + ")";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                //   pi = UiDataManager.getInstance().GetPlayerById(Convert.ToInt64(targetid));
                //    if (pi != null)
                //    {
                //        who = pi.name;
                //    }
                modeTxt = "自主训练模式-独立";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
                //         gi = UiDataManager.getInstance().GetGroupByToken(targetid);
                //          if (gi != null)
                //           {
                //              who = gi.name;
                //           }
                modeTxt = "自主训练模式-小组";
                break;
            case ComonEnums.TeachingMode.SelfTrain_All:
                //        gi = UiDataManager.getInstance().GetGroupByToken(targetid);
                //         if (gi != null)
                //        {
                //            who = gi.name;
                //        }
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

        if (msgShow != null)
        {
            msgShow.ShowMessage(modeTxt);
        }

        if (IsTeacher)
        {
            // 老师界面 状态改变 
            stateinfor.GetComponent<StateInforShow>().SetModeText(modeTxt);
        }
        else
        {
            // 学生状态改变 
            studentstate.ChangeState(modeTxt);
        }

        Debug.Log(modeTxt + " -- 模式改变");
    }

    // 选择了某个试题对应的弹出相应界面
    public void ChooseQuestion(ComonEnums.InClassTestType catage, ComonEnums.QuestionType typ, int questionid)
    {
        if (!IsTeacher)
        {
            return;
        }

        switch (catage)
        {
            case ComonEnums.InClassTestType.Test:
                //测试反馈
                questionback.ShowSelf(questionid, typ);

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case ComonEnums.InClassTestType.Ask:
                // 学生选择界面
                // TODO
                ShowFirstUI(false);

                players.ShowSelf(null, "请选择你要提问的学生:");
                handuplist.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                fastquestion.HideSelf();
                questionback.HideSelf();
                break;
            case ComonEnums.InClassTestType.Fast:
                // 抢答学生界面
                //TODO
                fastquestion.ShowSelf(questionid);

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            default:
                break;
        }
    }



    // 学生点赞 举手
    public void SendLike()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        UiDataManager.getInstance().SendLike();
    }

    public void SendDoubt()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        UiDataManager.getInstance().SendDoubt();
    }

    // 中间的按钮模块
    // 测试按钮
    public void TestBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        fastquestion.HideSelf();
        handuplist.HideSelf();

        ShowFirstUI(false);
    }

    public void TestBtn(bool isopen = true)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!isopen)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        fastquestion.HideSelf();
        handuplist.HideSelf();

        ShowFirstUI(false);
    }
    // 抢答按钮
    public void FastBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf(ComonEnums.InClassTestType.Fast);

        fastquestion.HideSelf();
        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        handuplist.HideSelf();
        ShowFirstUI(false);

    }

    //提问按钮
    public void AskBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf(ComonEnums.InClassTestType.Ask);

        fastquestion.HideSelf();
        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        handuplist.HideSelf();
        ShowFirstUI(false);
    }

    //返回大厅 按钮点击监听
    public void BackLobby()
    {
        // 返回大厅
    }

    public void BackToLobby(Int64 userid)
    {
        Application.Quit();
    }

    // 重置场景
    public Dropdown reset;
    public void ResetScene()
    {
        if (!IsTeacher)
        {
            return;
        }

        if (reset == null)
        {
            return;
        }

        switch (reset.value)
        {
            case 0:
                // 重置全部
                UiDataManager.getInstance().ResetSceneAll();

                players.HideSelf();
                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case 1:
                // 重置组
                // 显示组选择界面
                UiDataManager.getInstance().ResetScene(1);

                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case 2:
                // 重置学生
                // 显示所有学生界面
                UiDataManager.getInstance().ResetScene(2);

                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            default:
                break;
        }
    }

    public void ResetSceneBack(Int64 userid)
    {
        SceneManager.LoadScene(0);
    }
}
