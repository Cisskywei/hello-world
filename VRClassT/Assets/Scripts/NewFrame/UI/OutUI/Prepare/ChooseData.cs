using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class ChooseData : OutUIBase
{
    public Transform listpanel;
    public GameObject iconPrafab;

    public override void ShowSelf(params object[] args)
    {
        InitData();

        base.ShowSelf(args);
    }

    public void InitData()
    {
        Dictionary<int, DownLoadItemInfor> contents = DownLoadDataManager.getInstance().GetAllContent();

        int count = 0;
        int tip = 0;
        if (listpanel != null)
        {
            count = listpanel.childCount;
        }

        if (contents == null || contents.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                listpanel.GetChild(i).gameObject.SetActive(false);
            }

            HideSelf();
            return;
        }

        foreach (DownLoadItemInfor c in contents.Values)
        {
            if (tip < count)
            {
                Transform icon = listpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PushDataItem>().Init(c);
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
                icon.GetComponent<PushDataItem>().Init(c);
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

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        UnRegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.DownLoadFileItem, this.DownLoadFileItem);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.DownLoadFileItem, this.DownLoadFileItem);
    }

    private void DownLoadFileItem(int fileid)
    {
        HideSelf();

        DownLoadItemInfor dlii = DownLoadDataManager.getInstance().GetContentById(fileid);
        DownLoadDataManager.getInstance().DownLoadOnce(fileid);

        // 发送推送消息
        if(dlii != null)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.PushDataOne);
            msg.Add(dlii.filename);
            msg.Add(dlii.filepath);
            msg.Add(dlii.type);
            msg.Add(dlii.fileid.ToString());
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, msg);
        }
    }

    // 一键推送按钮
    public void OnClickPushAllOnce()
    {
        Dictionary<int, DownLoadItemInfor> datalist = DownLoadDataManager.getInstance().GetAllContent();
        DownLoadDataManager.getInstance().DownLoadAll();

        if(datalist != null && datalist.Count > 0)
        {
            Hashtable files = new Hashtable();

            foreach(DownLoadItemInfor de in datalist.Values)
            {
                Hashtable file = new Hashtable();
                file.Add("name", de.filename);
                file.Add("typ", de.type);
                file.Add("url", de.filepath);

                files.Add(de.fileid, file);
            }

            if(files.Count > 0)
            {
                ArrayList msg = new ArrayList();
                msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
                msg.Add((Int64)CommandDefine.SecondLayer.PushDataAll);
                msg.Add(files);
                CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, msg);
            }
        }

        HideSelf();
    }
}
