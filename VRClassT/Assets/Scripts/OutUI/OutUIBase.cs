using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutUIBase : MonoBehaviour {

	public virtual void ShowSelf(params System.Object[] args)
    {
        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public virtual void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
