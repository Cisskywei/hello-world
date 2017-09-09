using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
        {
            CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.InitScene, InitScene);
            ArrayList a = new ArrayList();
            a.Add((Int64)CommandDefine.FirstLayer.CourseWave);
            a.Add((Int64)CommandDefine.SecondLayer.InitScene);
            a.Add(99);
            NetworkCommunicate.getInstance().ReqCommand(DataCache.getInstance().classid, DataCache.getInstance().userid, a);

            Debug.Log(" 0000 ");
        }
	}

    private void InitScene(int userid, ArrayList msg)
    {
        Debug.Log(msg.Count);

        for(int i=0; i<msg.Count; i++)
        {
            Debug.Log(msg[i]);
        }
    }
}
