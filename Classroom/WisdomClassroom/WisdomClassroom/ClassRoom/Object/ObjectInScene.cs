using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class ObjectInScene
    {
        public int selfid = -1;
        public int roomid = -1;
        public int teamid = -1;

        // 三维场景数据
        public Hashtable infor3d;
        public bool _ischange = false;

        //物体锁
        public int locker = -1;

        //三维场景数据解析
        public void Change3DInfor(Hashtable infor)
        {
            infor3d = infor;

            _ischange = true;
        }

        public Hashtable Get3DInfor()
        {
            if (!_ischange)
            {
                return null;
            }

            _ischange = false;

            return infor3d;
        }
    }
}
