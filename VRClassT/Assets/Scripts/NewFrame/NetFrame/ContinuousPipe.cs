using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousPipe {

    public static ContinuousPipe getInstance()
    {
        return Singleton<ContinuousPipe>.getInstance();
    }

    public ContinuousPipe()
    {
        NetMainLoop.Instance().AddUpdate(OnUpdate);
    }

    private Hashtable senddata = new Hashtable();  // key : objectid, value : arraylist (连续变换的值)

    private int framespace = 5;  // 每5帧同步
    private int frame = 0;

    void OnUpdate() {
        if (frame++ > framespace)
        {
            frame = 0;
            Send();
        }
    }

    public void AddData(int objectid, Vector2 data)
    {
        string key = objectid.ToString();
        if(senddata == null)
        {
            senddata = new Hashtable();
        }

        if(senddata.ContainsKey(key))
        {
            if(senddata[key] == null)
            {
                ArrayList a = new ArrayList();
                a.Add(data.x);
                a.Add(data.y);
                senddata[key] = a;
            }
            else
            {
                ((ArrayList)senddata[key]).Add(data.x);
                ((ArrayList)senddata[key]).Add(data.y);
            }
        }
        else
        {
            ArrayList a = new ArrayList();
            a.Add(data.x);
            a.Add(data.y);
            senddata.Add(key, a);
        }
    }

    private void Send()
    {
        if(senddata == null || senddata.Count <= 0)
        {
            return;
        }

        NetworkCommunicate.getInstance().ReqPipe((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, -1, senddata);

        senddata.Clear();
    }

    // data :  key:fromid, value:ArrayList(HashTable(客户端发送的管道数据))
    public void Receive(int userid, Hashtable data)
    {
        if(userid < 0)
        {
            foreach(DictionaryEntry v in data)
            {
                string from = (string)v.Key;
                int fromid = Convert.ToInt32(from);
                ArrayList a = (ArrayList)v.Value;
                for(int i=0;i<a.Count;i++)
                {
                    Hashtable h = (Hashtable)a[i];
                    foreach (DictionaryEntry vv in h)
                    {
                        string objectkey = (string)vv.Key;
                        NetObjectInterFace.IObjectPipeSync iops = PipeObjectCollector.getInstance().GetById(Convert.ToInt32(objectkey));
                        if(iops != null)
                        {
                            iops.ReceiveSync(fromid,(ArrayList)vv.Value);
                        }
                    }
                }
            }
        }
        else if(userid == (int)UserInfor.getInstance().UserId)
        {

        }
    }
}
