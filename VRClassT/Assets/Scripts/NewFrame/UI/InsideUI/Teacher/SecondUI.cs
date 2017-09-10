using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondUI : OutUIBase
{
    public enum UISecondType
    {
        None = -1,

        TeacherInfor,
    }

    public OutUIBase[] uilist;

    public override void ShowSelf(params object[] args)
    {
        if (args != null && args.Length > 0)
        {
            UISecondType f = (UISecondType)args[0];
            ShowUI(f);
        }
        else
        {
            ShowUI(UISecondType.TeacherInfor);
        }
        base.ShowSelf(args);
    }

    public void ShowUI(UISecondType id, params System.Object[] args)
    {
        if (uilist == null || uilist.Length <= 0)
        {
            return;
        }

        int uiid = (int)id;
        if (uiid > uilist.Length)
        {
            return;
        }

        for (int i = 0; i < uilist.Length; i++)
        {
            if (i == uiid)
            {
                uilist[i].ShowSelf(args);
            }
            else
            {
                uilist[i].HideSelf();
            }
        }
    }
}
