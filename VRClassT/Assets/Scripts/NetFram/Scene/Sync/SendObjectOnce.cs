using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class SendObjectOnce : MonoBehaviour
    {
        private static SendObjectOnce _instance;
        public static SendObjectOnce getInstance()
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("SyncObjectOnce").GetComponent<SendObjectOnce>();
            }

            return _instance;
        }

        private Hashtable _allchange = new Hashtable();
        private Hashtable _playerchange = new Hashtable();
        private Hashtable _objectchange = new Hashtable();
        // 发送频率
        public float syncfreq = 0.01f;
        private float timer = 0;

        private readonly string objectkey = "objects";
        private readonly string playerkey = "player";

        //// Use this for initialization
        //void Start()
        //{

        //}

        // Update is called once per frame
        void Update()
        {
            if (timer >= syncfreq)
            {
                timer = 0;

                if(_objectchange.Count > 0)
                {
                    _allchange.Add(objectkey, _objectchange);
                }

                if (_playerchange.Count > 0)
                {
                    _allchange.Add(playerkey, _playerchange);
                }

                if (_allchange.Count > 0)
                {
                    MsgModule.getInstance().req_all_change_once(_allchange);

                    clearAll();
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        public void AddObject(string name, Hashtable change)
        {
            if (_objectchange.ContainsKey(name))
            {
                _objectchange[name] = change;
            }
            else
            {
                _objectchange.Add(name, change);
            }
        }

        public void AddPlayer(string name, Hashtable change)
        {
            if (_playerchange.ContainsKey(name))
            {
                _playerchange[name] = change;
            }
            else
            {
                _playerchange.Add(name, change);
            }
        }

        public void clearAll()
        {
            try
            {
                if (_objectchange.Count > 0)
                {
                    foreach (DictionaryEntry de in _objectchange)
                    {
                        Hashtable h = de.Value as Hashtable;
                        h.Clear();
                    }

                    _objectchange.Clear();
                }

                if (_playerchange.Count > 0)
                {
                    foreach (DictionaryEntry de in _playerchange)
                    {
                        Hashtable h = de.Value as Hashtable;
                        h.Clear();
                    }

                    _playerchange.Clear();
                }

                if (_allchange.Count > 0)
                {
                    foreach (DictionaryEntry de in _allchange)
                    {
                        Hashtable h = de.Value as Hashtable;
                        h.Clear();
                    }

                    _allchange.Clear();
                }
            }catch
            {

            }
        }

        private void OnDestroy()
        {
            clearAll();
        }
    }
}

