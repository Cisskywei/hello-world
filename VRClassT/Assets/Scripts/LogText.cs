using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour {

    private static GameObject selfgo;
    private static LogText _instance;
    public static LogText getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("logText");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<LogText>();
            }
        }

        return _instance;

    }

    public Text log;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Log(string msg)
    {
        log.text += " || " + msg;
    }
}
