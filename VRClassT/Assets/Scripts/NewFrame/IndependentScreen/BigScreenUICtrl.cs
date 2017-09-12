using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigScreenUICtrl : MonoBehaviour {

    public OutUiManager outui;
    public ClassPrepare cp;
    public Member m;
    public MemberInfor mi;
    public Push p;
    public PushInfor pi;
    public Teaching t;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitUICtrl()
    {
        // 大量的初始化代码
        if(outui != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.ChoseCourse, outui.ListenerBigScreen);
        }

        if(cp != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.ClassPrepare, cp.ListenerBigScreen);
        }

        if (m != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.Member, m.ListenerBigScreen);
        }

        if (mi != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.MemberInfor, mi.ListenerBigScreen);
        }

        if (p != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.Push, p.ListenerBigScreen);
        }

        if (pi != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.PushInfor, pi.ListenerBigScreen);
        }

        if (t != null)
        {
            BigScreen.getInstance().AddUICtrlListener(BigScreen.ScreenUIOrder.Teaching, t.ListenerBigScreen);
        }
    }
}
