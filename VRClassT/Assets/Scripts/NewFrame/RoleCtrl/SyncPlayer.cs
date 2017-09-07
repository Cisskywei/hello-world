using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实现玩家网络同步 头 左右手 手指数据同步
/// </summary>
public class SyncPlayer : MonoBehaviour {

    public int userid = -1;

    public Transform head;
    public Transform lefthand;
    public Transform righthand;

    private int framespace = 4;  // 每5帧同步
    private int frame = 0;

    void Awake()
    {
        // 注册update 函数 
        NetMainLoop.Instance().AddUpdate(OnUpdate);

        posl = lefthand.position;
        rotl = lefthand.rotation;

        posr = righthand.position;
        rotr = righthand.rotation;

        posh = head.position;
        roth = head.rotation;

        lastleftpos = lefthand.position;
        lastleftrot = lefthand.rotation;

        lastrightpos = righthand.position;
        lastrightrot = righthand.rotation;

        lastheadpos = head.position;
        lastheadrot = head.rotation;
    }

    private void Start()
    {
    }

    public void Init(int userid)
    {
        this.userid = userid;
    }

    // Update is called once per frame
    void OnUpdate()
    {
        if (_syncstate == Enums.ObjectState.CanSend)
        {
            if (frame++ > framespace)
            {
                frame = 0;
                SendSync();
            }
        }
        else if (_syncstate == Enums.ObjectState.CanReceive)
        {
            DoSync();
        }
    }

    public Hashtable senddata = new Hashtable();
    public void SendSync()
    {
        sendHead();
        sendLeftHand();
        sendRightHand();

        if(senddata == null)
        {
            senddata = new Hashtable();
        }

        if(_headdata != null && _headdata.Count > 0)
        {
            if (senddata.ContainsKey("head"))
            {
                senddata["head"] = _headdata;
            }
            else
            {
                senddata.Add("head", _headdata);
            }
        }

        if (_lefthanddata != null && _lefthanddata.Count > 0)
        {
            if (senddata.ContainsKey("left"))
            {
                senddata["left"] = _lefthanddata;
            }
            else
            {
                senddata.Add("left", _lefthanddata);
            }
        }

        if (_righthanddata != null && _righthanddata.Count > 0)
        {
            if (senddata.ContainsKey("right"))
            {
                senddata["right"] = _righthanddata;
            }
            else
            {
                senddata.Add("right", _righthanddata);
            }
        }

        if (userid > 0 && senddata.Count > 0)
        {
            UnifiedSend.getInstance().AddPlayer(userid, senddata);
        }
    }

    private void DoSync()
    {
        syncHead();
        syncLeftHand();
        syncRightHand();
    }

    private float calspeed(Vector3 from, Vector3 to, float t)
    {
        float dis = Vector3.Distance(from, to);
        float allt = t + 0.4f;
        float speeddis = dis / allt;

        return speeddis;
    }

