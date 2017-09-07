using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UITouch3D : MonoBehaviour {

    public string comparetag = "paint";
    public UnityEvent onclick;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " -- ? " + collision.gameObject.tag);

        if (collision.gameObject.tag != comparetag)
        {
            return;
        }

        Debug.Log(collision.gameObject.name + " -- 进来了");

        if(onclick != null)
        {
            onclick.Invoke();
        }

    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (board == null)
    //    {
    //        board = collision.transform;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag != comparetag)
    //    {
    //        return;
    //    }

    //}
}
