using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class Push : OutUIBase
{
    // 推送资料列表选择
    public ChooseData choose;

    public GameObject iconPrafab;
    public Transform listpanel;

    public override void ShowSelf(params object[] args)
    {
        InitClassList();
        base.ShowSelf();
    }

    public override void HideSelf()
    {
        base.HideSelf();
    }

    //选择资料推送按钮监听
    public void OnClickPushChoose()
    {
        if(choose != null)
        {
            choose.ShowSelf();
        }
    }

    //初始化班级列表
    public void InitClassList()
    {
        Dictionary<int, ClassInfor> classlist = ClassManager.getInstance().GetAllClass();

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (classlist == null || classlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        foreach (ClassInfor c in classlist.Values)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<ClassIcon>().Init(c.classname,"学生详细", c.classid);

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

                GameObject icon2 = GameObject.Instantiate(iconPrafab, listpanel);
                icon2.GetComponent<ClassIcon>().Init(c.classname, "学生详细", c.classid);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                GameObject icon3 = listpanel.GetChild(i).gameObject;
                if (icon3.activeSelf)
                {
                    icon3.SetActive(false);
                }
            }
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ChooseClass, this.ChooseClass);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ChooseClass, this.ChooseClass);
    }

    public void ChooseClass(int id)
    {
        Debug.Log("ooooooooooo " + id);
        // 查看详细信息
        ClassPrepare.getInstance().ShowUI(ClassPrepare.UIPrepare.ClassDataPushInfor, new System.Object[] { id });
    }

    // 全部

    // 正在下载

    // 下载失败
}
