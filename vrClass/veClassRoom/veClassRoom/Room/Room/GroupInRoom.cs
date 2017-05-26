using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class GroupInRoom : BaseRoomClass
    {
        public Dictionary<string, PlayerInScene> teammembers;

        public ArrayList _viewer;   // 观看者

        public GroupInRoom()
        {
            if(teammembers == null)
            {
                teammembers = new Dictionary<string, PlayerInScene>();
            }
        }

        public GroupInRoom(Dictionary<string,PlayerInScene> members)
        {
            if(members == null || members.Count <= 0)
            {
                return;
            }

            teammembers = members;

            if (teammembers == null)
            {
                teammembers = new Dictionary<string, PlayerInScene>();
            }
        }

        public void AddMember(PlayerInScene p)
        {
            if(p == null)
            {
                return;
            }

            try
            {
                if(teammembers.ContainsKey(p.token))
                {
                    teammembers[p.token] = p;
                }else
                {
                    teammembers.Add(p.token, p);
                }
            }catch
            {

            }
        }

        /// <summary>
        /// 外部注入观看者 需要同步同步数据
        /// </summary>
        public void InjectiveViewer(ArrayList viewer)
        {
            if(viewer == null || viewer.Count <= 0)
            {
                return;
            }

            try
            {
                foreach (string uuid in viewer)
                {
                    if (_uuid_of_player.Contains(uuid))
                    {
                        continue;
                    }

                    _viewer.Add(uuid);
                }
            }catch
            {

            }
        }

        // 覆盖父类方法
        public override void SyncClient()
        {
            Hashtable msgObject = new Hashtable();
            Hashtable msgPlayer = new Hashtable();
            while (_syncstate)
            {
                // 同步数据
                foreach (ObjectInScene so in moveablesceneobject.Values)
                {
                    if (!so.changeorno)
                    {
                        continue;
                    }

                    // 同步客户端
                    msgObject.Add(so.name, so.Serialize());
                }

                foreach (PlayerInScene sp in sceneplaylist.Values)
                {
                    if (!sp.changeorno)
                    {
                        continue;
                    }

                    // 同步客户端
                    msgPlayer.Add(sp.name, sp.Serialize());
                }

                // 同步指令
                //TODO

                // 同步客户端
                if (msgPlayer.Count > 0 || msgObject.Count > 0)
                {
                    try
                    {

                        if (_uuid_sync_cache.Count > 0)
                        {
                            _uuid_sync_cache.Clear();
                        }

                        foreach (PlayerInScene p in sceneplaylist.Values)
                        {
                            if (p.isCanReceive)
                            {
                                _uuid_sync_cache.Add(p.uuid);
                            }
                        }

                        // 加入观看者
                        if(_viewer!=null&&_viewer.Count>0)
                        {
                            foreach(string uuid in _viewer)
                            {
                                _uuid_sync_cache.Add(uuid);
                            }
                        }

                    }
                    catch
                    {

                    }

                    if (_uuid_sync_cache.Count > 0)
                    {
                        hub.hub.gates.call_group_client(_uuid_sync_cache, "cMsgConnect", "SyncClient", msgObject, msgPlayer);
                        _uuid_sync_cache.Clear();
                    }
                }

                // 同步之后清除
                msgObject.Clear();
                msgPlayer.Clear();

                Thread.Sleep(16);
            }
        }
    }
}
