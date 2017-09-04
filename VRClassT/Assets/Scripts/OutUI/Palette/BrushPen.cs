using PaintCraft.Pro.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushPen : MonoBehaviour {

    public Transform drawboard;
    public Transform penpoint;

    public Drawing3DController draw;

    private bool isBeginDraw = false;

    public GameObject point;
    private Material pointcolor;

    private void Awake()
    {
        if(point != null)
        {
            pointcolor = point.GetComponent<MeshRenderer>().material;
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            //draw.StartDraw();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            //draw.EndDraw();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBeginDraw)
        {
            return;
        }

        if (other.tag == "paint")
        {
            isBeginDraw = true;

            //draw.StartDraw();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isBeginDraw)
        {
            return;
        }

        if (other.tag == "paint")
        {
            isBeginDraw = false;

            //draw.EndDraw();
        }
    }

    public void ChangeColor(Color cto)
    {
        Debug.Log("changecolor");
        if (pointcolor == null)
        {
            pointcolor = point.GetComponent<MeshRenderer>().material;
        }

        if (pointcolor != null)
        {
            Debug.Log("changecolor 222");

            pointcolor.color = cto;
        }
    }

    public void ChangeToolTo(string toolName)
    {
        if (toolName == "Pen")
        {
        }

        if (toolName == "Eraser")
        {
        }
    }
}
