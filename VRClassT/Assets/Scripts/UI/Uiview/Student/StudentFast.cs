using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class StudentFast : OutUIBase {

    public FlowUI flow;

    public Text stem;

    public int questionid;

    // Use this for initialization
    void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

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
        MsgModule.getInstance().reqAnswerFastQuestion(this.questionid);
   //     EventDispatcher.GetInstance().MainEventManager.TriggerEvent < int,int>(EventId.StudentFastQuestion, 1, this.questionid);
    }

    public void SetPosition()
    {
        if (flow != null)
        {
            flow.ChangeMove(false);
        }
    }
}
