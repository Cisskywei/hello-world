using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class Member : OutUIBase
{
    public enum GroupRange
    {
        Classes = 0,
        All,
    }

    public enum GroupRule
    {
        Grade = 0,
        Random,
    }

    public GroupRange range = GroupRange.Classes;
    public GroupRule rule = GroupRule.Random;

    public Text memberTxt;
    private string cout = "({0}人)";
    private int membercount = 0;

    public GameObject iconPrafab;

    public Transform listpanel;

    public bool forceupdate = false;
    private bool isinit = false;

    // Use this for initialization
    void Start () {
		
	}

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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ChooseClass, this.ChooseClass);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ChooseClass, this.ChooseClass);
    }

    public void ChooseClass(int id)
    {
        // 查看详细信息
        ClassPrepare.getInstance().ShowUI(ClassPrepare.UIPrepare.ClassPersonInfor,new System.Object[] { id });
    }

    public override void ShowSelf(params System.Object[] args)
    {
        InitData();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void InitData()
    {
        if (!(forceupdate || !isinit))
        {
            return;
        }

        Dictionary<int, ClassInfor> classlist = ClassManager.getInstance().GetAllClass();

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (classlist == null || classlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        foreach (ClassInfor c in classlist.Values)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<ClassIcon>().Init(c);

                    if (!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (iconPrafab == null)
                {
                    return;
                }

                GameObject icon2 = GameObject.Instantiate(iconPrafab, listpanel);
                icon2.GetComponent<ClassIcon>().Init(c);
            }
            tip++;

            membercount += c.Count;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                GameObject icon3 = listpanel.GetChild(i).gameObject;
                if (icon3.activeSelf)
                {
                    icon3.SetActive(false);
                }
            }
        }

        ShowMember();

        isinit = true;
    }

    public void ShowMember()
    {
        if(memberTxt == null)
        {
            return;
        }

        memberTxt.text = string.Format(cout, membercount);
    }

    public void OnToggleClass(Toggle tog)
    {
        if(tog.isOn)
        {
            range = GroupRange.Classes;
        }
    }

    public void OnToggleAll(Toggle tog)
    {
        if (tog.isOn)
        {
            range = GroupRange.All;
        }
    }

    public void OnToggleGrade(Toggle tog)
    {
        if (tog.isOn)
        {
            rule = GroupRule.Grade;
        }
    }

    public void OnToggleRandom(Toggle tog)
    {
        if (tog.isOn)
        {
            rule = GroupRule.Random;
        }
    }

    // 请求分组操作
    private void RequestGroupingOperation()
    {

    }
}
