using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ko.NetFram;

public class CourseList : OutUIBase
{
    public GameObject iconPrafab;

    public Transform listpanel;

    public bool forceupdate = false;
    private bool isinit = false;

    public override void ShowSelf(params System.Object[] args)
    {
        InitData();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void InitData()
    {
        if (!(forceupdate || !isinit))
        {
            return;
        }

        DataType.CourseListData[] courses = ClassDataManager.getInstance().courselist;

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (courses == null || courses.Length <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        for (int i=0; i < courses.Length; i++)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<CourseIcon>().Init(courses[i]);
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
                icon.GetComponent<CourseIcon>().Init(courses[i]);
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
