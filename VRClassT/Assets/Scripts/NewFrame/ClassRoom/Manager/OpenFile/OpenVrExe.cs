using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using TinyFrameWork;
using UnityEngine;
using Valve.VR;

public class OpenVrExe : MonoBehaviour {

    public OpenFileManager.VoidCallBack closecallback;
    private string fullpath = string.Empty;

    //// Use this for initialization
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}

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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.PlayerEnterCourseware, this.PlayerEnterCourseware);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.PlayerEnterCourseware, this.PlayerEnterCourseware);
    }

    private void PlayerEnterCourseware(int ret)
    {
        if (ret != 1) return;
        if (fullpath == string.Empty) return;

        StartProcess(fullpath);
    }

    public void Open(string path)
    {
        if(path == null)
        {
            return;
        }

        //      path = "file:///" + path.Replace('\\', '/');

        fullpath = path;

        StartProcess(path);

        //      MsgModule.getInstance().reqPlayerEnterCourseware();

        //if (!gameObject.activeSelf)
        //{
        //    gameObject.SetActive(true);
        //}
    }

    public void Close()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        if (closecallback != null)
        {
            closecallback.Invoke();
        }
    }

    private void StartProcess(string fullpath)
    {
        if (!File.Exists(fullpath))
        {
            return;
        }
        /*
        try
        {
            //   BackFromVrExe.getInstance().SaveJsonDataNo();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fullpath;
            //startInfo.WorkingDirectory = cur.StartInfo.WorkingDirectory;
            startInfo.Arguments = BackFromVrExe.getInstance().savedata.name
                + " " + BackFromVrExe.getInstance().savedata.password
                + " " + BackFromVrExe.getInstance().savedata.courseid.ToString()
                + " " + BackFromVrExe.getInstance().OpenExeCachePath()
                + " " + Process.GetCurrentProcess().MainModule.FileName;
            Process proc = Process.Start(startInfo);

            //var applications = OpenVR.Applications;
            //Process cur = Process.GetCurrentProcess();
            //applications.LaunchInternalProcess(fullpath, "haha", cur.StartInfo.WorkingDirectory);

            Application.Quit();

            //if (proc != null)
            //{
            //    //监视进程退出
            //    proc.EnableRaisingEvents = true;
            //    //指定退出事件方法
            //    proc.Exited += new EventHandler(proc_Exited);
            //}
        }
        catch (ArgumentException ex)
        {

        }*/
    }
}
