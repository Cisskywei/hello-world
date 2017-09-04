using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CourseContent : OutUIBase
{
    public Text titleTxt;
    //public Text chapterTxt;

    public GameObject iconPrafab;

    public Transform listpanel;

    public bool forceupdate = false;
    private bool isinit = false;

    // Use this for initialization
    void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

    public override void ShowSelf(params System.Object[] args)
    {
        InitData();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void InitData()
    {
        if (!(forceupdate || !isinit))
        {
            return;
        }

        //       Dictionary<int, ContentInfor> classlist = CourseContentManager.getInstance().GetAllContent();

        Dictionary<int, DownLoadItemInfor> contentlist = DownLoadDataManager.getInstance().GetAllContent();

        if (titleTxt != null)
        {
            //titleTxt.text = CourseContentManager.getInstance().coursename;
        }

        //if (chapterTxt != null)
        //{
        //    chapterTxt.text = CourseContentManager.getInstance().chapter;
        //}

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (contentlist == null || contentlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        foreach (DownLoadItemInfor c in contentlist.Values)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<ContentItem>().Init(c);
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
                icon.GetComponent<ContentItem>().Init(c);
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

        isinit = true;
    }
}
