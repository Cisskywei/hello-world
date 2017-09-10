using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using ko.NetFram;

public class MsgTips : OutUIBase {

    public Text msgTxt;
    public Image img;

    private Color _c;
    private Color _cText;

    private Tweener _fadein;
    private Tweener _fadeout;

    private TimeCountDown _timer = new TimeCountDown();

    // Use this for initialization
    void Start () {
        if (img != null)
        {
            _c = img.color;
            _cText = msgTxt.color;
        }
    }

    private void fadeinend()
    {
        _fadeout = img.DOFade(0, 1.5f);
        msgTxt.DOFade(0, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(_timer != null)
        {
            _timer.loop(Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            ShowMessage("llllllllllllllllllllll");
        }
    }

    public void  ShowMessage(string msg)
    {
        if(msg == null)
        {
            return;
        }

        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        msgTxt.text = msg;

        if(img == null)
        {
            img = gameObject.GetComponent<Image>();
            _c = img.color;
            _cText = msgTxt.color;
        }

        _fadein = img.DOFade(1, 0.5f);
        msgTxt.DOFade(1, 0.5f);

        _fadein.OnComplete(fadeinover);
    }

    private void fadeinover()
    {
        _timer.OnTimeOver(fadeinend);
        _timer.StartTimer(5);
    }
}
