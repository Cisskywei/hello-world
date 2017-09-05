using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;
using ko.NetFram;
using System;

public class TestFeedBackUI : OutUIBase
{

    // 控件相关
    public Text content;
    public Text typ;
    public Text online;
    public Text takepart;
    public Text nottakepart;
    public Text correct;
    public Text error;
    public Text timedown; // 倒计时
    [SerializeField]
    public Text[] option;
    [SerializeField]
    public GameObject[] optionimage; // 正确选项的图片

    // 反馈时的题目信息界面
    public TestFeedBackUpUI upquestion;

    // 数据相关
    public int questionid;
    public ComonEnums.QuestionType questiontyp = ComonEnums.QuestionType.SingleChoice;
    public ArrayList correctoption;

    // 控件显示数据
    private int _online;
    private int _takepart;
    private int _nottakepart;
    private int _correct;
    private int _error;
    private int[] _option = new int[4];

 //   private TimeCountDown _timer = new TimeCountDown();

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
	//	//if(_timer != null)
 // //      {
 // //          // 计时器
 // //          _timer.loop(Time.deltaTime);

 // //          // 用于显示
 // //          //_timer.GetLeftTime();
 // //      }
	//}

    // 倒计时
    public void StartTimeCutDown(int lefttime = 50)
    {
        //if(_timer == null)
        //{
        //    _timer = new TimeCountDown();
        //}

        //if(_timer.IsTimeGoing())
        //{
        //    return;
        //}
        //_timer.OnTimeOver(timeOver);
        //_timer.StartTimer(lefttime);
    }

    private void timeOver()
    {
        // 时间倒计时结束
        // 向服务器公布题目答案
    }

    public override void ShowSelf(params System.Object[] args)
    {
        ShowSelf(this.questionid, this.questiontyp);

    }

    // 初始化界面
    public void ShowSelf(int questionid, ComonEnums.QuestionType typ = ComonEnums.QuestionType.SingleChoice)
    {
        if(typ == ComonEnums.QuestionType.ShortAnswer || typ == ComonEnums.QuestionType.TrueOrFalse)
        {
            return;
        }

        Init(questionid,typ);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        StartTimeCutDown();

    }

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init(int questionid, ComonEnums.QuestionType typ = ComonEnums.QuestionType.SingleChoice)
    {
        QuestionInfor qi = QuestionManager.getInstance().GetQuestionById(questionid);

        if(qi == null)
        {
            return;
        }

        this.questionid = qi.question_id;
        this.content.text = qi.stem;
        this.typ.text = QuestionManager.ConvertQuestionType(qi.typ);

        showRightOptionImage(qi.correctanswers);

        this.correctoption = qi.correctanswers;

        // 需要动态改变的数值
        // TODO
        _online = UiDataManager.getInstance().studentonlinecount;
        online.text = _online.ToString();

        takepart.text = "0";
        nottakepart.text = _online.ToString();
        correct.text = "0";
        error.text = "0";

        for(int i=0;i< this.option.Length;i++)
        {
            this.option[i].text = "0 人";
        }

        // 初始化上方ui
        if (upquestion != null)
        {
            upquestion.ShowSelf(qi.stem, qi.options, qi.correctanswers);
        }
    }

    //
    // 显示正确答案的对号
    private void showRightOptionImage(int rightid)
    {
        if(rightid < 0 || rightid > optionimage.Length)
        {
            return;
        }

        for(int i=0;i<optionimage.Length;i++)
        {
            if(i == rightid)
            {
                optionimage[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                optionimage[i].GetComponent<Image>().enabled = false;
            }
        }
    }

    private void showRightOptionImage(ArrayList rightid)
    {
        if(rightid == null || rightid.Count <= 0)
        {

        }

        for (int i = 0; i < optionimage.Length; i++)
        {
            if (rightid.Contains(i))
            {
                optionimage[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                optionimage[i].GetComponent<Image>().enabled = false;
            }
        }
    }

    void OnEnable()
    {
        RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEventListener();
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int, int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    public void TestFeedBack(int userid, int questionid, int optionid)
    {
        // 答题返回
        if(correctoption.Contains(optionid))
        {
            // 答题正确
            _correct++;

            correct.text = _correct.ToString();
        }
        else
        {
            _error++;

            error.text = _error.ToString();
        }
        _takepart++;
        _nottakepart = _online - _takepart;
        _option[optionid]++;

        takepart.text = _takepart.ToString();
        nottakepart.text = _nottakepart.ToString();
        this.option[optionid].text = _option[optionid].ToString() + " 人";
    }

    // 确认返回
    public void Yes()
    {
        HideSelf();

        Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);

   //     UIManager.getInstance().ShowFirstUI(true);
    }

    public void No()
    {
        HideSelf();

        Teaching.getInstance().ShowUI(Teaching.UITeaching.CourseContent);

        //     UIManager.getInstance().ShowFirstUI(true);
    }
}
