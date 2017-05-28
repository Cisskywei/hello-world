using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;
using ko.NetFram;
using System;

public class TestFeedBackUI : uibase {

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

    // 数据相关
    public int questionid;
    public int correctoption;

    // 控件显示数据
    private int _online;
    private int _takepart;
    private int _nottakepart;
    private int _correct;
    private int _error;
    private int[] _option = new int[4];

    private TimeCountDown _timer = new TimeCountDown();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(_timer != null)
        {
            // 计时器
            _timer.loop(Time.deltaTime);

            // 用于显示
            //_timer.GetLeftTime();
        }
	}

    // 倒计时
    public void StartTimeCutDown(int lefttime = 50)
    {
        if(_timer == null)
        {
            _timer = new TimeCountDown();
        }

        if(_timer.IsTimeGoing())
        {
            return;
        }
        _timer.OnTimeOver(timeOver);
        _timer.StartTimer(lefttime);
    }

    private void timeOver()
    {
        // 时间倒计时结束
        // 向服务器公布题目答案
    }

    // 初始化界面
    public void ShowSelf(int questionid, Enums.InClassTestType typ = Enums.InClassTestType.Test)
    {
        Init(questionid,typ);
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        StartTimeCutDown();
    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init(int questionid, Enums.InClassTestType typ = Enums.InClassTestType.Test)
    {
        QuestionInfor qi = QuestionManager.getInstance().GetQuestionByTypId(typ, questionid);

        if(qi == null)
        {
            return;
        }

        this.questionid = qi._id;
        this.correctoption = qi.answerid;

        this.content.text = qi.content;
        this.typ.text = QuestionManager.ConvertQuestionType(qi.typ);

        showRightOptionImage(qi.answerid);

        // 需要动态改变的数值
        // TODO
        _online = UiDataManager.getInstance().studentonlinecount;
        online.text = _online.ToString();
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
                if(!optionimage[i].activeSelf)
                {
                    optionimage[i].SetActive(true);
                }
            }
            else
            {
                if (optionimage[i].activeSelf)
                {
                    optionimage[i].SetActive(false);
                }
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    public void TestFeedBack(int option)
    {
        // 答题返回
        if(option == correctoption)
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
        _option[option]++;

        takepart.text = _takepart.ToString();
        nottakepart.text = _nottakepart.ToString();
        this.option[option].text = _option[option].ToString();
    }
}
