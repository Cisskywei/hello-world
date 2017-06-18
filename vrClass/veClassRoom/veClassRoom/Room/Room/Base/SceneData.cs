using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 场景数据
    /// </summary>
    class SceneData
    {
        // 场景中需要同步的物体
        public Dictionary<string, ObjectInScene> moveablesceneobject = new Dictionary<string, ObjectInScene>();

        // 场景中的玩家列表
        public Dictionary<int, PlayerInScene> sceneplaylist = new Dictionary<int, PlayerInScene>();

        //场景中的指令
        public List<OrderInScene> sceneorderlist = new List<OrderInScene>();

        private bool _isinit = false;

        public void InitSceneData(Dictionary<string, ObjectInScene> o, Dictionary<int, PlayerInScene> p, List<OrderInScene> c)
        {
            if(_isinit)
            {
                return;
            }

            foreach (KeyValuePair<string,ObjectInScene> os in o)
            {
                moveablesceneobject.Add(os.Key, os.Value);
            }

            foreach (KeyValuePair<int, PlayerInScene> ps in p)
            {
                sceneplaylist.Add(ps.Key, ps.Value);
            }

            foreach(OrderInScene ois in c)
            {
                sceneorderlist.Add(ois);
            }

            _isinit = true;
        }

        public void ClearSceneData()
        {
            moveablesceneobject.Clear();
            sceneplaylist.Clear();
            sceneorderlist.Clear();
            _isinit = false;
        }
    }
}
