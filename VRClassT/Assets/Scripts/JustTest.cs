using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustTest : MonoBehaviour {

    //public SpriteRenderer s;

	// Use this for initialization
	void Start () {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            EnterCourse.getInstance().PlayerEnterCourse(197);
        }
    }
}
