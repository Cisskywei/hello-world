using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    // 游戏物体的服务器场景数据
    public class SceneObjectServer
    {
        // 基本三维数据
        public Vector3 pos;
        public Vector3 scale;
        public Quaternion rot;

        public int state;  // 状态 具体含义有客户端自己定义
        public bool usegravity = true;
        public bool iskinematic;
        public bool changeorno;

        // 同步苏联
 //       public float speed = 16;
        public float _lastframetime = 0;
        public float _curframetime = 0;

        // 序列化辅助变量
        private Hashtable _selfhash = new Hashtable();

        // 根据模式控制收发状态
        public bool isCanReceive = true;
        public bool isCanSend = false;
        public bool isCanOperate = false;   //是否可操作物体  用于请求释放操作权限

        public SceneObjectServer()
        {

        }

        public SceneObjectServer(Vector3 pos, Vector3 scale, Quaternion rot)
        {
            this.pos = pos;
            this.scale = scale;
            this.rot = rot;

            changeorno = false;
        }

        public SceneObjectServer(Vector3 pos, Vector3 scale, Quaternion rot, int state, bool g, bool k)
        {
            this.pos = pos;
            this.scale = scale;
            this.rot = rot;

            this.state = state;
            usegravity = g;
            iskinematic = k;

            changeorno = false;
        }

        public bool CheckCanSend()
        {
            bool ret = UserInfor.getInstance().CheckCanSend();

            ret = ret && isCanSend;

            return ret;
        }

        public bool CheckCanRecive()
        {
            bool ret = UserInfor.getInstance().CheckCanRecive();

            ret = ret && isCanReceive;

            return ret;
        }

        public bool CheckCanSync3DInfor()
        {
            // 排除物理属性等的同步
            bool ret = UserInfor.getInstance().CheckCanSync3DInfor();

            ret = ret && isCanReceive;

            return ret;
        }

        public bool CheckCanOperate()
        {
            bool ret = UserInfor.getInstance().CheckCanOperate();

            ret = ret && isCanOperate;

            return ret;
        }

        // 序列化辅助函数
        public Hashtable Serialize(GameObject go)
        {
            if(go == null)
            {
                return null;
            }

            Transform tr = go.transform;

            if(_selfhash.Count <= 0)
            {
                _selfhash.Add("posx", tr.position.x);
                _selfhash.Add("posy", tr.position.y);
                _selfhash.Add("posz", tr.position.z);

                _selfhash.Add("rotx", tr.rotation.x);
                _selfhash.Add("roty", tr.rotation.y);
                _selfhash.Add("rotz", tr.rotation.z);
                _selfhash.Add("rotw", tr.rotation.w);

                _selfhash.Add("scalx", tr.localScale.x);
                _selfhash.Add("scaly", tr.localScale.y);
                _selfhash.Add("scalz", tr.localScale.z);

                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    _selfhash.Add("phycg", rb.useGravity);
                    _selfhash.Add("phyck", rb.isKinematic);
                }

                _selfhash.Add("state", this.state);
            }
            else
            {
                _selfhash["posx"] = tr.position.x;
                _selfhash["posy"] = tr.position.y;
                _selfhash["posz"] = tr.position.z;

                _selfhash["rotx"] = tr.rotation.x;
                _selfhash["roty"] = tr.rotation.y;
                _selfhash["rotz"] = tr.rotation.z;
                _selfhash["rotw"] = tr.rotation.w;

                _selfhash["scalx"] = tr.localScale.x;
                _selfhash["scaly"] = tr.localScale.y;
                _selfhash["scalz"] = tr.localScale.z;

                Rigidbody rb = go.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    _selfhash["phycg"] = rb.useGravity;
                    _selfhash["phyck"] = rb.isKinematic;
                }
                
                _selfhash["state"] = this.state;
            }
            

            return _selfhash;
        }

        // 接收服务器端的同步数据
        public void Deserialization(Hashtable t)
        {
            if (t == null || t.Count <= 0)
            {
                Debug.Log("接收服务器端的同步数据  t.Count <= 0");
                return;
            }

            pos.x = (float)Convert.ToDouble(t["posx"]);
            pos.y = (float)Convert.ToDouble(t["posy"]);
            pos.z = (float)Convert.ToDouble(t["posz"]);

            rot.x = (float)Convert.ToDouble(t["rotx"]);
            rot.y = (float)Convert.ToDouble(t["roty"]);
            rot.z = (float)Convert.ToDouble(t["rotz"]);
            rot.w = (float)Convert.ToDouble(t["rotw"]);

            scale.x = (float)Convert.ToDouble(t["scalx"]);
            scale.y = (float)Convert.ToDouble(t["scaly"]);
            scale.z = (float)Convert.ToDouble(t["scalz"]);

            usegravity = (bool)Convert.ToBoolean(t["phycg"]);
            iskinematic = (bool)Convert.ToBoolean(t["phyck"]);

            state = (int)Convert.ToInt64(t["state"]);

            _lastframetime = _curframetime;
            _curframetime = (float)Convert.ToDouble(t["frametime"]);
            if(_lastframetime <= 0)
            {
                _lastframetime = _curframetime;
            }

            changeorno = true;
        }
    }
}
