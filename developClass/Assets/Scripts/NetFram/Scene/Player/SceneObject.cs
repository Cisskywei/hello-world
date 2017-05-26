using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour {

    public SceneObjectServer sos;

    // 一次性发送所有改变数据 临时缓存
    public Hashtable _allchangeself = new Hashtable();

    private Vector3 _originposition;
    private Quaternion _originrotation;
    private Vector3 _originscale;

    // 发送控制相关
    private bool _ischangepos = false;
    private bool _ischangerot = false;
    private bool _ischangescale = false;

    public float diff = 0.02f;

    //临时同步方案
    private Vector3 _targetpos = new Vector3();
    private Vector3 _targetscale = new Vector3();
    private Quaternion _targetrot = new Quaternion();
    private bool _isstartsyncpos = false;
    private bool _isstartsyncrot = false;
    private bool _isstartsyncscale = false;

    // 同步速度控制
    private float _speedpos = 16f;

    private void Awake()
    {
        if(sos == null)
        {
            sos = new SceneObjectServer(transform.position, transform.localScale, transform.rotation);
        }

        _originposition = transform.position;
        _originrotation = transform.rotation;
        _originscale = transform.localScale;

        _targetpos = transform.position;
        _targetrot = transform.rotation;
        _targetscale = transform.localScale;

        CollectionObject.getInstance().signSync(gameObject);
    }

 //   // Use this for initialization
 //   void Start () {
		
	//}
	
	// Update is called once per frame
	void Update () {

        CheckChange();

        Sync3DInfor();

        Access3DInfor();

    }

    /// 两个方法 1，检测自己的同步 2，接收同步改变更改自己
    public void CheckChange()
    {
        if(sos == null || !sos.CheckCanSend())
        {
            return;
        }

        //检测位置 角度 缩放 改变
        if (System.Math.Abs(transform.rotation.x - _originrotation.x) > diff || System.Math.Abs(transform.rotation.y - _originrotation.y) > diff || System.Math.Abs(transform.rotation.z - _originrotation.z) > diff)
        {
            // 角度改变
            _originrotation = transform.rotation;
            _ischangerot = true;

            Quaternion r = transform.rotation;
            addchangevalue("rotx", r.x);
            addchangevalue("roty", r.y);
            addchangevalue("rotz", r.z);
            addchangevalue("rotw", r.w);
        }

        if (System.Math.Abs(transform.position.x - _originposition.x) > diff || System.Math.Abs(transform.position.y - _originposition.y) > diff || System.Math.Abs(transform.position.z - _originposition.z) > diff)
        {
            // 位置移动
            _originposition = transform.position;
            _ischangepos = true;

            Vector3 r = transform.position;
            addchangevalue("posx", r.x);
            addchangevalue("posy", r.y);
            addchangevalue("posz", r.z);
        }

        if (System.Math.Abs(transform.localScale.x - _originscale.x) > diff || System.Math.Abs(transform.localScale.y - _originscale.y) > diff || System.Math.Abs(transform.localScale.z - _originscale.z) > diff)
        {
            // 位置移动
            _originscale = transform.localScale;
            _ischangescale = true;

            Vector3 r = transform.localScale;
            addchangevalue("scalx", r.x);
            addchangevalue("scaly", r.y);
            addchangevalue("scalz", r.z);
        }

        if (_allchangeself.Count > 0)
        {
            if(_allchangeself["frametime"] != null)
            {
                _allchangeself["frametime"] = Time.time;
            }
            else
            {
                _allchangeself.Add("frametime", Time.time);
            }
            SyncObjectOnce.getInstance().AddObject(gameObject.name, _allchangeself);
        }
    }

    // 插值同步
    public void Sync3DInfor()
    {
        if(sos == null || !sos.CheckCanSync3DInfor())
        {
            return;
        }

        if (_isstartsyncpos)
        {
            if (Vector3.Distance(transform.position, _targetpos) < 0.04f)
            {
                transform.position = _targetpos;
                _isstartsyncpos = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _targetpos, _speedpos); //Time.deltaTime * 
            }
        }

        if (_isstartsyncrot)
        {
            if (Quaternion.Angle(transform.rotation, _targetrot) < 0.01f)
            {
                transform.rotation = _targetrot;
                _isstartsyncrot = false;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetrot, Time.deltaTime * 8);
            }
        }

        if (_isstartsyncscale)
        {
            transform.localScale = _targetscale;
        }

        //Debug.Log("插值同步");
    }

    // 获取3d 服务器同步数据信息
    public void Access3DInfor()
    {
        if(sos == null || !sos.CheckCanRecive())
        {
            return;
        }

        if(!sos.changeorno)
        {
            return;
        }

        _targetpos = sos.pos;
        _targetrot = sos.rot;
        _targetscale = sos.scale;

        calculatespeed();

        sos.changeorno = false;

        _isstartsyncpos = true;
        _ischangerot = true;
        _ischangescale = true;
    }

    // 操作监听返回
    public void listen_operate_result(string token, string typ, string result)
    {
        if (UserInfor.getInstance().UserToken != token)
        {
            sos.isCanOperate = false;
            sos.isCanReceive = true;
            sos.isCanSend = false;

            return;
        }

        if (typ == "release")
        {
            if (result == "yes")
            {
                sos.isCanSend = false;
                sos.isCanOperate = false;
                sos.isCanReceive = true;
            }
            else
            {
                //sos.isCanOperate = false;
                //sos.isCanReceive = true;
                //sos.isCanSend = false;
            }
        }
        else if (typ == "hold")
        {
            if (result == "yes")
            {
                sos.isCanOperate = true;
                sos.isCanReceive = false;
                sos.isCanSend = true;
            }
            else
            {
                //sos.isCanOperate = false;
                //sos.isCanReceive = true;
                //sos.isCanSend = false;
            }
        }
    }

    // 计算同步速度
    private float difft = 16;
    private void calculatespeed()
    {
        float t = (sos._curframetime - sos._lastframetime);

        if (t <= 0)
        {
            _speedpos = 16;
        }
        else
        {
            _speedpos = (Vector3.Distance(_targetpos, transform.position)) / t;
            difft = t;
        }
    }

    private void addchangevalue(string key, float value)
    {
        if (_allchangeself.ContainsKey(key))
        {
            _allchangeself[key] = value;
        }
        else
        {
            _allchangeself.Add(key, value);
        }
    }
}
