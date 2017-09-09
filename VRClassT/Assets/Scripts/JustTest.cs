using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustTest : MonoBehaviour {

    //public SpriteRenderer s;

	// Use this for initialization
	void Start () {
        //int count = PPTToPicture.getInstance().ConvertPPT2Image("C:\\Users\\Administrator\\Desktop\\testfile\\tyh.pptx", "C:\\Users\\Administrator\\Desktop\\testfile\\xx");
        //Debug.Log(count);
        //Sprite[] slist = PPTToPicture.getInstance().LoadImage2SpriteByIO("C:\\Users\\Administrator\\Desktop\\testfile\\xx",count);

        //Debug.Log(slist.Length);
        //s.sprite = slist[slist.Length-1];

        ArrayList a = new ArrayList();
        a.Add(1);
        a.Add(2);
        a.Add(3);
        a.Add(4);
        a.Add(5);
        a.Add(6);

        ArrayList a2 = a.GetRange(0, a.Count-1);
        for(int i=0;i<a2.Count;i++)
        {
            Debug.Log(a2[i]);
        }
    }
}
