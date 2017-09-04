using PaintCraft.Controllers;
using PaintCraft.Pro.Controllers;
using PaintCraft.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw3DPad : MonoBehaviour {

    public Drawing3DController dc;
    public LineConfig lc;

    public GameObject pen;
    public GameObject penctrl;

    private CanvasController cc;

    // Use this for initialization
    void Start()
    {
        dc = gameObject.GetComponent<Drawing3DController>();

        lc = gameObject.GetComponent<LineConfig>();

        //if(dc != null && dc.paintcraftPlane != null)
        //{
        //    cc = dc.paintcraftPlane.PaintcraftCanvas;
        //}
    }

    //// Update is called once per frame
    //void Update () {

    //}

    private void OnEnable()
    {
        if(pen != null && !pen.activeSelf)
        {
            pen.SetActive(true);
        }

        if (penctrl != null && !penctrl.activeSelf)
        {
            penctrl.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (pen != null && pen.activeSelf)
        {
            pen.SetActive(false);
        }

        if (penctrl != null && penctrl.activeSelf)
        {
            penctrl.SetActive(false);
        }
    }

    public void ShowPenAndCtrl(bool sshow)
    {
        if(!sshow)
        {
            if (pen != null && pen.activeSelf)
            {
                pen.SetActive(false);
            }

            if (penctrl != null && penctrl.activeSelf)
            {
                penctrl.SetActive(false);
            }
        }else if (sshow)
        {
            if (pen != null && !pen.activeSelf)
            {
                pen.SetActive(true);
            }

            if (penctrl != null && !penctrl.activeSelf)
            {
                penctrl.SetActive(true);
            }
        }

    }

    private bool CheckCraftPlane()
    {
        if(dc == null)
        {
            return false;
        }

        if (cc != null)
        {
            return true;
        }

        //if (dc.paintcraftPlane == null)
        //{
        //    return false;
        //}

        //if(cc == null)
        //{
        //    cc = dc.paintcraftPlane.PaintcraftCanvas;
        //}
        
        if(cc == null)
        {
            return false;
        }

        return true;
    }

    public void OnClickClear()
    {
        if(!CheckCraftPlane())
        {
            return;
        }

        cc.ClearCanvas();
    }

    public void OnClickPen(Brush b)
    {
        if(b == null || lc == null)
        {
            return;
        }

        lc.Brush = b;
    }

    public void ChangeBrushSize(float size)
    {
        if (lc == null)
        {
            return;
        }

        lc.Scale = size * 0.1f;
    }

    public void OnClickColor(GameObject co)
    {
        if(lc == null)
        {
            return;
        }

        Color c = co.GetComponent<Image>().color;
        //lc.Color = new PointColor(c);
    }

    public void OnClickEraser()
    {
        if (!CheckCraftPlane())
        {
            return;
        }

    }
}