    // 接收同步
    private bool _issynclpos = false;
    private bool _issynclrot = false;
    private Vector3 posl;
    private Quaternion rotl;
    private float _lastlspeed = 16;
    private float _lspeed;
    private Queue<Vector3> poslcache = new Queue<Vector3>();
    private Queue<float> posltimecache = new Queue<float>();
    private Queue<Quaternion> rotlcache = new Queue<Quaternion>();
    private void syncLeftHand()
    {
        if (_issynclpos)
        {
            if (Vector3.Distance(posl, lefthand.position) < 0.001f)
            {
                lefthand.position = posl;

                if (poslcache.Count > 0)
                {
                    posl = poslcache.Dequeue();

                    float t = -1;
                    if (posltimecache.Count > 0)
                    {
                        t = posltimecache.Dequeue();
                    }

                    if (t < 0)
                    {
                        t = Time.deltaTime;
                    }

                    _lspeed = calspeed(lefthand.position, posl, t);

                    if (_lastlspeed >= 10)
                    {
                        _lastlspeed = _lspeed;
                    }

                    if (Math.Abs(_lastlspeed - _lspeed) > 0.01f)
                    {
                        _lastlspeed = Mathf.Lerp(_lastlspeed, _lspeed, Time.deltaTime);
                    }

                    lefthand.position = Vector3.MoveTowards(lefthand.position, posl, _lastlspeed * Time.deltaTime);
                }
                else
                {
                    _issynclpos = false;
                }
            }
            else
            {
                if (_lastlspeed > 10)
                {
                    _lastlspeed = _lspeed;
                }

                if (Math.Abs(_lastlspeed - _lspeed) > 0.01f)
                {
                    _lastlspeed = Mathf.Lerp(_lastlspeed, _lspeed, Time.deltaTime);
                }

                _lastlspeed = Mathf.Lerp(_lastlspeed, _lspeed, Time.deltaTime);
                lefthand.position = Vector3.MoveTowards(lefthand.position, posl, _lastlspeed * Time.deltaTime);
            }
        }

        if (_issynclrot)
        {
            if (Quaternion.Angle(rotl, lefthand.rotation) < 0.1f)
            {
                lefthand.rotation = rotl;

                if (rotlcache.Count > 0)
                {
                    rotl = rotlcache.Dequeue();
                }
                else
                {
                    _issynclrot = false;
                }
            }
            else
            {
                lefthand.rotation = Quaternion.Lerp(lefthand.rotation, rotl, 16 * Time.deltaTime);
            }
        }
    }

    private bool _issyncrpos = false;
    private bool _issyncrrot = false;
    private Vector3 posr;
    private Quaternion rotr;
    private float _lastrspeed = 16;
    private float _rspeed;
    private Queue<Vector3> posrcache = new Queue<Vector3>();
    private Queue<float> posrtimecache = new Queue<float>();
    private Queue<Quaternion> rotrcache = new Queue<Quaternion>();
    private void syncRightHand()
    {
        if (_issyncrpos)
        {
            if (Vector3.Distance(posr, righthand.position) < 0.001f)
            {
                righthand.position = posr;

                if (posrcache.Count > 0)
                {
                    posr = posrcache.Dequeue();

                    float t = -1;
                    if (posrtimecache.Count > 0)
                    {
                        t = posrtimecache.Dequeue();
                    }

                    if (t < 0)
                    {
                        t = Time.deltaTime;
                    }

                    _rspeed = calspeed(righthand.position, posr, t);

                    if (_lastrspeed >= 10)
                    {
                        _lastrspeed = _rspeed;
                    }

                    if (Math.Abs(_lastrspeed - _rspeed) > 0.01f)
                    {
                        _lastrspeed = Mathf.Lerp(_lastrspeed, _rspeed, Time.deltaTime);
                    }

                    righthand.position = Vector3.MoveTowards(righthand.position, posr, _lastrspeed * Time.deltaTime);
                }
                else
                {
                    _issyncrpos = false;
                }
            }
            else
            {
                if (_lastrspeed > 10)
                {
                    _lastrspeed = _rspeed;
                }

                if (Math.Abs(_lastrspeed - _rspeed) > 0.01f)
                {
                    _lastrspeed = Mathf.Lerp(_lastrspeed, _rspeed, Time.deltaTime);
                }

                _rspeed = Mathf.Lerp(_lastrspeed, _rspeed, Time.deltaTime);
                righthand.position = Vector3.MoveTowards(righthand.position, posr, _lastrspeed * Time.deltaTime);
            }
        }

        if (_issyncrrot)
        {
            if (Quaternion.Angle(rotr, righthand.rotation) < 0.1f)
            {
                righthand.rotation = rotr;

                if (rotrcache.Count > 0)
                {
                    rotr = rotrcache.Dequeue();
                }
                else
                {
                    _issyncrrot = false;
                }
            }
            else
            {
                righthand.rotation = Quaternion.Lerp(righthand.rotation, rotr, 16 * Time.deltaTime);
            }
        }
    }

