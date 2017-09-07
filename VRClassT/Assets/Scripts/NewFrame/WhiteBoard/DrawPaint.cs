using PaintCraft.Controllers;
using PaintCraft.Pro.Controllers;
using PaintCraft.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPaint : MonoBehaviour {

    public enum PaintCtrl
    {
        None = -1,

        ChangColor,
        ChangeSize,
        Clear,

    }

    public Transform startpos;
    public Draw3DController draw;
    public CanvasController ctrl;
    public LineConfig line;

    // 自身控制
    public Material mat;

    private void Awake()
    {
        if(draw != null)
        {
            draw.penpoint = startpos;
        }

        if(mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().materials[0];
        }
    }

    // 对画笔的控制
    public void ChangeColor(Color c)
    {
        mat.color = c;
        line.Color.R = c.r;
        line.Color.G = c.g;
        line.Color.B= c.b;
        line.Color.Alpha = c.a;
    }

    public void ChangeBrushSize(float size)
    {
        line.Scale = size;
    }

    public void Clear()
    {
        ctrl.ClearCanvas();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "white")
        {
            return;
        }

        draw.StartDraw();
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (board == null)
    //    {
    //        board = collision.transform;
    //    }
    //}

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "white")
        {
            return;
        }

        draw.EndDraw();
    }
}
