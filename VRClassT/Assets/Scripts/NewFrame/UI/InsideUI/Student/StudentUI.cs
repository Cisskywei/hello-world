using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentUI : OutUIBase {

    public enum UILeft
    {
        None = -1,

        StudentTest,
        StudentFast,
        StudentFastGet,

        Max,
    }

    public enum UIRight
    {
        None = -1,
        MsgTips,
    }

    public OutUIBase[] uilistleft;
    public OutUIBase[] uilistright;

    public void ShowUILeft(UILeft id, params System.Object[] args)
    {
        if (uilistleft == null || uilistleft.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilistleft.Length)
        {
            return;
        }

        for (int i = 0; i < uilistleft.Length; i++)
        {
            if (i == uiid)
            {
                uilistleft[i].ShowSelf(args);
            }
            else
            {
                uilistleft[i].HideSelf();
            }
        }
    }

    public void ShowUIRight(UIRight id, params System.Object[] args)
    {
        if (uilistright == null || uilistright.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilistright.Length)
        {
            return;
        }

        for (int i = 0; i < uilistright.Length; i++)
        {
            if (i == uiid)
            {
                uilistright[i].ShowSelf(args);
            }
            else
            {
                uilistright[i].HideSelf();
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

        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.OpenUI, this.OpenUI);

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

    // 接受题目
    public void StudentReciveQuestion(int userid, int questiontyp, int questionid, string other)
    {
        //if(questionid < 0)
        //{
        //    // 抢答题
        //    ReciveQuestion((ComonEnums.InClassTestType)questiontyp, (ComonEnums.QuestionType)questiontyp, questionid, other);
        //}
        //else if(questionid == 16)
        //{
        //    // 提问 显示点赞等的界面
        //    if (!first01.activeSelf)
        //    {
        //        first01.SetActive(true);
        //    }
        //}
        //else
        //{
        //    ReciveQuestion(ComonEnums.InClassTestType.Test, ComonEnums.QuestionType.SingleChoice, questionid, other);
        //}

        ReciveQuestion((ComonEnums.InClassTestType)questiontyp, questionid, other);
    }
    public void ReciveQuestion(ComonEnums.InClassTestType catage, int id, string other)
    {
        if (IsTeacher)
        {
            return;
        }

        // 学生
        switch (catage)
        {
            case ComonEnums.InClassTestType.Test:
                studenttest.ShowSelf(id, other);

                studentfast.HideSelf();
                break;
            case ComonEnums.InClassTestType.Ask:
                break;
            case ComonEnums.InClassTestType.Fast:
                studentfast.ShowSelf(id, other);

                studenttest.HideSelf();
                break;
            default:
                break;
        }
    }

    // 学生操作监听
    public void StudentAnswerQuestion(int id)
    {

    }

    public void StudentFastQuestion(int userid, int questionid)
    {
        if (studentfast != null)
        {
            studentfast.HideSelf();
        }
        if (studentfastget != null && ((userid == (int)(UserInfor.getInstance().UserId)) || UserInfor.getInstance().isTeacher))
        {
            studentfastget.ShowSelf(questionid);
        }
    }
}
