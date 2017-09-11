using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class StudentFast : OutUIBase {

    public FlowUI flow;

    public Text stem;

    public int questionid;

    public override void ShowSelf(params System.Object[] args)
    {
        if(args != null && args.Length > 0)
        {
            this.questionid = (int)args[0];
        }

        ShowSelf(this.questionid, null);
    }

    public void ShowSelf(int id, string other)
    {
        SetPosition();

        initQuestion(id);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Quaternion r = gameObject.GetComponent<RectTransform>().rotation;
        r.x = 0;
        r.z = 0;
        gameObject.GetComponent<RectTransform>().rotation = r;
    }

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void initQuestion(int id)
    {
        QuestionInfor q = QuestionManager.getInstance().GetQuestionById(id);

        if (q == null)
        {
            return;
        }

        stem.text = q.stem;

        questionid = q.question_id;
    }

    public void GetFastAnswer()
    {
        // 点击答题按钮
        // 显示抢答页面
        //HideSelf();
        ArrayList msg = new ArrayList();
        msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
        msg.Add((Int64)CommandDefine.SecondLayer.TestInClass);
        msg.Add((Int64)this.questionid);
        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, msg);

        //MsgModule.getInstance().reqAnswerFastQuestion(this.questionid);
   //     EventDispatcher.GetInstance().MainEventManager.TriggerEvent < int,int>(EventId.StudentFastQuestion, 1, this.questionid);
    }

    public void SetPosition()
    {
        if (flow != null)
        {
            flow.ChangeMove(false);
        }
    }

    void OnEnable()
    {
        RegListener();
    }

    void OnDisable()
    {
        RemoveListener();
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

        Int64 typ = (Int64)msg[2];

        if(typ == 1)
        {
            // 得到答题权
        }
        else if(typ == 2)
        {
            // 答题结束
            HideSelf();
            
            if(StudentUI.getInstance()!=null)
            {
                StudentUI.getInstance().HideSelf();
            }

            if (StudentUIManager.getInstance() != null)
            {
                StudentUIManager.getInstance().ShowUI(StudentUIManager.UIStudent.Prepare);
            }
        }
    }
}
