using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushInfor : OutUIBase
{
    public GameObject iconPrafab;

    public Transform listpanel;

    public int classid = -1;

    public override void ShowSelf(params object[] args)
    {
        int id = -1;
        if (args != null && args[0] != null)
        {
            id = (int)args[0];
        }

        InitData(id);

        base.ShowSelf(args);
    }

    public void InitData(int classid = -1)
    {
        if (classid < 0)
        {
            classid = this.classid;
        }

        if (classid < 0)
        {
            return;
        }

        ClassInfor ci = ClassManager.getInstance().GetClassById(classid);
        Dictionary<int, PlayerInfor> playerlist = ci.playerlist;

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (playerlist == null || playerlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        foreach (PlayerInfor c in playerlist.Values)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PlayerDownLoadIcon>().Init(c);
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

                GameObject icon = GameObject.Instantiate(iconPrafab, listpanel);
                icon.GetComponent<PlayerDownLoadIcon>().Init(c);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = listpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }

    }

    public void OnClickBack()
    {
        ClassPrepare.getInstance().ShowUI(ClassPrepare.UIPrepare.DataPush);
    }
}
