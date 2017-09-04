using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    // 场景物体的容器
    public class CollectionObject
    {

        public static CollectionObject getInstance()
        {
            return Singleton<CollectionObject>.getInstance();
        }

        private int _counter = 1;

        private Dictionary<string, GameObject> _allsynsobject = new Dictionary<string, GameObject>();
        private Dictionary<string, SceneObject> _allsceneobject = new Dictionary<string, SceneObject>();

        private Dictionary<string, GameObject> _allplayers = new Dictionary<string, GameObject>();

        public CollectionObject()
        {
            if (_allsynsobject == null)
            {
                _allsynsobject = new Dictionary<string, GameObject>();
            }
        }

        public void signSync(GameObject go)
        {
            if(_allsynsobject.ContainsKey(go.name))
            {
                _allsynsobject[go.name] = go;
            }
            else
            {
                _allsynsobject.Add(go.name, go);

                _counter++;
            }

            SceneObject so = go.GetComponent<SceneObject>();
            if(so!=null)
            {
                if (_allsceneobject.ContainsKey(go.name))
                {
                    _allsceneobject[go.name] = so;
                }
                else
                {
                    _allsceneobject.Add(go.name, so);
                }
            }
        }

        public void signSync(GameObject go, string key)
        {
            if (_allsynsobject.ContainsKey(key))
            {
                _allsynsobject[key] = go;
            }
            else
            {
                _counter++;
                _allsynsobject.Add(key, go);
            }

            SceneObject so = go.GetComponent<SceneObject>();
            if (so != null)
            {
                if (_allsceneobject.ContainsKey(key))
                {
                    _allsceneobject[key] = so;
                }
                else
                {
                    _allsceneobject.Add(key, so);
                }
            }
        }

        public GameObject getSyncObjectByKey(string key)
        {
            try
            {
                if (!_allsynsobject.ContainsKey(key))
                {
                    return null;
                }
            }catch
            {

            }
            
            return _allsynsobject[key];
        }

        public SceneObject getSceneObjectByKey(string key)
        {
            try
            {
                if (!_allsceneobject.ContainsKey(key))
                {
                    return null;
                }
            }
            catch
            {

            }

            return _allsceneobject[key];
        }


        // 向服务器同步场景
        public Hashtable SyncSceneToService(bool directsend = false)
        {
            if (_allsynsobject.Count <= 0)
            {
                return null;
            }

            Hashtable scenedata = new Hashtable();

            try
            {
                foreach (KeyValuePair<string, GameObject> v in _allsynsobject)
                {
                    SceneObject so = v.Value.GetComponent<SceneObject>();
                    if (so != null && so.sos != null)
                    {
                        var h = so.sos.Serialize(v.Value);
                        if (h != null)
                        {
                            scenedata.Add(v.Key, h);
                        }
                    }
                    else
                    {
                        //
                    }
                }

                if (directsend && (UserInfor.getInstance().RoomConnecter != null))
                {
                    // 直接发送
                    MsgModule.getInstance().req_msg(scenedata, UserInfor.getInstance().RoomConnecter, "InitScenes");
                }
            }
            catch
            {
                
            }

            return scenedata;
        }

    }
}