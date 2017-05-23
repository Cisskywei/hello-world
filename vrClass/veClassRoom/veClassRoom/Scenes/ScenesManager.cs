using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace veClassRoom
{
    class ScenesManager
    {
        private Dictionary<string, Scene> allscenes = new Dictionary<string, Scene>();

        private Dictionary<string, Thread> scenesthread = new Dictionary<string, Thread>();

        ~ScenesManager()
        {
            allscenes.Clear();
            scenesthread.Clear();
        }

        public bool CreateSceneByName(string name)
        {
            bool ret = true;

            do
            {
                if (allscenes.ContainsKey(name))
                {
                    ret = false;
                    break;
                }

                Scene s = new Scene();
                s.CreateScene(name);
                allscenes.Add(name, s);

                Thread t = new Thread(s.SyncClient);
                t.Start();
                scenesthread.Add(name, t);

                ret = true;

            } while (false);
            
            return ret;
        }

        public Scene FindSceneByName(string name)
        {
            Scene s = null;

            if(allscenes.ContainsKey(name))
            {
                s = allscenes[name];
            }

            return s;
        }
    }
}
