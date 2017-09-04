using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowUI : MonoBehaviour {

    public GameObject flowCamera;

    public bool ismove = true;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate() {
        if (ismove && flowCamera != null)
        {
            transform.position = flowCamera.transform.position;
            transform.rotation = flowCamera.transform.rotation;
        }
    }

    private void OnEnable()
    {
        if (flowCamera != null)
        {
            transform.position = flowCamera.transform.position;
            transform.rotation = flowCamera.transform.rotation;
        }
    }

    public void ChangeMove(bool ismove)
    {
        this.ismove = ismove;
    }
}
