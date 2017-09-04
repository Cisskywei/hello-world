using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberInfor : OutUIBase
{
    public Text nameTxt;
    public Text memberTxt;
    private string cout = "({0}人)";
    private int membercount;

    public GameObject iconPrafab;

    public Transform listpanel;

    public int classid = 1;

    public bool forceupdate = false;
    private bool isinit = false;

    public override void ShowSelf(params System.Object[] args)
    {
        int id = -1;
        if(args != null && args[0] != null)
        {
            id = (int)args[0];
        }

        InitData(id);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void InitData(int classid = -1)
    {
        if (!(forceupdate || !isinit))
        {
            return;
        }
        if(classid < 0)
        {
            classid = this.classid;
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
                    icon.GetComponent<PlayerIcon>().Init(c, null);
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
                icon.GetComponent<PlayerIcon>().Init(c, null);
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

        membercount = playerlist.Count;

        ShowMember();

        isinit = true;
    }

    public void ShowMember()
    {
        if (memberTxt == null)
        {
            return;
        }

        memberTxt.text = string.Format(cout, membercount);
    }

    public void OnClickBack()
    {
        ClassPrepare.getInstance().ShowUI(ClassPrepare.UIPrepare.PersonnelManagement);
    }
}
