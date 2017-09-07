using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 同步白板上面的画板
/// </summary>
public class SyncPaint : MonoBehaviour, NetObjectInterFace.IObjectPipeSync
{
    [SerializeField]
    private int selfid = -1;

    private ArrayList senddata = new ArrayList();

    // Use this for initialization
    void Start () {
        if (selfid > 0)
        {
            ObjectCollector.getInstance().Add(selfid, gameObject);
        }

        // 注册update 函数 
        NetMainLoop.Instance().AddUpdate(OnUpdate);
    }
	
	// Update is called once per frame
	void OnUpdate() {
		
	}

    public void DoSync()
    {
    }

    public Hashtable GetHash()
    {
        Hashtable ret = new Hashtable();

        return ret;
    }

    public void ReceiveSync(Hashtable data)
    {
    }

    public void SendSync(System.Object data)
    {
        if(selfid < 0)
        {
            return;
        }

        ContinuousPipe.getInstance().AddData(selfid, (Vector2)data);
    }

    // 提供给画笔调用
    public void AddData(Vector2 data)
    {
        if(senddata == null)
        {
            senddata = new ArrayList();
        }

        senddata.Add(data.x);
        senddata.Add(data.y);
    }

    public void ReceiveSync(int fromid, ArrayList data)
    {
    }
}
