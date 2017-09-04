using PaintCraft.Pro.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPaint : MonoBehaviour {

    public Transform startpos;

    public Draw3DController draw;

    private Transform board;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");

        if(board == null)
        {
            board = collision.transform;
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
        board = null;
        draw.EndDraw();
    }
}
