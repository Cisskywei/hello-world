using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class FastInClassUI : OutUIBase {

    public Text stem;
    public PlayerIcon one;
    public PlayerIcon teacher;

    public int questionid;
    public int chooseuserid = -1;

    //   // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }


    public override void ShowSelf(params System.Object[] args)
    {
        ShowSelf(this.questionid);

        if (args != null && args.Length > 0)
        {
            chooseuserid = (int)args[0];
            SomeOneCome(chooseuserid);
        }
    }

    // 初始化界面
    public void ShowSelf(int questionid)
    {
        QuestionInfor qi = QuestionManager.getInstance().GetQuestionById(questionid);
        if(qi != null)
        {
            stem.text = qi.stem;
        }
        Debug.Log(UserInfor.getInstance().UserName + UserInfor.getInstance().avatar);
        // 显示老师自己头像
        teacher.GetComponent<PlayerIcon>().Init(UserInfor.getInstance().UserName, UserInfor.getInstance().avatar);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

    }

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void SomeOneCome(int userid)
    {
        PlayerInfor p = UiDataManager.getInstance().GetPlayerById(userid);

        if(p != null)
        {
            chooseuserid = userid;

            one.Init(p.avatar,p.name);
        }
    }

    private void EndFastQuestion(int userid)
    {
        if(UserInfor.getInstance().isTeacher)
        {
            Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);
        }
        else
        {
            StudentUIManager.getInstance().ShowUI(StudentUIManager.UIStudent.Prepare);
        }

        chooseuserid = -1;
    }

    public void Yes()
    {
        HideSelf();

        if(chooseuserid != -1)
        {
            MsgModule.getInstance().reqEndFastQuestion((Int64)chooseuserid);
            chooseuserid = -1;
        }

        //       UIManager.getInstance().ShowFirstUI(true);
    }

    public void No()
    {
        HideSelf();

        if (chooseuserid != -1)
        {
            MsgModule.getInstance().reqEndFastQuestion((Int64)chooseuserid);
            chooseuserid = -1;
        }

        //      UIManager.getInstance().ShowFirstUI(true);
    }
}
