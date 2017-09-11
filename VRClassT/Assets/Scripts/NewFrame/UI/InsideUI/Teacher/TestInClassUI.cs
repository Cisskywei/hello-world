using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInClassUI : OutUIBase {

    public GameObject questionprefab;

    public Transform questionlistpanel;

    public Dictionary<int, QuestionInfor> questionlist;

    // 提问显示语音
    public GameObject voicepanel;

    // 大厅界面一定要先设置ComonEnums.InClassTestType
    public ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test;

    // Use this for initialization
    void Start () {
		
	}

    public override void ShowSelf(params System.Object[] args)
    {
        if (!transform.parent.gameObject.activeSelf)
        {
            transform.parent.gameObject.SetActive(true);
        }

        if(args != null && args.Length > 0)
        {
            this.catage = (ComonEnums.InClassTestType)args[0];
        }
        else
        {
            catage = ComonEnums.InClassTestType.Test;
        }

        InitQuestionItem(this.catage);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void ShowSelf(ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test)
    {
        if(!transform.parent.gameObject.activeSelf)
        {
            transform.parent.gameObject.SetActive(true);
        }

        InitQuestionItem(catage);

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

    public void InitQuestionItem(ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test)
    {
        switch(catage)
        {
            case ComonEnums.InClassTestType.Ask:
                if(voicepanel != null && !voicepanel.activeSelf)
                {
                    voicepanel.SetActive(true);
                }
                break;
            default:
                if (voicepanel != null && voicepanel.activeSelf)
                {
                    voicepanel.SetActive(false);
                }
                break;
        }

        this.catage = catage;

        // 获取试题信息
        questionlist = QuestionManager.getInstance().GetQuestionList();

        int count = 0;
        int tip = 0;
        if (questionlistpanel != null)
        {
            count = questionlistpanel.childCount;
        }

        if (questionlist == null || questionlist.Count <= 0)
        {
            return;
        }

        foreach (QuestionInfor g in questionlist.Values)
        {
            if (tip < count)
            {
                Transform icon = questionlistpanel.GetChild(tip);
                if (icon)
                {
                    QuestionItem qt = icon.GetComponent<QuestionItem>();
                    if(qt != null && !qt.isvoice)
                    {
                        qt.InitQuestion(g, catage);
                        if (!icon.gameObject.activeSelf)
                        {
                            icon.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (questionprefab == null)
                {
                    return;
                }

                GameObject icon = GameObject.Instantiate(questionprefab, questionlistpanel);
                icon.GetComponent<QuestionItem>().InitQuestion(g, catage);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = questionlistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    // 确认返回
    public void Yes()
    {
        HideSelf();

        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }

    public void No()
    {
        HideSelf();

        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }
}
