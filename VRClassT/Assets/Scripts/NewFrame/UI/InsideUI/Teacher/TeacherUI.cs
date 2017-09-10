using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class TeacherUI : OutUIBase {

    public enum UILeft
    {
        None = -1,

        First,
        Second,

        Max,
    }

    public enum UIRight
    {
        None = -1,
        MsgTips,
    }

    public OutUIBase[] uilistleft;
    public OutUIBase[] uilistright;

    public override void ShowSelf(params object[] args)
    {
        OpenUI();
        base.ShowSelf(args);
    }

    public void ShowUILeft(UILeft id, params System.Object[] args)
    {
        if (uilistleft == null || uilistleft.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilistleft.Length)
        {
            return;
        }

        for (int i = 0; i < uilistleft.Length; i++)
        {
            if (i == uiid)
            {
                uilistleft[i].ShowSelf(args);
            }
            else
            {
                uilistleft[i].HideSelf();
            }
        }
    }

    public void HideUILeftAll()
    {
        if (uilistleft == null || uilistleft.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < uilistleft.Length; i++)
        {
            uilistleft[i].HideSelf();
        }
    }

    public void ShowUIRight(UIRight id, params System.Object[] args)
    {
        if (uilistright == null || uilistright.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilistright.Length)
        {
            return;
        }

        for (int i = 0; i < uilistright.Length; i++)
        {
            if (i == uiid)
            {
                uilistright[i].ShowSelf(args);
            }
            else
            {
                uilistright[i].HideSelf();
            }
        }
    }

    public void HideUIRightAll()
    {
        if (uilistright == null || uilistright.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < uilistright.Length; i++)
        {
            uilistright[i].HideSelf();
        }
    }

    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.OpenUI, this.OpenUI);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.OpenUI, this.OpenUI);
    }

    public int _isopenui = 0;
    private int uiorder = -1;
    private UILeft _uisort = UILeft.First;
    private void OpenUI()
    {
        uiorder = (int)_uisort;
        uiorder = (++uiorder) % (int)UILeft.Max;

        uiorder -= 1;
        for (int i = 0; i < uilistleft.Length; i++)
        {
            if (i == uiorder)
            {
                uilistleft[i].ShowSelf(new System.Object[] { 0 });
            }
            else
            {
                uilistleft[i].HideSelf();
            }
        }

        _uisort = (UILeft)(++uiorder);

        if (_uisort == UILeft.Max || _uisort == UILeft.None)
        {
            // 关闭所有界面
            HideUILeftAll();
            HideUIRightAll();
        }
        else
        {
            ShowUIRight(UIRight.MsgTips);
        }
    }
}
