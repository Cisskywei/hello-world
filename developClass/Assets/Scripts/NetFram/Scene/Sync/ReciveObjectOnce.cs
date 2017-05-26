using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ko.NetFram
{
    public class ReciveObjectOnce// : MonoBehaviour
    {
        public static ReciveObjectOnce getInstance()
        {
            return Singleton<ReciveObjectOnce>.getInstance();
        }

        private Hashtable _msgplayer;
        private Hashtable _msgobject;

        // 接收频率
        public int syncinterval = 4;
        private bool _syncctrl = true;

        // 单独的线程调用
        public void SyncClientUpdate()
        {
            while(_syncctrl)
            {
                distributeSyncObjectMsg();
                distributeSyncPlayerMsg();

                Thread.Sleep(syncinterval);
            }
            
        }

        public void StopSyncClient()
        {
            _syncctrl = false;
        }

        // 接收服务器的同步数据
        public void ReciveSyncObject(Hashtable msgObject, Hashtable msgPlayer)
        {
            if (msgObject != null && msgObject.Count > 0)
            {
                _msgobject = msgObject;
            }

            if (msgObject != null && msgPlayer.Count > 0)
            {
                _msgplayer = msgPlayer;
            }
        }

        // 开始分发处理接收到的数据

        private void distributeSyncPlayerMsg()
        {
            if(_msgplayer == null || _msgplayer.Count <= 0)
            {
                return;
            }

            try
            {
                foreach (DictionaryEntry de in _msgplayer)
                {
                    //TODO
                }

                _msgplayer.Clear();
            }catch
            {

            }

        }

        private void distributeSyncObjectMsg()
        {
            if (_msgobject == null || _msgobject.Count <= 0)
            {
                return;
            }

            try
            {
                foreach (DictionaryEntry de in _msgobject)
                {
                    SceneObject go = CollectionObject.getInstance().getSceneObjectByKey((string)de.Key);

                    if (go == null || go.sos == null)
                    {
                        continue;
                    }

                    go.sos.Deserialization((Hashtable)de.Value);
                }

                _msgobject.Clear();
            }
            catch
            {

            }
            
        }
    }
}
