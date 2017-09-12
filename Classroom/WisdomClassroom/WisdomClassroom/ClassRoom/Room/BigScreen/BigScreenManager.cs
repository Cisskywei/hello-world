using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class BigScreenManager
    {
        public static BigScreenManager getInstance()
        {
            return Singleton<BigScreenManager>.getInstance();
        }

        private Timer t = new Timer();

        private Dictionary<int, BigScreen> _bigscreen = new Dictionary<int, BigScreen>();

        // 回掉函数
        public delegate bool BoolCallBack(ref BigScreen screen);
        public BoolCallBack isputscreen;

        public BigScreen AddBigScreen(string name, string password, string uuid)
        {
            BigScreen b = new BigScreen();
            int id = t.Count();
            b.Init(name,uuid,id);

            if (isputscreen != null)
            {
                if(isputscreen(ref b))
                {
                    isputscreen = null;
                }
            }

            _bigscreen.Add(id, b);

            return b;
        }

        public void RecycleBigScreen(BigScreen screen)
        {
            screen.isused = false;

            if (_bigscreen.ContainsKey(screen.selfid))
            {
                _bigscreen[screen.selfid] = screen;
            }
            else
            {
                _bigscreen.Add(screen.selfid, screen);
            }
        }

        public BigScreen GetBigScreen(int id)
        {
            if(_bigscreen.ContainsKey(id))
            {
                return null;
            }

            return _bigscreen[id];
        }

        public BigScreen GetBigScreen()
        {
            BigScreen s = null;
            foreach(BigScreen b in _bigscreen.Values)
            {
                if(b.isused)
                {
                    continue;
                }

                s = b;
                break;
            }

            return s;
        }

        public class Timer
        {
            int _count = 0;

            public int Count()
            {
                return _count++;
            }
        }
    }
}
