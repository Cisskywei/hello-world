using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class DownLoadDataManager {

    public static DownLoadDataManager getInstance()
    {
        return Singleton<DownLoadDataManager>.getInstance();
    }

    public Dictionary<int, DownLoadItemInfor> contentlist = new Dictionary<int, DownLoadItemInfor>();

    public void InitMaterialList(string jsondata)
    {
        jsondata = JsonDataHelp.getInstance().DecodeBase64(null, jsondata);
        DataType.MaterialItemInforRetData datalist = JsonDataHelp.getInstance().JsonDeserialize<DataType.MaterialItemInforRetData>(jsondata);

        Debug.Log("课程资料信息 " + jsondata);

        if (datalist != null && datalist.data != null)
        {
            for (int i = 0; i < datalist.data.Length; i++)
            {
                DownLoadItemInfor q = new DownLoadItemInfor(datalist.data[i]);
                if (q != null)
                {
                    contentlist.Add(q.fileid, q);
                }
            }
        }
    }

    // 学生端监听老师推送资料消息 动态添加下载 条目
    public DownLoadItemInfor AddDownLoadItem(string name, string path, string typ, int fileid)
    {
        DownLoadItemInfor q = new DownLoadItemInfor(name,path,typ,fileid);
        if (q != null)
        {
            contentlist.Add(q.fileid, q);
        }

        Debug.Log("添加下载 列表");

        return q;
    }

    //public void RegisterEvent()
    //{
    //    EventDispatcher.GetInstance().MainEventManager.AddEventListener<string, string>(EventId.DownLoadFileOne, this.DownLoadFileOne);
    //    EventDispatcher.GetInstance().MainEventManager.AddEventListener<Hashtable>(EventId.DownLoadFileAll, this.DownLoadFileAll);
    //}

    ///// <summary>
    ///// unregister the target event message.
    ///// </summary>
    //public void UnRegisterEvent()
    //{
    //    EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string, string>(EventId.DownLoadFileOne, this.DownLoadFileOne);
    //    EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Hashtable>(EventId.DownLoadFileAll, this.DownLoadFileAll);
    //}

    //private void DownLoadFileOne(string filename, string fileurl)
    //{
    //    // 学生端收到老师端在资源推送消息 单个文件下载
    //}

    //private void DownLoadFileAll(Hashtable files)
    //{
    //    // 学生端收到老师端在资源推送消息 单个文件下载
    //}

    public DownLoadItemInfor GetContentById(int id)
    {
        if (contentlist.ContainsKey(id))
        {
            return contentlist[id];
        }

        return null;
    }

    public Dictionary<int, DownLoadItemInfor> GetAllContent()
    {
        return contentlist;
    }

    // 根据后缀名获取文件列表
    //public List<DownLoadItemInfor> GetContentsByType(string suffix, string filetype)
    //{
    //    List<DownLoadItemInfor> ret = new List<DownLoadItemInfor>();
    //    return ret;
    //}

    // 根据文件类型获取文件 （仅在线下载了的）
    public List<DownLoadItemInfor> GetContentsByType(ComonEnums.ContentDataType type)
    {
        List<DownLoadItemInfor> ret = new List<DownLoadItemInfor>();

        foreach(DownLoadItemInfor dinfor in contentlist.Values)
        {
            if(dinfor.contentype != type || dinfor.state != DownLoadItemInfor.DownLoadState.Complete)
            {
                continue;
            }

            ret.Add(dinfor);
        }

        return ret;
    }

    public void DownLoadOnce(int fileid)
    {
        if(!contentlist.ContainsKey(fileid))
        {
            return;
        }

        // 通知学生开始下载相应资料
        // 

        contentlist[fileid].StartDownLoad();
    }

    public void DownLoadAll()
    {
        if(contentlist == null || contentlist.Count <= 0)
        {
            return;
        }

        // 通知学生开始下载相应资料
        // 

        foreach (DownLoadItemInfor dii in contentlist.Values)
        {
            dii.StartDownLoad();
        }
    }
}
