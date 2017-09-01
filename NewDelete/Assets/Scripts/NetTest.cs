using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTest : MonoBehaviour {

    void Awake()
    {
        Debug.Log("注册");

        EventsDispatcher.getInstance().MainEventInt.AddEventListener(0, onConnectHub);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetKeyDown(KeyCode.C))
  //      {
  //          NetworkCommunicate.getInstance().ChangeModel(1, 2);
  //      }

        ChangeModelTest();
    }

    private void onConnectHub()
    {
        Debug.Log("onConnectHub  --   NetTest");

        Login();
    }

    // 初始化监听函数
    private void InitListener()
    {

    }

    private void Login()
    {
        NetworkCommunicate.getInstance().PlayerLogin("wang","123");

        Debug.Log("Login  --   NetTest");

        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retLogin, retLogin);
    }

    private void retLogin(Hashtable msg)
    {
        Debug.Log(msg.Count + " ----- ");

        string duty = Convert.ToString(msg["duty"]);
        userid = Convert.ToInt32(msg["id"]);
        uuid = Convert.ToString(msg["uuid"]);
        string token = Convert.ToString(msg["token"]);

        Debug.Log(duty + userid + uuid + token);

        NetworkCommunicate.getInstance().PlayerEnterLab(token, userid, duty == "teacher"?2:1);

        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retEnterLab, retEnterLab);
    }

    private int userid = 1;
    private string uuid = string.Empty;
    private void retEnterLab(Hashtable msg)
    {
        Debug.Log((msg.Count));

        NetworkCommunicate.getInstance().PlayerEnterCourse(userid, uuid, 227);

        CommandReceive.getInstance().AddHashMsgListener(CommandDefine.HashTableMsgType.retEnterCourse, retEnterCourse);
    }

    private int classid = -1;
    private void retEnterCourse(Hashtable msg)
    {
        Debug.Log((msg.Count));

        //foreach(DictionaryEntry v in msg)
        //{
        //    Debug.Log(v.Key + " ---- " + v.Value);
        //}

        classid = Convert.ToInt32(msg["classid"]);

        InitScene();

        a.Add((Int64)CommandDefine.FirstLayer.CourseWave);
        a.Add((Int64)CommandDefine.SecondLayer.ChangeMode);
        a.Add((Int64)Enums.TeachingMode.WatchLearnModel_Sync);
    }

    private void InitScene()
    {
        Hashtable h = ObjectCollector.getInstance().GetHash();
        ArrayList a = new ArrayList();
        a.Add((Int64)CommandDefine.FirstLayer.CourseWave);
        a.Add((Int64)CommandDefine.SecondLayer.InitScene);
        a.Add(h);
        NetworkCommunicate.getInstance().ReqCommand(classid, userid, a);
        Debug.Log("InitScene  --   NetTest");
    }

    private ArrayList a = new ArrayList();
    private void ChangeModelTest()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            a[2] = ((int)Enums.TeachingMode.WatchLearnModel_Sync);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            a[2] = ((Int64)Enums.TeachingMode.WatchLearnModel_Async);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            a[2] = ((Int64)Enums.TeachingMode.GuidanceMode_Personal);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            a[2] = ((Int64)Enums.TeachingMode.GuidanceMode_Group);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            a[2] = ((Int64)Enums.TeachingMode.SelfTrain_Personal);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            a[2] = ((Int64)Enums.TeachingMode.SelfTrain_Group);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            a[2] = ((Int64)Enums.TeachingMode.SelfTrain_All);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            ArrayList a2 = new ArrayList();
            a2.Add((Int64)CommandDefine.FirstLayer.CourseWave);
            a2.Add((Int64)CommandDefine.SecondLayer.Hold);
            a2.Add(1);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a2);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ArrayList a2 = new ArrayList();
            a2.Add((Int64)CommandDefine.FirstLayer.CourseWave);
            a2.Add((Int64)CommandDefine.SecondLayer.Release);
            a2.Add(1);
            NetworkCommunicate.getInstance().ReqCommand(227, 67, a2);
        }
    }
}
