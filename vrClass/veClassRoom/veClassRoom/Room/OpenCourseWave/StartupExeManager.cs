using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class StartupExeManager
    {
        public static StartupExeManager getInstance()
        {
            return Singleton<StartupExeManager>.getInstance();
        }

        public List<int> personnel_cache = new List<int>();

        public void AddPerson(int userid)
        {
            if(!personnel_cache.Contains(userid))
            {
                personnel_cache.Add(userid);
            }
        }

        public void RemovePerson(int userid)
        {
            if (personnel_cache.Contains(userid))
            {
                personnel_cache.Remove(userid);
            }
        }

        public bool CheckPerson(int userid)
        {
            if(personnel_cache == null || personnel_cache.Count <= 0)
            {
                return false;
            }

            return personnel_cache.Contains(userid);
        }

        public void ClearAll()
        {
            personnel_cache.Clear();
        }

    }
}
