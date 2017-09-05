using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerOld : MonoBehaviour {

    public Transform head;
    public Transform lefthand;
    public Transform righthand;

    public float diff = 0.4f;
    public float freq = 0.02f;
    private float _timer = 0;

    private Vector3 _headoripos;
    private Quaternion _headorirot;
    private Vector3 _headoriscal;

    private Vector3 _lefthandoripos;
    private Quaternion _lefthandorirot;
    private Vector3 _lefthandoriscal;

    private Vector3 _righthandoripos;
    private Quaternion _righthandorirot;
    private Vector3 _righthandoriscal;


    private Hashtable headchange = new Hashtable();
    private Hashtable leftchange = new Hashtable();
    private Hashtable rightchange = new Hashtable();

    // Use this for initialization
    void Start () {

        if(head != null)
        {
            _headoripos = head.position;
            _headorirot = head.rotation;
            _headoriscal = head.localScale;
        }

        if(lefthand != null)
        {
            _lefthandoripos = lefthand.position;
            _lefthandorirot = lefthand.rotation;
            _lefthandoriscal = lefthand.localScale;
        }
        
        if(righthand != null)
        {
            _righthandoripos = righthand.position;
            _righthandorirot = righthand.rotation;
            _righthandoriscal = righthand.localScale;
        }
        
    }
	
	// Update is called once per frame
	void Update () {

        if (_timer > freq)
        {
            _timer = 0;

            CheckChange();
        }

        _timer += Time.deltaTime;
    }

    public void CheckHeadChange()
    {
        if (head == null)
        {
            return;
        }

        //检测位置 角度 缩放 改变
        if (System.Math.Abs(head.rotation.x - _headorirot.x) > diff || System.Math.Abs(head.rotation.y - _headorirot.y) > diff || System.Math.Abs(head.rotation.z - _headorirot.z) > diff)
        {
            // 角度改变
            _headorirot = head.rotation;

            Quaternion r = head.rotation;
            addheadchangevalue("headrotx", r.x);
            addheadchangevalue("headroty", r.y);
            addheadchangevalue("headrotz", r.z);
            addheadchangevalue("headrotw", r.w);
        }

        if (System.Math.Abs(head.position.x - _headoripos.x) > diff || System.Math.Abs(head.position.y - _headoripos.y) > diff || System.Math.Abs(head.position.z - _headoripos.z) > diff)
        {
            // 位置移动
            _headoripos = head.position;

            Vector3 r = head.position;
            addheadchangevalue("headposx", r.x);
            addheadchangevalue("headposy", r.y);
            addheadchangevalue("headposz", r.z);
        }

        //if (System.Math.Abs(head.localScale.x - _headoriscal.x) > diff || System.Math.Abs(head.localScale.y - _headoriscal.y) > diff || System.Math.Abs(head.localScale.z - _headoriscal.z) > diff)
        //{
        //    // 位置移动
        //    _headoriscal = head.localScale;

        //    Vector3 r = head.localScale;
        //    addheadchangevalue("headscalx", r.x);
        //    addheadchangevalue("headscaly", r.y);
        //    addheadchangevalue("headscalz", r.z);
        //}

        if (headchange.Count > 0)
        {
            SendObjectOnce.getInstance().AddPlayer("head", headchange);
        }
    }

    public void CheckLeftChange()
    {
        if (lefthand == null)
        {
            return;
        }

        //检测位置 角度 缩放 改变
        if (System.Math.Abs(lefthand.rotation.x - _lefthandorirot.x) > diff || System.Math.Abs(lefthand.rotation.y - _lefthandorirot.y) > diff || System.Math.Abs(lefthand.rotation.z - _lefthandorirot.z) > diff)
        {
            // 角度改变
            _lefthandorirot = lefthand.rotation;

            Quaternion r = lefthand.rotation;
            addleftchangevalue("lhandrotx", r.x);
            addleftchangevalue("lhandroty", r.y);
            addleftchangevalue("lhandrotz", r.z);
            addleftchangevalue("lhandrotw", r.w);
        }

        if (System.Math.Abs(lefthand.position.x - _lefthandoripos.x) > diff || System.Math.Abs(lefthand.position.y - _lefthandoripos.y) > diff || System.Math.Abs(lefthand.position.z - _lefthandoripos.z) > diff)
        {
            // 位置移动
            _lefthandoripos = lefthand.position;

            Vector3 r = lefthand.position;
            addleftchangevalue("lhandposx", r.x);
            addleftchangevalue("lhandposy", r.y);
            addleftchangevalue("lhandposz", r.z);
        }

        //if (System.Math.Abs(lefthand.localScale.x - _lefthandoriscal.x) > diff || System.Math.Abs(lefthand.localScale.y - _lefthandoriscal.y) > diff || System.Math.Abs(lefthand.localScale.z - _lefthandoriscal.z) > diff)
        //{
        //    // 位置移动
        //    _lefthandoriscal = lefthand.localScale;

        //    Vector3 r = lefthand.localScale;
        //    addleftchangevalue("lhandscalx", r.x);
        //    addleftchangevalue("lhandscaly", r.y);
        //    addleftchangevalue("lhandscalz", r.z);
        //}

        if (leftchange.Count > 0)
        {
            SendObjectOnce.getInstance().AddPlayer("lefthand", leftchange);
        }
    }

    public void CheckRightChange()
    {
        if (righthand == null)
        {
            return;
        }

        //检测位置 角度 缩放 改变
        if (System.Math.Abs(righthand.rotation.x - _righthandorirot.x) > diff || System.Math.Abs(righthand.rotation.y - _righthandorirot.y) > diff || System.Math.Abs(righthand.rotation.z - _righthandorirot.z) > diff)
        {
            // 角度改变
            _righthandorirot = righthand.rotation;

            Quaternion r = righthand.rotation;
            addrightchangevalue("rhandrotx", r.x);
            addrightchangevalue("rhandroty", r.y);
            addrightchangevalue("rhandrotz", r.z);
            addrightchangevalue("rhandrotw", r.w);
        }

        if (System.Math.Abs(righthand.position.x - _righthandoripos.x) > diff || System.Math.Abs(righthand.position.y - _righthandoripos.y) > diff || System.Math.Abs(righthand.position.z - _righthandoripos.z) > diff)
        {
            // 位置移动
            _righthandoripos = righthand.position;

            Vector3 r = righthand.position;
            addrightchangevalue("rhandposx", r.x);
            addrightchangevalue("rhandposy", r.y);
            addrightchangevalue("rhandposz", r.z);
        }

        //if (System.Math.Abs(righthand.localScale.x - _righthandoriscal.x) > diff || System.Math.Abs(righthand.localScale.y - _righthandoriscal.y) > diff || System.Math.Abs(righthand.localScale.z - _righthandoriscal.z) > diff)
        //{
        //    // 位置移动
        //    _righthandoriscal = righthand.localScale;

        //    Vector3 r = righthand.localScale;
        //    addrightchangevalue("rhandscalx", r.x);
        //    addrightchangevalue("rhandscaly", r.y);
        //    addrightchangevalue("rhandscalz", r.z);
        //}

        if (rightchange.Count > 0)
        {
            SendObjectOnce.getInstance().AddPlayer("righthand", rightchange);
        }
    }

    private void addheadchangevalue(string key, float value)
    {
        if (headchange.ContainsKey(key))
        {
            headchange[key] = value;
        }
        else
        {
            headchange.Add(key, value);
        }
    }

    private void addleftchangevalue(string key, float value)
    {
        if (leftchange.ContainsKey(key))
        {
            leftchange[key] = value;
        }
        else
        {
            leftchange.Add(key, value);
        }
    }

    private void addrightchangevalue(string key, float value)
    {
        if (rightchange.ContainsKey(key))
        {
            rightchange[key] = value;
        }
        else
        {
            rightchange.Add(key, value);
        }
    }

    public void CheckChange()
    {
        if (!UserInfor.getInstance().CheckCanSend())
        {
            return;
        }

        CheckHeadChange();
        CheckLeftChange();
        CheckRightChange();
    }
}
