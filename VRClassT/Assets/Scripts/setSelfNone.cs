using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setSelfNone : MonoBehaviour {

    private bool ishide = false;
    private int waitframe = 0; // 4;

    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        //gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (waitframe <= 4)
        {
            waitframe++;
            return;
        }

        if (transform.childCount > 1)
        {

            if (transform.GetChild(1).gameObject.activeSelf)
            {
                transform.GetChild(1).gameObject.SetActive(false);

            }
        }
	}
}
