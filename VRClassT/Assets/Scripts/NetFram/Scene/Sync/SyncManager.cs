using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ko.NetFram
{
    public class SyncManager : MonoBehaviour
    {
        private static SyncManager _instance;
        public static SyncManager getInstance()
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("SyncObjectOnce").GetComponent<SyncManager>();
            }

            return _instance;
        }

        private ReciveObjectOnce _recivesync = ReciveObjectOnce.getInstance();
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(_recivesync != null)
            {
                _recivesync.SyncClientUpdate();
            }
        }

        private void OnDestroy()
        {
            if(_recivesync!=null)
            {
                ReciveObjectOnce.getInstance().StopSyncClient();
            }
        }
    }
}

