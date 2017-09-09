using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class StudentFastGet : OutUIBase {

    public FlowUI flow;

    public Text stem;

    public PlayerIcon student;
    public PlayerIcon teacher;

    public int questionid;
    public string teachername = string.Empty;

    // Use this for initialization
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

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.EndFastQuestion, this.EndFastQuestion);
    }

    private void EndFastQuestion(int userid)
    {
        if(userid != (int)UserInfor.getInstance().UserId)
        {
            return;
        }

        StudentUIManager.getInstance().ShowUI(StudentUIManager.UIStudent.Prepare);
    }

    public override void ShowSelf(params System.Object[] args)
    {
        if (args != null )
        {
            if(args.Length > 1)
            {
                this.questionid = (int)args[0];
                this.teachername = (string)args[1];

                teacher.Init(teachername, null);
            }
            else if(args.Length > 0)
            {
                this.questionid = (int)args[0];
            }
        }

        ShowSelf(this.questionid);
    }

    public void ShowSelf(int questionid)
    {
        initQuestion(questionid);

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
        if(id < 0)
        {
            return;
        }

        QuestionInfor q = QuestionManager.getInstance().GetQuestionById(id);

        if (q == null)
        {
            return;
        }

        stem.text = q.stem;
    }

    public void AnswerEnd()
    {
        this.teachername = string.Empty;
        this.questionid = -1;
        HideSelf();
    }

    public void SetPosition()
    {
        if (flow != null)
        {
            flow.ChangeMove(false);
        }
    }
}
