using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StateInforTeacher : uibase {

    public GameObject headprefab;
    public GameObject stateuibg;
    public Transform headlist;
    public Text modeTxt;
    public Text rateTxt;
    public Text likeTxt;

    private int likecount;

    //[SerializeField]
    //public Vector3 movefrom;
    //[SerializeField]
    //public Vector3 moveto;

    //private bool _isshowinfor = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
	//}

    public void ShowSelf()
    {
        Debug.Log("显示");
        InitData();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        //if (_isshowinfor)
        //{
        //    HideInfor();
        //}
    }

    private void OnEnable()
    {
        InitData();
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
        PlayerInfor p = UiDataManager.getInstance().GetPlayerById(userid);

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
