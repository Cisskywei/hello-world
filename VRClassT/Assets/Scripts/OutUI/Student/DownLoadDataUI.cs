using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownLoadDataUI : OutUIBase {

    public GameObject iconPrafab;

    public Transform leftlistpanel;
    public Transform rightlistpanel;

    public Dictionary<int, DownLoadItemInfor> downlist = new Dictionary<int, DownLoadItemInfor>();
    public Dictionary<int, DownLoadItemInfor> downlistother = new Dictionary<int, DownLoadItemInfor>();

    //   // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public override void ShowSelf(params System.Object[] args)
    {
        InitLeft();
   //     InitRight();

        base.ShowSelf();
    }

    public void InitLeft()
    {
        // 获取小组信息
        downlist = DownLoadDataManager.getInstance().GetAllContent();

        int count = 0;
        int tip = 0;
        if (leftlistpanel != null)
        {
            count = leftlistpanel.childCount;
        }

        if (downlist == null || downlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                leftlistpanel.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        foreach (DownLoadItemInfor g in downlist.Values)
        {
            if (tip < count)
            {
                Transform icon = leftlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<DownLoadItem>().Init(g);
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

                GameObject icon = GameObject.Instantiate(iconPrafab, leftlistpanel);
                icon.GetComponent<DownLoadItem>().Init(g);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = leftlistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    public void InitRight()
    {
        // 获取小组信息
        downlistother = DownLoadDataManager.getInstance().GetAllContent();

        int count = 0;
        int tip = 0;
        if (rightlistpanel != null)
        {
            count = rightlistpanel.childCount;
        }

        if (downlistother == null || downlistother.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                rightlistpanel.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        foreach (DownLoadItemInfor g in downlistother.Values)
        {
            if (tip < count)
            {
                Transform icon = rightlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<DownLoadItem>().Init(g);
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

                GameObject icon = GameObject.Instantiate(iconPrafab, rightlistpanel);
                icon.GetComponent<DownLoadItem>().Init(g);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = rightlistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    // 左边 课程资料
    public void AddCourseData(string name, string path, string typ, int fileid)
    {
        DownLoadItemInfor dii = DownLoadDataManager.getInstance().AddDownLoadItem(name, path, typ, fileid);

        int count = leftlistpanel.childCount;

        GameObject icon = null;
        for(int i=0;i<count;i++)
        {
            Transform t = leftlistpanel.GetChild(i);
            if(t != null && !t.gameObject.activeSelf)
            {
                icon = t.gameObject;
                break;
            }
        }

        if(icon == null)
        {
            icon = GameObject.Instantiate(iconPrafab, leftlistpanel);
        }

        icon.GetComponent<DownLoadItem>().Init(dii);

        if (!icon.gameObject.activeSelf)
        {
            icon.gameObject.SetActive(true);
        }
    }

    public void AddCourseDataAll(Hashtable files)
    {
        // 学生端收到老师端在资源推送消息 单个文件下载
        foreach (DictionaryEntry de in files)
        {
            int fileid = Convert.ToInt32(de.Key);
            Hashtable v = (Hashtable)de.Value;
            AddCourseData((string)v["name"], (string)v["url"], (string)v["typ"], fileid);
        }
    }

    //// 右边
    //public void AddSharedData()
    //{

    //}
}
