using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class BigScreen
    {
        public int selfid = -1;
        public string uuid;
        public string name;
        public string token;
        public int classid = -1;

        public bool isused = false;

        public void Init(string name, string uuid, int id)
        {
            this.name = name;
            this.uuid = uuid;
            this.selfid = id;
        }
    }
}
