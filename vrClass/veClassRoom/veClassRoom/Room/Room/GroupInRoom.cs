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
        public Dictionary<int, PlayerInScene> teammembers;

        public ArrayList _viewer;   // 观看者

        public string name;

        public GroupInRoom(string name = "group")
        {
            if (teammembers == null)
            {
                teammembers = new Dictionary<int, PlayerInScene>();
            }

            this.name = name;
        }

        public GroupInRoom(Dictionary<int, PlayerInScene> members)
        {
            if (members == null || members.Count <= 0)
            {
                return;
            }

            teammembers = members;

            if (teammembers == null)
            {
                teammembers = new Dictionary<int, PlayerInScene>();
            }
        }

        // 初始化场景数据
        public void InitSceneData(Dictionary<string, ObjectInScene> objects, Dictionary<int, PlayerInScene> players)
        {
            if (objects == null || objects.Count <= 0 || players == null || players.Count <= 0)
            {
                return;
            }

            this.moveablesceneobject = objects;
            this.sceneplaylistbyid = players;
        }

        public void AddMember(PlayerInScene p)
        {
            if (p == null)
            {
                return;
            }

            try
            {
                if (teammembers.ContainsKey(p.selfid))
                {
                    teammembers[p.selfid] = p;
                }
                else
                {
                    teammembers.Add(p.selfid, p);
                }
            }
            catch
            {

            }
        }

        public bool HasMember(int useid)
        {
            if (teammembers == null || teammembers.Count <= 0)
            {
                return false;
            }

            return teammembers.ContainsKey(useid);
        }

        /// <summary>
        /// 外部注入观看者 需要同步同步数据
        /// </summary>
        public void InjectiveViewer(ArrayList viewer)
        {
            if (viewer == null || viewer.Count <= 0)
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
            }
            catch
            {

            }
        }

        public void InjectiveViewer(string viewer)
        {
            if (viewer == null)
            {
                return;
            }

            try
            {
                if (_uuid_of_player.Contains(viewer))
                {
                    return;
                }

                if (_viewer.Contains(viewer))
                {
                    return;
                }

                _viewer.Add(viewer);
            }
            catch
            {

            }
        }
    }
}
