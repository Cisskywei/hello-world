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

    void OnEnable()
    {
        RegListener();
        RegisterEvent();
    }

    void OnDisable()
    {
        RemoveListener();
        UnRegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }

    // 注册 取消 网络消息监听模块
    private void RegListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
    }

    private void RemoveListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
    }

    private void TestInClass(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 questionid = (Int64)msg[2];

        SomeOneCome(userid);
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
        PlayerInfor p = ClassManager.getInstance().FindPlayerById(userid);

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
            if (Teaching.getInstance() != null)
            {
                Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);
            }

            if (TeacherUI.getInstance() != null)
            {
                TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
            }
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
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.PushDataOne);
            msg.Add((Int64)chooseuserid);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, msg);

            //MsgModule.getInstance().reqEndFastQuestion((Int64)chooseuserid);
            chooseuserid = -1;
        }

        if (Teaching.getInstance() != null)
        {
            Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);
        }

        if (TeacherUI.getInstance() != null)
        {
            TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
        }
    }

    public void No()
    {
        HideSelf();

        if (chooseuserid != -1)
        {
   //         MsgModule.getInstance().reqEndFastQuestion((Int64)chooseuserid);
            chooseuserid = -1;
        }

        if (Teaching.getInstance() != null)
        {
            Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);
        }

        if (TeacherUI.getInstance() != null)
        {
            TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
        }
    }
}
