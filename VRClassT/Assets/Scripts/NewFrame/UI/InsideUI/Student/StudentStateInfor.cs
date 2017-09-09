using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentStateInfor : OutUIBase {

    public Text modeTxt;
    public Text teacherTxt;

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
