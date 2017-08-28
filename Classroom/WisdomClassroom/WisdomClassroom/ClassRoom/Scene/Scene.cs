using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class Scene
    {
        public int selfid = -1;

        //场景中的物体
        public Dictionary<int, PlayerInScene> sceneplayer = new Dictionary<int, PlayerInScene>();
        public Dictionary<int, ObjectInScene> sceneobject = new Dictionary<int, ObjectInScene>();
        public Dictionary<int, CommandInScene> scenecommand = new Dictionary<int, CommandInScene>();
    }
}
