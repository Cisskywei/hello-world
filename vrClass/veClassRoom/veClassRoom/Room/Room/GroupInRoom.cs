using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class GroupInRoom : BaseRoomClass
    {
        public Dictionary<string, PlayerInScene> teammembers;

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
    }
}
