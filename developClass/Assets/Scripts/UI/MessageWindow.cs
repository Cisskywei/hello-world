using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 消息提示 窗口
/// </summary>
public class MessageWindow : MonoBehaviour {

    private int _maxline = 10;
    private int _currentline = 0;

    public Text msgtext;
    private string _msg;

    private void Awake()
    {
        if(msgtext == null)
        {
            msgtext = gameObject.GetComponentInChildren<Text>();
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        ShowMsg("lllll");
    //    }

    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        ShowMsg("ssssss");
    //    }
    //}

    public void ShowMsg(string msg)
    {
        _currentline++;
        this._msg += '*' + msg + "\n";

        if(_currentline > _maxline)
        {
            //清除多余行
            string[] s = this._msg.Split('*');
            this._msg = string.Empty;
            for (int i=s.Length-_maxline; i<s.Length;i++)
            {
                this._msg += '*' + s[i];
            }

            _currentline = _maxline;
        }

        msgtext.text = this._msg;
    }

    public void ShowMsgColor(string msg, Color c)
    {
        // TODO
    }
}
