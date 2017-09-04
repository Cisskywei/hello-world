using HTC.UnityPlugin.ColliderEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DonotFly : MonoBehaviour , IColliderEventDragStartHandler, IColliderEventDragEndHandler
{
    private Rigidbody rb;
    private BasicGrabbable bg;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        bg = gameObject.GetComponent<BasicGrabbable>();
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void OnColliderEventDragEnd(ColliderButtonEventData eventData)
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void OnColliderEventDragStart(ColliderButtonEventData eventData)
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
