using PaintCraft.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaetteCtrl : MonoBehaviour {

    // Store the Pen button
    public BrushPen pen;
    public Draw3DPad draw;

    [SerializeField]
    public Brush[] brush;

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public void NewBoard(int id)
    {
        draw.OnClickClear();
    }

    public void ChangeBrush(int id)
    {
        if(id < brush.Length && brush[id] != null)
        {
            draw.OnClickPen(brush[id]);
        }

        pen.ChangeToolTo("Pen");
    }

    public void ChangeBrushSize(float size)
    {
        draw.ChangeBrushSize(size);
    }

    public void ChangeColor(GameObject co)
    {
        draw.OnClickColor(co);
        pen.ChangeColor(co.GetComponent<Image>().color);
    }

    public void Eraser()
    {
        draw.OnClickEraser();
        pen.ChangeToolTo("Eraser");
    }

    public void Dustbin()
    {
        draw.OnClickClear();
    }
}
