using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : MonoBehaviour {

    public delegate void TestDelegate();
    public event TestDelegate teste;
    public TestDelegate testd;

    // Use this for initialization
    void Start () {
        teste += new TestDelegate(func1);
        teste += new TestDelegate(func2);

        testd += func1;
        testd += func2;
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("开始了 o");
            teste();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("开始了 p");
            testd();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("开始了 q");
            testd.Invoke();
        }
    }

    private void func1()
    {
        Debug.Log("哈哈");
    }

    private void func2()
    {
        Debug.Log("嘻嘻");
    }
}
