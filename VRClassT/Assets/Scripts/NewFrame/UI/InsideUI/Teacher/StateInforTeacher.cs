using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StateInforTeacher : OutUIBase {

    public GameObject headprefab;
    public GameObject stateuibg;
    public Transform headlist;
    public Text modeTxt;
    public Text rateTxt;
    public Text likeTxt;

    private int likecount = 0;

    public MsgTips msgShow;

    // Use this for initialization
    void Start () {
		
	}

    public override void ShowSelf(params object[] args)
    {
        Debug.Log("显示");
        InitData();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    void OnEnable()
    {
        InitData();
        RegisterListener();
    }

    void OnDisable()
    {
        UnRegisterListener();
    }

    private bool _isreglistener = false;
    public void RegisterListener()
    {
        if (_isreglistener)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.StudentLike, ReceiveLike);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.StudentDoubt, ReceiveDoubt);

        _isreglistener = true;
    }

    public void UnRegisterListener()
    {
        if (!_isreglistener)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.StudentLike, ReceiveLike);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.StudentDoubt, ReceiveDoubt);

        _isreglistener = false;
    }

    public void InitData()
    {
        if (modeTxt != null)
        {
            modeTxt.text = UIManager.getInstance().modeTxt;
        }

        if (likeTxt != null)
        {
            likeTxt.text = UiDataManager.getInstance().GetLikeCount();
        }

        if (rateTxt != null)
        {
            rateTxt.text = UiDataManager.getInstance().CalculateTheAttendance();
        }

        Debug.Log(likeTxt);
        Debug.Log(UIManager.getInstance().modeTxt);
    }

    public void ChangeModeText(string txt)
    {
        if(modeTxt == null)
        {
            return;
        }

        modeTxt.text = txt;
    }

    public void ChangeLikeCount(int count)
    {
        if (likeTxt == null)
        {
            return;
        }

        likeTxt.text = count.ToString();
    }

    public void ChangeRateCount(string rate)
    {
        if (rateTxt == null)
        {
            return;
        }

        rateTxt.text = rate;
    }

    public void AddDoubtPerson(int userid)
    {
        PlayerInfor p = ClassManager.getInstance().FindPlayerById(userid);

        if (p == null)
        {
            return;
        }

        if (headlist == null)
        {
            return;
        }

        int count = headlist.childCount;
        bool isinit = false;
        Transform c = null;
        for (int i = 0; i < count; i++)
        {
            c = headlist.GetChild(i);
            if (c != null && !c.gameObject.activeSelf)
            {
                c.GetComponent<PlayerIcon>().Init(p, null);
                isinit = true;
                break;
            }
        }

        if (!isinit)
        {
            if (headprefab == null)
            {
                return;
            }

            GameObject icon = GameObject.Instantiate(headprefab, headlist);
            icon.GetComponent<PlayerIcon>().Init(p, null);

            if (!icon.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(true);
            }
        }
    }

    //接收学生点赞 举手
    public void ReceiveLike(int userid, ArrayList msg)
    {
        PlayerInfor p = ClassManager.getInstance().FindPlayerById(userid);
        if (p != null)
        {
            string modeTxt = p.name + "给您点了赞";
            if (msgShow != null)
            {
                msgShow.ShowMessage(modeTxt);
            }
        }

        ChangeLikeCount(++likecount);
    }

    public void ReceiveDoubt(int userid, ArrayList msg)
    {
        //// 显示有疑问图标
        //if (flashing != null && !flashing.activeSelf)
        //{
        //    flashing.SetActive(true);
        //}

        // 添加有疑问的学生进入列表
        // TODO !!!
        AddDoubtPerson((int)userid);

        //// 添加举手回答问题的学生
        //if (handuplist != null && handuplist.gameObject.activeInHierarchy)
        //{
        //    handuplist.AddStudent((int)userid);
        //}

    }

    // 举手之类的信息提醒： ui闪烁
    public void FlashingState()
    {
    }

    //public void ShowInfor()
    //{
    //    stateuibg.transform.DOLocalMoveX(moveto.x,2);
    //}

    //public void HideInfor()
    //{
    //    stateuibg.transform.DOLocalMoveX(movefrom.x, 2);
    //}

    //public void OnClick()
    //{
    //    _isshowinfor = !_isshowinfor;

    //    if(_isshowinfor)
    //    {
    //        ShowInfor();
    //    }
    //    else
    //    {
    //        HideInfor();
    //    }
    //}
}
