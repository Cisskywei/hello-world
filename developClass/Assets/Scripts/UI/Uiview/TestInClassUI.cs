using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInClassUI : uibase {

    public GameObject questionprefab;

    public Transform questionlistpanel;

    public List<QuestionInfor> questionlist;

    private bool _isInitQuestion = false;  // 标记是否已经初始化题目

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void ShowSelf()
    {
        InitQuestionItem();

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
    }

    public void InitQuestionItem()
    {
        if(_isInitQuestion)
        {
            return;
        }

        // 获取试题信息
        questionlist = QuestionManager.getInstance().GetQuestionList(Enums.InClassTestType.Test);

        if(questionlist == null || questionlist.Count <= 0)
        {
            return;
        }

        int count = 0;
        int tip = 0;
        if (questionlistpanel != null)
        {
            count = questionlistpanel.childCount;
        }

        foreach (QuestionInfor g in questionlist)
        {
            if (tip < count)
            {
                Transform icon = questionlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<QuestionItem>().InitQuestion(g.content, g.typ, g._id);
                    if (!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
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
                icon.GetComponent<QuestionItem>().InitQuestion(g.content, g.typ, g._id);
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

        _isInitQuestion = true;
    }
}
