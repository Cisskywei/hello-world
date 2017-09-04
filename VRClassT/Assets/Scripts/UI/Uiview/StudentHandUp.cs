using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentHandUp : uibase {

    public Transform playerlistpanel;
    public GameObject playericon;

	// Use this for initialization
	void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

    // 初始化界面
    public void ShowSelf()
    {
        if (playerlistpanel == null)
        {
            return;
        }

        int count = playerlistpanel.childCount;
        Transform c = null;
        for (int i = 0; i < count; i++)
        {
            c = playerlistpanel.GetChild(i);
            if (c != null && c.gameObject.activeSelf)
            {
                c.gameObject.SetActive(false);
            }
        }

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

    public void AddStudent(int userid)
    {
        PlayerInfor p = UiDataManager.getInstance().GetPlayerById(userid);

        if(p == null)
        {
            return;
        }

        if(playerlistpanel == null)
        {
            return;
        }

        int count = playerlistpanel.childCount;
        bool isinit = false;
        Transform c = null;
        for (int i = 0;i< count;i++)
        {
            c = playerlistpanel.GetChild(i);
            if (c != null && !c.gameObject.activeSelf)
            {
                c.GetComponent<PlayerIcon>().Init(p, null);
                isinit = true;
                break;
            }
        }

        if(!isinit)
        {
            if (playericon == null)
            {
                return;
            }

            GameObject icon = GameObject.Instantiate(playericon, playerlistpanel);
            icon.GetComponent<PlayerIcon>().Init(p, null);

            if (!icon.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(true);
            }
        }
    }
}
