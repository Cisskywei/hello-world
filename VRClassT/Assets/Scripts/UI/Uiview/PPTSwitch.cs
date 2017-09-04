using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTSwitch : MonoBehaviour {

    [SerializeField]
    public GameObject[] images;

    public int currentid = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Next()
    {
        if(images.Length <= 0)
        {
            return;
        }

        images[currentid].SetActive(false);

        currentid++;

        if (currentid >= images.Length)
        {
            currentid = currentid % images.Length;
        }

        images[currentid].SetActive(true);
    }
}