    private bool _issynchpos = false;
    private bool _issynchrot = false;
    private Vector3 posh;
    private Quaternion roth;
    private float _lasthspeed = 16;
    private float _hspeed;
    private float _lasthtime;
    private Queue<Vector3> poshcache = new Queue<Vector3>();
    private Queue<float> poshtimecache = new Queue<float>();
    private Queue<Quaternion> rothcache = new Queue<Quaternion>();
    private void syncHead()
    {
        if (_issynchpos)
        {
            if (Vector3.Distance(posh, head.position) < 0.001f)
            {
                head.position = posh;

                if (poshcache.Count > 0)
                {
                    posh = poshcache.Dequeue();

                    float t = -1;
                    if (poshtimecache.Count > 0)
                    {
           //             t = poshtimecache.Dequeue() - _lasthtime;
                    }

                    if (t < 0)
                    {
                        t = Time.deltaTime;
                    }

                    _hspeed = calspeed(head.position, posh, t);

                    if (_lasthspeed >= 10)
                    {
                        _lasthspeed = _hspeed;
                    }

                    if (Math.Abs(_lasthspeed - _hspeed) > 0.01f)
                    {
                        _lasthspeed = Mathf.Lerp(_lasthspeed, _hspeed, Time.deltaTime);
                    }

                    head.position = Vector3.MoveTowards(head.position, posh, _lasthspeed * Time.deltaTime);
                }
                else
                {
                    _lasthtime = Time.time;
                    _issynchpos = false;
                }
            }
            else
            {
                if (_lasthspeed >= 10)
                {
                    _lasthspeed = _hspeed;
                }

                if (Math.Abs(_lasthspeed - _hspeed) > 0.01f)
                {
                    _lasthspeed = Mathf.Lerp(_lasthspeed, _hspeed, Time.deltaTime);
                }

                _lasthspeed = Mathf.Lerp(_lasthspeed, _hspeed, Time.deltaTime);
                head.position = Vector3.MoveTowards(head.position, posh, _lasthspeed * Time.deltaTime);
            }
        }
        else
        {
            _lasthtime = Time.time;
        }

        if (_issynchrot)
        {
            if (Quaternion.Angle(roth, head.rotation) < 0.1f)
            {
                head.rotation = roth;

                if (rothcache.Count > 0)
                {
                    roth = rothcache.Dequeue();
                }
                else
                {
                    _issynchrot = false;
                }
            }
            else
            {
                head.rotation = Quaternion.Lerp(head.rotation, roth, 16 * Time.deltaTime);
            }
        }
    }

