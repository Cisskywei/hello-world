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
        public ArrayList _viewer;   // 观看者

        public string name;
        public int id;

        public GroupInRoom(string name = "group")
        {
            
            if (sceneplaylistbyid == null)
            {
                sceneplaylistbyid = new Dictionary<int, PlayerInScene>();
            }

            this.name = name;
        }

        public GroupInRoom(Dictionary<int, PlayerInScene> members)
        {
            if (members == null || members.Count <= 0)
            {
                return;
            }

            sceneplaylistbyid = members;

            if (sceneplaylistbyid == null)
            {
                sceneplaylistbyid = new Dictionary<int, PlayerInScene>();
            }
        }

        // 初始化场景数据
        public void InitSceneData(Dictionary<string, ObjectInScene> objects, Dictionary<int, PlayerInScene> players = null)
        {
            if (objects == null || objects.Count <= 0)
            {
                return;
            }

            this.moveablesceneobject = objects;

            if ( !(players == null || players.Count <= 0))
            {
                this.sceneplaylistbyid = players;
            }
        }

        public void AddMember(PlayerInScene p)
        {
            if (p == null)
            {
                return;
            }

            try
            {
                if (sceneplaylistbyid.ContainsKey(p.selfid))
                {
                    sceneplaylistbyid[p.selfid] = p;
                }
                else
                {
                    sceneplaylistbyid.Add(p.selfid, p);
                }
            }
            catch
            {

            }
        }

        public bool HasMember(int useid)
        {
            if (sceneplaylistbyid == null || sceneplaylistbyid.Count <= 0)
            {
                return false;
            }

            return sceneplaylistbyid.ContainsKey(useid);
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
