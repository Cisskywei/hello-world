/** 
 *Copyright(C) 2015 by SpringVr 
 *All rights reserved. 
 *FileName:     ErrorDisplay.cs 
 *Author:       SpringVr_Gz 
 *Version:      0.4.0 
 *UnityVersion：5.4.0f3 
 *Date:         2016-12-19 
 *Description:    在安卓上打印错误log
*/
using UnityEngine;
using System.Collections;

public class ErrorDisplay : MonoBehaviour
{
    private string m_strLogs;
    public bool Log;
    private Vector2 m_scroll;

    internal void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    internal void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    /// <summary>接收错误信息</summary>    
    /// /// <param name="logString">错误信息</param>    /// 
    /// <param name="stackTrace">跟踪堆栈</param>    /// 
    /// <param name="type">错误类型</param>    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        m_strLogs += logString + "\n" + stackTrace;
    }
    internal void OnGUI()
    {
        if (!Log) return;
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;    //设置背景填充  
        fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色  
        fontStyle.fontSize = 40;       //字体大小 
        m_scroll = GUILayout.BeginScrollView(m_scroll, GUILayout.Width(1800),GUILayout.Height(800));
        GUILayout.Label(m_strLogs, fontStyle);
        GUILayout.EndScrollView();
        //GUILayout.TextField("左手所有已经接受到但是还没有经过处理的数据 ： " + ReadLeftHandData.Instance.m_strAllReceiveData);
        //GUILayout.TextField("右手所有已经接受到但是还没有经过处理的数据 ： " + ReadRightHandData.Instance.m_strAllReceiveData);
        //GUILayout.TextField("左手数据 ： " + ReadLeftHandData.Instance.m_strCanUseData);
        //GUILayout.TextField("右手数据 ： " + ReadRightHandData.Instance.m_strCanUseData);
        if (GUILayout.Button("Clear", GUILayout.Width(250), GUILayout.Height(100)))
        {
            m_strLogs = null;
            //ReadLeftHandData.Instance.m_strCanUseData = null;
            //ReadRightHandData.Instance.m_strCanUseData = null;
        }
    }
}