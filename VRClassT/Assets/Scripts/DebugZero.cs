using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugZero {

    public static bool IsPrintLog = true;

    public static void Log(string msg)
    {
        if(!IsPrintLog)
        {
            return;
        }

        #if UNITY_EDITOR
            Debug.Log(msg);
        #endif
    }

    public static void Log(int msg)
    {
        if (!IsPrintLog)
        {
            return;
        }

        #if UNITY_EDITOR
            Debug.Log(msg.ToString());
        #endif
    }

    public static void LogWarning(string msg)
    {
        if (!IsPrintLog)
        {
            return;
        }

        #if UNITY_EDITOR
            Debug.LogWarning(msg);
        #endif
    }

    public static void LogError(string msg)
    {
        if (!IsPrintLog)
        {
            return;
        }

        #if UNITY_EDITOR
            Debug.LogError(msg);
        #endif
    }
}
