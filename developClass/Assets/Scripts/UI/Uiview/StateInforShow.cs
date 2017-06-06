using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateInforShow : MonoBehaviour {

    public Text modeTxt;
    public Text secondTxt;
    public Text threeTxt;

    public void SetModeText(string txt)
    {
        modeTxt.text = txt;
    }

    public void SetTeacherText(string txt)
    {
        if(UserInfor.getInstance().isTeacher)
        {
            return;
        }

        secondTxt.text = txt;
    }

    public void SetLikeText(string txt)
    {
        if (!UserInfor.getInstance().isTeacher)
        {
            return;
        }

        threeTxt.text = txt;
    }

    public void SetRateText(string txt)
    {
        if (!UserInfor.getInstance().isTeacher)
        {
            return;
        }

        secondTxt.text = txt;
    }
}
