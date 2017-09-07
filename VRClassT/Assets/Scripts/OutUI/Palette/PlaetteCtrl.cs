using PaintCraft.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaetteCtrl : MonoBehaviour {

    // Store the Pen button
    //public BrushPen pen;
    //public Draw3DPad draw;

    public DrawPaint pen;

    //[SerializeField]
    //public Brush[] brush;

    public void NewBoard(int id)
    {
        pen.Clear();
    }

    public void ChangeBrush(int id)
    {
        //if(id < brush.Length && brush[id] != null)
        //{
        //    pen.OnClickPen(brush[id]);
        //}
    }

    public void ChangeBrushSize(float size)
    {
        pen.ChangeBrushSize(size);
    }

    public void ChangeColor(GameObject co)
    {
        pen.ChangeColor(co.GetComponent<Image>().color);
    }

    public void Eraser()
    {
        //pen.OnClickEraser();
    }

    public void Dustbin()
    {
        pen.Clear();
    }
}
