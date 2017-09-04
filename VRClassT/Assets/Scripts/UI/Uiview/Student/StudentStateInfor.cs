using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentStateInfor : uibase {

    public Text modeTxt;
    public Text teacherTxt;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 提供外部调用显示接口
    public void ShowSelf()
    {
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

    // 显示信息
    public void ChangeState(string txt)
    {
        if(txt == null)
        {
            return;
        }

        if(modeTxt == null)
        {
            return;
        }

        modeTxt.text = txt;
    }
}
