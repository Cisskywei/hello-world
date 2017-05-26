using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ko.NetFram
{
    public class SyncObject : MonoBehaviour
    {
        public int _selfid = -1;

        //标记是否有操作权限
        public bool isctrl = false;


        // 标记是否本地物理模拟
        public bool ispyh = false;
        // 标记在释放时是否 还未到达终点
        public bool isppppp = false;

        private Vector3 _originposition;
        private Quaternion _originrotation;
        private Vector3 _originscale;

        private const float diff = 0.02f;
        private string _uuid;
        private string connector;
        public string Connector
        {
            get
            {
                return connector;
            }
            set
            {
                connector = value;
                Debug.Log(connector + "connector");
            }
        }

        // 发送控制相关
        private bool _ischangepos = false;
        private bool _ischangerot = false;

        //临时同步方案
        private Vector3 _targetpos = new Vector3();
        private Quaternion _targetrot = new Quaternion();
        private bool _isstartsyncpos = false;
        private bool _isstartsyncrot = false;


        // 一次性发送所有改变数据 临时缓存
        public Hashtable _allchangeself = new Hashtable();

        private void Awake()
        {
            _originposition = transform.position;
            _originrotation = transform.rotation;
            _originscale = transform.localScale;

            // 注册自己
            if (_selfid >= 0)
            {
                Container.getInstance().signSync(gameObject);
            }
        }

        // Use this for initialization
        void Start()
        {
            _targetpos.z = 1024;
        }
        // Update is called once per frame
        void Update()
        {
            if(ispyh)
            {
                // 物理模拟直接返回
                return;
            }

            if (!isctrl)
            {

                if (_isstartsyncpos && _targetpos.z != 1024)
                {
                    if (Vector3.Distance(transform.position, _targetpos) < 0.04f)
                    {
                        transform.position = _targetpos;
                        _isstartsyncpos = false;
                        _targetpos.z = 1024;

                        if(isppppp)
                        {
                            ispyh = true;
                            isppppp = false;

                            gameObject.GetComponent<Rigidbody>().useGravity = true;
                        }
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, _targetpos, Time.deltaTime * 16);
                    }
                }

                if (_isstartsyncrot)
                {
                    if (Quaternion.Angle(transform.rotation, _targetrot) < 0.001f)
                    {
                        transform.rotation = _targetrot;
                        _isstartsyncrot = false;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, _targetrot, Time.deltaTime * 8);
                    }
                }

                return;
            }

            if (_uuid == null)
            {
                _uuid = UserInfor.getInstance().UserUuid;
            }
            if (_uuid == null)
            {
                return;
            }

            if (connector == null)
            {
                connector = UserInfor.getInstance().RoomConnecter;
            }
            if (connector == null)
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

            if(_allchangeself.Count > 0)
            {
                SyncObjectOnce.getInstance().AddObject(gameObject.name, _allchangeself);

    //            _allchangeself.Clear();
            }

            //if (_ischangerot && _ischangepos)
            //{
            //    MsgModule.getInstance().req_pos_rot(gameObject.name, transform.position, transform.rotation);
            //    _ischangerot = false;
            //    _ischangepos = false;
            //}else if (_ischangerot && !_ischangepos)
            //{
            //    MsgModule.getInstance().req_rotation(gameObject.name, transform.rotation);
            //    _ischangerot = false;
            //}
            //else if (_ischangepos && !_ischangerot)
            //{
            //    MsgModule.getInstance().req_position(gameObject.name, transform.position);
            //    _ischangepos = false;
            //}

            //if (System.Math.Abs(transform.localScale.x - _originscale.x) > diff || System.Math.Abs(transform.localScale.y - _originscale.y) > diff || System.Math.Abs(transform.localScale.z - _originscale.z) > diff)
            //{
            //    // 缩放改变
            //    MsgModule.getInstance().req_sync_vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z, Config.SyncTransform.SCALE, _selfid, _uuid, connector);
            //    _originscale = transform.localScale;
            //}

        }

        public void addchangevalue(string key, float value)
        {
            if(_allchangeself.ContainsKey(key))
            {
                _allchangeself[key] = value;
            }
            else
            {
                _allchangeself.Add(key, value);
            }
        }

        // 反序列化自己
        public void Deserialization(Hashtable t)
        {
            if(isctrl)
            {
                return;
            }

            if (t == null || t.Count <= 0)
            {
                Debug.Log("收到同步消息指令 直接返回");
                return;
            }

            _targetpos.x = (float)Convert.ToDouble(t["posx"]);
            _targetpos.y = (float)Convert.ToDouble(t["posy"]);
            _targetpos.z = (float)Convert.ToDouble(t["posz"]);

            _isstartsyncpos = true;

            _targetrot.x = (float)Convert.ToDouble(t["rotx"]);
            _targetrot.y = (float)Convert.ToDouble(t["roty"]);
            _targetrot.z = (float)Convert.ToDouble(t["rotz"]);
            _targetrot.w = (float)Convert.ToDouble(t["rotw"]);

            _isstartsyncrot = true;

        }

        public void request_operation_permissions()
        {
            MsgModule.getInstance().req_operation_permission(UserInfor.getInstance().UserToken, gameObject.name, UserInfor.getInstance().UserUuid);
        }

        public void request_operation_release()
        {
            MsgModule.getInstance().req_operation_release(UserInfor.getInstance().UserToken, gameObject.name, UserInfor.getInstance().UserUuid);
        }

        public void listen_operation_result(string token, string typ, string result)
        {
            Debug.Log("操作请求类型" + typ + "操作请求结果" + result);

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            if (UserInfor.getInstance().UserToken != token)
            {
                // 不是自己
                // 对物理模拟特殊处理
                if (rb != null)
                {
                    if (typ == "release")
                    {
                        if (result == "yes")
                        {
                            if(_isstartsyncpos)
                            {
                                isppppp = true;
                                isctrl = false;
                                ispyh = false;
                                rb.useGravity = false;
                            }
                            else
                            {
                                isctrl = false;
                                ispyh = true;
                                rb.useGravity = true;
                            }
                            
                            rb.isKinematic = false;
                        }
                        else
                        {
                            ispyh = false;
                        }
                    }
                    else if (typ == "hold")
                    {
                        if (result == "yes")
                        {
                            ispyh = false;
                            rb.useGravity = false;
                            rb.isKinematic = false;
                            isctrl = false;
                        }
                        else
                        {
                            isctrl = false;
                            ispyh = false;
                            rb.useGravity = true;
                            rb.isKinematic = false;
                        }
                    }
                }

                return;
            }

            if (typ == "release")
            {
                if (result == "yes")
                {
                    ispyh = true;
                    isctrl = false;
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    _targetpos.z = 1024;
                }
                else
                {
                    //
                    ispyh = true;
                    isctrl = false;
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    _targetpos.z = 1024;
                }
            }
            else if (typ == "hold")
            {
                if (result == "yes")
                {
                    ispyh = false;
                    isctrl = true;
                    rb.useGravity = false;
                    rb.isKinematic = false;
                }
                else
                {
                    ispyh = true;
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    isctrl = false;
                }
            }
        }

    }
}

