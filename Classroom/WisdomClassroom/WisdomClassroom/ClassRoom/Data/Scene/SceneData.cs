using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class SceneData
    {
        public Dictionary<int, PlayerInScene> allstudents = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> allobjects = new Dictionary<int, ObjectInScene>();

        public void Init(Dictionary<int, ObjectInScene> o, Dictionary<int, PlayerInScene> p)
        {
            foreach (KeyValuePair<int, ObjectInScene> os in o)
            {
                allobjects.Add(os.Key, os.Value);
            }

            foreach (KeyValuePair<int, PlayerInScene> ps in p)
            {
                allstudents.Add(ps.Key, ps.Value);
            }
        }

        public void ClearSceneData()
        {
            allstudents.Clear();
            allobjects.Clear();
        }
    }
}
