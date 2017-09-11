using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncObject : MonoBehaviour, NetObjectInterFace.IObjectSync
{
    public int selfid = -1;

    private Queue<Vector3> poscache = new Queue<Vector3>();
    private Queue<float> postimecache = new Queue<float>();
    private Queue<Quaternion> rotcache = new Queue<Quaternion>();
    private Vector3 pos;
    private Quaternion rot;

    private int framespace = 4;  // 每5帧同步
    private int frame = 0;

    void Awake()
    {
        if(selfid > 0)
        {
            ObjectCollector.getInstance().Add(selfid, gameObject);
        }

        // 注册update 函数 
        NetMainLoop.Instance().AddUpdate(OnUpdate);

        pos = transform.position;
        rot = transform.rotation;

        lastpos = transform.position;
        lastrot = transform.rotation;

        //Rig = gameObject.GetComponent<Rigidbody>();

        Debug.Log(selfid + gameObject.name);
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void OnUpdate () {

        if(_syncstate == Enums.ObjectState.CanSend)
        {
            if (frame++ > framespace)
            {
                frame = 0;
                SendSync();
            }
        }
        else if(_syncstate == Enums.ObjectState.CanReceive)
        {
            DoSync();
        }
    }

    private Vector3 lastpos;
    private Quaternion lastrot;
    private float lastrottime = -1;
    private float lastpostime = -1;
    private Hashtable senddata = new Hashtable();
    public void SendSync()
    {
        if(Vector3.Distance(lastpos,transform.position)>0.01f)
        {
            lastpos = transform.position;

            if(senddata == null)
            {
                senddata = new Hashtable();
            }
            if(senddata.ContainsKey("px"))
            {
                senddata["px"] = transform.position.x;
            }
            else
            {
                senddata.Add("px", transform.position.x);
            }

            if (senddata.ContainsKey("py"))
            {
                senddata["py"] = transform.position.y;
            }
            else
            {
                senddata.Add("py", transform.position.y);
            }

            if (senddata.ContainsKey("pz"))
            {
                senddata["pz"] = transform.position.z;
            }
            else
            {
                senddata.Add("pz", transform.position.z);
            }

            float t = -1;
            if(lastpostime < 0)
            {
                lastpostime = Time.time;
            }
            else
            {
                t = Time.time - lastpostime;
                lastpostime = Time.time;
            }

            if (senddata.ContainsKey("tp"))
            {
                senddata["tp"] = t;
            }
            else
            {
                senddata.Add("tp", t);
            }
        }
        else
        {
            lastpostime = -1;
        }

        if (Quaternion.Angle(lastrot, transform.rotation) > 0.1f)
        {
            lastrot = transform.rotation;

            if (senddata == null)
            {
                senddata = new Hashtable();
            }
            if (senddata.ContainsKey("rx"))
            {
                senddata["rx"] = transform.rotation.x;
            }
            else
            {
                senddata.Add("rx", transform.rotation.x);
            }

            if (senddata.ContainsKey("ry"))
            {
                senddata["ry"] = transform.rotation.y;
            }
            else
            {
                senddata.Add("ry", transform.rotation.y);
            }

            if (senddata.ContainsKey("rz"))
            {
                senddata["rz"] = transform.rotation.z;
            }
            else
            {
                senddata.Add("rz", transform.rotation.z);
            }

            if (senddata.ContainsKey("rw"))
            {
                senddata["rw"] = transform.rotation.w;
            }
            else
            {
                senddata.Add("rw", transform.rotation.w);
            }
        }

        if(selfid >= 0 && senddata.Count > 0)
        {
            UnifiedSend.getInstance().AddObject(selfid, senddata);
        }
    }

    //private int syncRateFrames = 16;
    //private int lerpTime = 16;
    //private int LerpStep = 1;
    //private float t = 1;

 //   private Vector3 speed;
    private float speeddis;
    private float lastspeed = 10;
    private float dis = 1;
    private float allt = 1;
    public void calspeed(Vector3 from, Vector3 to, float t)
    {
        dis = Vector3.Distance(from, to);
    //    speed = (to - from).normalized;
        allt = t + 0.4f;
        speeddis = dis / allt;

        //if(lastspeed<0)
        //{
        //    lastspeed = speeddis;
        //}
    }

    private float diff = 0.2f;
    public void DoSync()
    {
        if(_issyncpos)
        {
            if (Vector3.Distance(pos, transform.position) < 0.001f)
            {
                transform.position = pos;

                if (poscache.Count > 0)
                {
          //          diff = 0.4f;
           //         lerpTime = syncRateFrames;
                    pos = poscache.Dequeue();

                    float t = -1;
                    if (postimecache.Count > 0)
                    {
                        t = postimecache.Dequeue();
                    }

                    if (t < 0)
                    {
                        t = Time.deltaTime;
                    }

                    calspeed(transform.position, pos, t);

                    if (lastspeed >= 10)
                    {
                        lastspeed = speeddis;
                    }

                    if (Math.Abs(lastspeed - speeddis) > 0.01f)
                    {
                        lastspeed = Mathf.Lerp(lastspeed, speeddis, Time.deltaTime);
                    }

                    transform.position = Vector3.MoveTowards(transform.position, pos, lastspeed * Time.deltaTime);
                }
                else
                {
                    _issyncpos = false;
                }
            }
            else
            {
                //speed = (pos - transform.position).normalized;
                //diff = ( dis * Time.deltaTime ) / allt;
                //transform.Translate(diff * speed);

                if (lastspeed < 0)
                {
                    lastspeed = speeddis;
                }

                if (Math.Abs(lastspeed - speeddis) > 0.01f)
                {
                    lastspeed = Mathf.Lerp(lastspeed, speeddis, Time.deltaTime);
                }

                lastspeed = Mathf.Lerp(lastspeed, speeddis, Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, pos, lastspeed * Time.deltaTime);

                //transform.position = Vector3.Lerp(transform.position, pos, 12 * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, pos, 0.1f);

                //lerpTime -= LerpStep;
                //if (lerpTime < 0)
                //{

                //}
                //else
                //{
                //    t = (float)(syncRateFrames - lerpTime) / (float)syncRateFrames;
                //    transform.position = Vector3.Lerp(transform.position, pos, t);
                //}
            }
        }
        
        if(_issyncrot)
        {
            if (Quaternion.Angle(rot, transform.rotation) < 0.1f)
            {
                transform.rotation = rot;

                if (rotcache.Count > 0)
                {
                    rot = rotcache.Dequeue();
                }
                else
                {
                    _issyncrot = false;
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 16 * Time.deltaTime);
            }
        }
        
    }

    //public Rigidbody Rig;
    //private void DoSyncPhy()
    //{
    //    if (Vector3.Distance(pos, transform.position) < 0.1f)
    //    {
    //        if (poscache.Count > 0)
    //        {
    //            pos = poscache.Dequeue();
    //        }
    //    }

    //    if (poscache.Count > 0)
    //    {
    //        Rig.velocity = Vector3.Lerp(Rig.velocity, new Vector3(
    //        pos.x - transform.position.x,
    //        pos.y - transform.position.y,
    //        pos.z - transform.position.z
    //        ).normalized * (8), 0.1f);
    //    }
    //    else
    //    {
    //        Rig.velocity = Vector2.Lerp(Rig.velocity, Vector2.zero, 0.1f);
    //    }

    //}

    //private Tweener _move;
    //private bool _istartmove = false;
    //private void DoSyncDoTween()
    //{
    //    Debug.Log("_istartmove" + _istartmove);
    //    Debug.Log("poscache.Count" + poscache.Count);

    //    if(poscache == null || poscache.Count <= 0)
    //    {
    //        Debug.Log("poscache == null || poscache.Count <= 0");
    //        _istartmove = false;
    //        return;
    //    }

    //    if (_istartmove)
    //    {
    //        return;
    //    }

    //    _istartmove = true;

    //    pos = poscache.Dequeue();

    //    float t = -1;
    //    if(postimecache.Count > 0)
    //    {
    //        t = postimecache.Dequeue();
    //    }
        
    //    if(t<0)
    //    {
    //        t = 0.1f;
    //    }

    //    _move = transform.DOMove(pos, t);
    //    _move.OnComplete(moveend);

    //    //if (_move == null)
    //    //{
    //    //    _move = transform.DOMove(pos, t);
    //    //    _move.SetEase(Ease.Linear);
    //    //    _move.OnComplete(moveend);
    //    //}
    //    //else
    //    //{
    //    //    _move.ChangeEndValue(pos);
    //    //    _move.Restart();
    //    //}

    //    Debug.Log("lllll" + poscache.Count);
    //}

    //private void moveend()
    //{
    //    Debug.LogError("moveend" + poscache.Count);
    //    _istartmove = false;
    //    DoSyncDoTween();
    //}

    private bool _issyncpos = false;
    private bool _issyncrot = false;
    public void ReceiveSync(Hashtable data)
    {
        float px = (float)Convert.ToDouble(data["px"]);
        float py = (float)Convert.ToDouble(data["py"]);
        float pz = (float)Convert.ToDouble(data["pz"]);

        Vector3 p = new Vector3(px, py, pz);
        poscache.Enqueue(p);
        //pos = p;

        float tp = (float)Convert.ToDouble(data["tp"]);
        postimecache.Enqueue(tp);

        float rx = (float)Convert.ToDouble(data["rx"]);
        float ry = (float)Convert.ToDouble(data["ry"]);
        float rz = (float)Convert.ToDouble(data["rz"]);
        float rw = (float)Convert.ToDouble(data["rw"]);

        Quaternion r = new Quaternion(rx, ry, rz, rw);
        rotcache.Enqueue(r);

        _issyncpos = true;
        _issyncrot = true;

        //DoSyncDoTween();
    }

    // 转换hashtable
    public Hashtable GetHash()
    {
        Hashtable h = new Hashtable();

        h.Add("px", transform.position.x);

        h.Add("py", transform.position.y);

        h.Add("pz", transform.position.z);

        h.Add("rx", transform.rotation.x);

        h.Add("ry", transform.rotation.y);

        h.Add("rz", transform.rotation.z);

        h.Add("rw", transform.rotation.w);

        return h;
    }

    // 网络控制相关
    public Enums.ObjectState _syncstate = Enums.ObjectState.CanReceive;
    public int locker = -1;                                                 // 物体锁

    // 根据模式控制相关
    public void ChangeObjectState(Enums.ObjectState state)
    {
        this._syncstate = state;
    }

    public bool CheckLocker(int user)
    {
        return user == locker;
    }
}