    // 发送同步
    private Vector3 lastleftpos;
    private Quaternion lastleftrot;
    private Hashtable _lefthanddata = new Hashtable();
    private void sendLeftHand()
    {
        if (Vector3.Distance(lastleftpos, lefthand.position) > 0.01f)
        {
            lastleftpos = lefthand.position;

            if (_lefthanddata == null)
            {
                _lefthanddata = new Hashtable();
            }
            if (_lefthanddata.ContainsKey("px"))
            {
                _lefthanddata["px"] = lefthand.position.x;
            }
            else
            {
                _lefthanddata.Add("px", lefthand.position.x);
            }

            if (_lefthanddata.ContainsKey("py"))
            {
                _lefthanddata["py"] = lefthand.position.y;
            }
            else
            {
                _lefthanddata.Add("py", lefthand.position.y);
            }

            if (_lefthanddata.ContainsKey("pz"))
            {
                _lefthanddata["pz"] = lefthand.position.z;
            }
            else
            {
                _lefthanddata.Add("pz", lefthand.position.z);
            }

            if (_lefthanddata.ContainsKey("tp"))
            {
                _lefthanddata["tp"] = Time.time;
            }
            else
            {
                _lefthanddata.Add("tp", Time.time);
            }
        }

        if (Quaternion.Angle(lastleftrot, lefthand.rotation) > 0.1f)
        {
            lastleftrot = lefthand.rotation;

            if (_lefthanddata == null)
            {
                _lefthanddata = new Hashtable();
            }
            if (_lefthanddata.ContainsKey("rx"))
            {
                _lefthanddata["rx"] = lefthand.rotation.x;
            }
            else
            {
                _lefthanddata.Add("rx", lefthand.rotation.x);
            }

            if (_lefthanddata.ContainsKey("ry"))
            {
                _lefthanddata["ry"] = lefthand.rotation.y;
            }
            else
            {
                _lefthanddata.Add("ry", lefthand.rotation.y);
            }

            if (_lefthanddata.ContainsKey("rz"))
            {
                _lefthanddata["rz"] = lefthand.rotation.z;
            }
            else
            {
                _lefthanddata.Add("rz", lefthand.rotation.z);
            }

            if (_lefthanddata.ContainsKey("rw"))
            {
                _lefthanddata["rw"] = lefthand.rotation.w;
            }
            else
            {
                _lefthanddata.Add("rw", lefthand.rotation.w);
            }
        }
    }

    private Vector3 lastrightpos;
    private Quaternion lastrightrot;
    private Hashtable _righthanddata = new Hashtable();
    private void sendRightHand()
    {
        if (Vector3.Distance(lastrightpos, righthand.position) > 0.01f)
        {
            lastrightpos = righthand.position;

            if (_righthanddata == null)
            {
                _righthanddata = new Hashtable();
            }
            if (_righthanddata.ContainsKey("px"))
            {
                _righthanddata["px"] = righthand.position.x;
            }
            else
            {
                _righthanddata.Add("px", righthand.position.x);
            }

            if (_righthanddata.ContainsKey("py"))
            {
                _righthanddata["py"] = righthand.position.y;
            }
            else
            {
                _righthanddata.Add("py", righthand.position.y);
            }

            if (_righthanddata.ContainsKey("pz"))
            {
                _righthanddata["pz"] = righthand.position.z;
            }
            else
            {
                _righthanddata.Add("pz", righthand.position.z);
            }

            if (_righthanddata.ContainsKey("tp"))
            {
                _righthanddata["tp"] = Time.time;
            }
            else
            {
                _righthanddata.Add("tp", Time.time);
            }
        }

        if (Quaternion.Angle(lastrightrot, righthand.rotation) > 0.1f)
        {
            lastrightrot = righthand.rotation;

            if (_righthanddata == null)
            {
                _righthanddata = new Hashtable();
            }
            if (_righthanddata.ContainsKey("rx"))
            {
                _righthanddata["rx"] = righthand.rotation.x;
            }
            else
            {
                _righthanddata.Add("rx", righthand.rotation.x);
            }

            if (_righthanddata.ContainsKey("ry"))
            {
                _righthanddata["ry"] = righthand.rotation.y;
            }
            else
            {
                _righthanddata.Add("ry", righthand.rotation.y);
            }

            if (_righthanddata.ContainsKey("rz"))
            {
                _righthanddata["rz"] = righthand.rotation.z;
            }
            else
            {
                _righthanddata.Add("rz", righthand.rotation.z);
            }

            if (_righthanddata.ContainsKey("rw"))
            {
                _righthanddata["rw"] = righthand.rotation.w;
            }
            else
            {
                _righthanddata.Add("rw", righthand.rotation.w);
            }
        }
    }

