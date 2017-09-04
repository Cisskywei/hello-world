using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DownLoadItemInfor {

    public enum DownLoadState
    {
        None = 0,
        DownLoading,
        Pause,
        Complete,
    }

    public int fileid;
    public int folderid;
    public string type;
    public string filename;
    public string filepath;
    public string extend;

    //public int userid;
    //public float size;
    //public string package;
    //public string create_time;
    //public string update_time;
    //public int converted;

    // 文件枚举类型
    public Enums.ContentDataType contentype = Enums.ContentDataType.None;

    // 下载过后的完整文件保存路径
    public string fullfilepath;
    public float progress;

    private HttpDownLoad http;
    public string savePath; // FileManager.getInstance().GetCourseMaterialPath(); // Application.persistentDataPath; // 路径后期通过文件管理程序获取
    public DownLoadState state = DownLoadState.None;

    public DownLoadItemInfor()
    {

    }

    public DownLoadItemInfor(DataType.MaterialItemInfor infor)
    {
        fileid = Convert.ToInt32(infor.file_id);
        folderid = Convert.ToInt32(infor.folder_id);
        type = infor.type;
        filename = infor.file_name;
        filepath = UserInfor.RootUrl + infor.file_path;
        extend = infor.extend;

        this.contentype = FileManager.getInstance().GetFileContenType(filename, type);

        this.state = DownLoadState.None;

        savePath = FileManager.getInstance().GetFileSavePath(filename, type);
    }

    public DownLoadItemInfor(string name, string path, string typ, int fileid)
    {
        this.fileid = fileid;
        type = typ;
        filename = name;
        filepath = path;

        this.contentype = FileManager.getInstance().GetFileContenType(filename, type);

        this.state = DownLoadState.None;

        savePath = FileManager.getInstance().GetFileSavePath(filename, type);
    }

    public void StartDownLoad()
    {

        if(state == DownLoadState.Complete || state == DownLoadState.DownLoading)
        {
            return;
        }

        if (http == null)
        {
            http = new HttpDownLoad();
        }

        if(savePath == null)
        {
            savePath = FileManager.getInstance().GetFileSavePath(filename, type);
        }

        http.DownLoad(filepath, savePath, DownLoadOver, filename);

        state = DownLoadState.DownLoading;
    }

    public void DownLoadOver()
    {
        fullfilepath = savePath + "\\" + filename;
        progress = 0.99f;

        // 如果是ppt执行转换操作
        Enums.ContentDataType suffixtype = FileManager.getInstance().GetFileContenType(filename, type);
        Debug.Log("ppt suffixtype" + suffixtype);
        switch(suffixtype)
        {
            case Enums.ContentDataType.PPt:
                string imagepath = FileManager.getInstance().GetPPTImagePath(filename);

                fullfilepath = fullfilepath.Replace('/', '\\');

        //        PPTToPicture.getInstance().ConvertPPT2Image(fullfilepath, imagepath);

                Debug.Log("ppt文件原先路径" + fullfilepath);

                fullfilepath = imagepath;
                Debug.Log("下载完成ppt 并且转换完成 " + imagepath);
                break;
            case Enums.ContentDataType.Zip:
                string err = string.Empty;
                Debug.Log("zip文件原先路径" + fullfilepath);
                ZipHelper.UnZipFile(fullfilepath,string.Empty,out err);
                fullfilepath = fullfilepath.Replace(Path.GetFileName(fullfilepath), Path.GetFileNameWithoutExtension(fullfilepath));
                Debug.Log("zip文件现在路径" + fullfilepath);

                if (!fullfilepath.EndsWith("/"))
                {
                    fullfilepath += "/";
                }
                string exename = string.Empty;
                if (filename.Contains("."))
                {
                    int diff = filename.LastIndexOf('.');
                    exename = filename.Substring(0, diff);
                    exename += "/" + exename + ".exe";
                }

                if(exename != string.Empty)
                {
                    fullfilepath += exename;
                }else
                {
                    fullfilepath += filename;
                }
                Debug.Log("zip文件真正路径" + fullfilepath);

                break;
            default:
                break;
        }

        state = DownLoadState.Complete;
        progress = 1;
    }

    public float GetProgress()
    {
        if (state == DownLoadState.Complete)
        {
            return 1;
        }

        if(http != null)
        {
            return http.progress;
        }

        return 0;
    }
}
