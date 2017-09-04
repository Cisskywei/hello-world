using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileDataListUI : OutUIBase {

    public Transform listPanel;
    public GameObject itemPrefab;

    public FileDataItem.OnClickListen clickfilelisten;

    public override void ShowSelf(params System.Object[] args)
    {
        if(args != null)
        {
            if(args.Length > 1)
            {
                string suffix = (string)args[0];
                string filetyp = (string)args[1];
                InitData(suffix, filetyp);
            }
            else if(args.Length > 0)
            {
                Enums.ContentDataType type = (Enums.ContentDataType)args[0];
                InitData(type);
            }
            
        }
        base.ShowSelf(args);
    }

    public void InitData(string suffix, string filetyp)
    {
        Enums.ContentDataType type = FileManager.getInstance().GetFileContenTypeBySuffix(suffix, filetyp);

        InitData(type);
    }

    public void InitData(Enums.ContentDataType type)
    {
        if (type == Enums.ContentDataType.None)
        {
            type = Enums.ContentDataType.PPt;
        }

        List<DownLoadItemInfor> fileitems = DownLoadDataManager.getInstance().GetContentsByType(type);

        if (fileitems == null || fileitems.Count <= 0)
        {
            HideSelf();
            return;
        }

        InitItemByList(fileitems, listPanel, itemPrefab);
    }

    // 生成子物体
    public void InitItemByList(List<DownLoadItemInfor> listobject, Transform listpanel, GameObject itemprefab)
    {
        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (listobject == null || listobject.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        foreach (DownLoadItemInfor g in listobject)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<FileDataItem>().Init(g, OnClickListen);
                    if (!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (itemprefab == null)
                {
                    return;
                }

                GameObject icon = GameObject.Instantiate(itemprefab, listpanel);
                icon.GetComponent<FileDataItem>().Init(g, OnClickListen);
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

    // 点击监听
    private void OnClickListen(DownLoadItemInfor fileinfor)
    {
        Debug.Log("打开文件" + fileinfor.filename);

        if(clickfilelisten != null)
        {
            clickfilelisten(fileinfor);
        }

        // 打开后关闭自己
        HideSelf();
    }

    // 注册点击监听
    public void RegisterOpenListen(FileDataItem.OnClickListen listen)
    {
        clickfilelisten = listen;
    }
}
