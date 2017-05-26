using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class Container
    {

        public static Container getInstance()
        {
            return Singleton<Container>.getInstance();
        }

        private int _counter = 1;

        private Dictionary<string, GameObject> _allsynsobject = new Dictionary<string, GameObject>();

        public Container()
        {
            if (_allsynsobject == null)
            {
                _allsynsobject = new Dictionary<string, GameObject>();
            }
        }

        public int signSync(GameObject go)
        {
            _counter++;

            _allsynsobject.Add(go.name, go);

            return _counter;
        }

        public void signSync(GameObject go, string key)
        {
            _counter++;

            if (_allsynsobject.ContainsKey(key))
            {
                _allsynsobject[key] = go;
            }
        }

        public GameObject getSyncObjectById(string key)
        {
            if (!_allsynsobject.ContainsKey(key))
            {
                return null;
            }
            return _allsynsobject[key];
        }


        // 向服务器同步场景
        public Hashtable SyncSceneToService(bool directsend = false)
        {
            Debug.Log(_allsynsobject.Count);

            if(_allsynsobject.Count <= 0)
            {
                return null;
            }

            Hashtable scenedata = new Hashtable();

            foreach(KeyValuePair<string,GameObject> v in _allsynsobject)
            {
                scenedata.Add(v.Key, Serialize(v.Value));
            }

            if (directsend && (UserInfor.getInstance().RoomConnecter != null))
            {
                // 直接发送
                MsgModule.getInstance().req_msg(scenedata, UserInfor.getInstance().RoomConnecter, "InitScenes");
            }

            return scenedata;
        }

        // 序列化辅助函数
        private Hashtable Serialize(GameObject go)
        {
            Hashtable t = new Hashtable();

            Transform tr = go.transform;

            t.Add("posx", tr.position.x);
            t.Add("posy", tr.position.y);
            t.Add("posz", tr.position.z);

            t.Add("rotx", tr.rotation.x);
            t.Add("roty", tr.rotation.y);
            t.Add("rotz", tr.rotation.z);
            t.Add("rotw", tr.rotation.w);

            t.Add("scalx", tr.localScale.x);
            t.Add("scaly", tr.localScale.y);
            t.Add("scalz", tr.localScale.z);

            return t;
        }

    }
}