    private Vector3 lastheadpos;
    private Quaternion lastheadrot;
    private Hashtable _headdata = new Hashtable();
    private void sendHead()
    {
        if (Vector3.Distance(lastheadpos, head.position) > 0.01f)
        {
            lastheadpos = head.position;

            if (_headdata == null)
            {
                _headdata = new Hashtable();
            }
            if (_headdata.ContainsKey("px"))
            {
                _headdata["px"] = head.position.x;
            }
            else
            {
                _headdata.Add("px", head.position.x);
            }

            if (_headdata.ContainsKey("py"))
            {
                _headdata["py"] = head.position.y;
            }
            else
            {
                _headdata.Add("py", head.position.y);
            }

            if (_headdata.ContainsKey("pz"))
            {
                _headdata["pz"] = head.position.z;
            }
            else
            {
                _headdata.Add("pz", head.position.z);
            }

            if (_headdata.ContainsKey("tp"))
            {
                _headdata["tp"] = Time.time;
            }
            else
            {
                _headdata.Add("tp", Time.time);
            }
        }

        if (Quaternion.Angle(lastheadrot, head.rotation) > 0.1f)
        {
            lastheadrot = head.rotation;

            if (_headdata == null)
            {
                _headdata = new Hashtable();
            }
            if (_headdata.ContainsKey("rx"))
            {
                _headdata["rx"] = head.rotation.x;
            }
            else
            {
                _headdata.Add("rx", head.rotation.x);
            }

            if (_headdata.ContainsKey("ry"))
            {
                _headdata["ry"] = head.rotation.y;
            }
            else
            {
                _headdata.Add("ry", head.rotation.y);
            }

            if (_headdata.ContainsKey("rz"))
            {
                _headdata["rz"] = head.rotation.z;
            }
            else
            {
                _headdata.Add("rz", head.rotation.z);
            }

            if (_headdata.ContainsKey("rw"))
            {
                _headdata["rw"] = head.rotation.w;
            }
            else
            {
                _headdata.Add("rw", head.rotation.w);
            }
        }
    }

    public void ReceiveSync(Hashtable d)
    {
        if(userid < 0)
        {
            return;
        }

        Hashtable data = (Hashtable)d[userid.ToString()];

        Hashtable h = null;
        Hashtable l = null;
        Hashtable r = null;

        if(data.ContainsKey("head"))
        {
            h = (Hashtable)data["head"];
        }

        if (data.ContainsKey("left"))
        {
            l = (Hashtable)data["left"];
        }

        if (data.ContainsKey("right"))
        {
            r = (Hashtable)data["right"];
        }

        if(h != null)
        {
            receive(h, poshcache, poshtimecache, rothcache);
            _issynchpos = true;
            _issynchrot = true;
        }

        if (l != null)
        {
            receive(l, poslcache, posltimecache, rotlcache);
            _issyncrpos = true;
            _issyncrrot = true;
        }

        if (r != null)
        {
            receive(r, posrcache, posrtimecache, rotrcache);
            _issynclpos = true;
            _issynclrot = true;
        }
    }

    private void receive(Hashtable data, Queue<Vector3> pos, Queue<float> t, Queue<Quaternion> rot)
    {
        float px = (float)Convert.ToDouble(data["px"]);
        float py = (float)Convert.ToDouble(data["py"]);
        float pz = (float)Convert.ToDouble(data["pz"]);
        Vector3 p = new Vector3(px, py, pz);
        pos.Enqueue(p);

        float tp = (float)Convert.ToDouble(data["tp"]);
        t.Enqueue(tp);

        float rx = (float)Convert.ToDouble(data["rx"]);
        float ry = (float)Convert.ToDouble(data["ry"]);
        float rz = (float)Convert.ToDouble(data["rz"]);
        float rw = (float)Convert.ToDouble(data["rw"]);
        Quaternion r = new Quaternion(rx, ry, rz, rw);
        rot.Enqueue(r);
    }

    // 网络控制相关
    public Enums.ObjectState _syncstate = Enums.ObjectState.CanReceive;
    public int locker = -1;                                                 // 物体锁

    // 根据模式控制相关
    public void ChangeObjectState(Enums.ObjectState state)
    {
        this._syncstate = state;
    }
}
