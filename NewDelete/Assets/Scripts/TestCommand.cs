using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestCommand : MonoBehaviour {

    public ArrayList a = new ArrayList();
	// Use this for initialization
	void Start () {
        a.Add(CommandDefine.FirstLayer.Login);
        a.Add(CommandDefine.SecondLayer.Login);
        a.Add("name");
        a.Add("password");

        init();
    }

    private void init()
    {
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Login, CommandDefine.SecondLayer.Login, receive_func);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Login, CommandDefine.SecondLayer.Login, receive_func);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Login, CommandDefine.SecondLayer.Login, receive_func2);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(System.DateTime.Now.Millisecond);
            CommandReceive.getInstance().Receive(1, a);
            Debug.Log(System.DateTime.Now.Millisecond);
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {

            CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Login, CommandDefine.SecondLayer.Login, receive_func);

        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {

            CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Login, CommandDefine.SecondLayer.Login, receive_func2);

        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            init();
        }
    }

    private void receive_func(int userid, ArrayList msg)
    {
        Debug.Log(msg.Count);
        Debug.Log(msg[0]);
        Debug.Log(msg[1]);
        Debug.Log(msg[2]);
        Debug.Log(msg[3]);
    }

    private void receive_func2(int userid, ArrayList msg)
    {
        Debug.Log(msg.Count + " ---- ");
        Debug.Log(msg[0] + " ---- ");
        Debug.Log(msg[1] + " ---- ");
        Debug.Log(msg[2] + " ---- ");
        Debug.Log(msg[3] + " ---- ");
    }
}
