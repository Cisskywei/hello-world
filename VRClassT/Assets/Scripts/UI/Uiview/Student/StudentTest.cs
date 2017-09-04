using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class StudentTest : OutUIBase {

    public FlowUI flow;

    public Text stem;
    public Text[] options;

    public int questionid;

    // Use this for initialization
    void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

    public override void ShowSelf(params System.Object[] args)
    {
        ShowSelf(this.questionid);
    }

    public void ShowSelf(int id, string other = null)
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

        if(q == null)
        {
            return;
        }

        stem.text = q.stem;

        for(int i=0; i<q.anwers.Length; i++)
        {
            if(options[i]!=null)
            {
                options[i].text = q.anwers[i];
            }
        }
    }

    public void ChooseQuestion(int optionid)
    {
        // 弹出回答问题界面
        HideSelf();
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.StudentAnswerQuestion, optionid);

        MsgModule.getInstance().reqAnswerQuestion(this.questionid, optionid);
//        UiDataManager.getInstance().AnswerQuestionTest(optionid);
    }

    public void SetPosition()
    {
        if (flow != null)
        {
            flow.ChangeMove(false);
        }
    }
}
