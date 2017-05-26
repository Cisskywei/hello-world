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

        private Thread _recivesync;
        // Use this for initialization
        void Start()
        {
            _recivesync = new Thread(ReciveObjectOnce.getInstance().SyncClientUpdate);
            _recivesync.Start();
        }

        //// Update is called once per frame
        //void Update () {

        //}

        private void OnDestroy()
        {
            if(_recivesync!=null)
            {
                ReciveObjectOnce.getInstance().StopSyncClient();
            }
        }
    }
}

