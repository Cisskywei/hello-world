using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 处理连续的数据同步  比如画笔
/// </summary>
namespace WisdomClassroom.ClassRoom
{
    class ContinuousPipe
    {
        public ClassRoom room;

        private Dictionary<int,PipeDataOne> pipe = new Dictionary<int, PipeDataOne>();
        public void Receive(int fromid, int toid, Hashtable data)
        {
            if(pipe.ContainsKey(toid))
            {
                pipe[toid].AddPipeData(fromid, data);
            }
            else
            {
                PipeDataOne pdo = new PipeDataOne();

                if (toid == -1)
                {
                    // 同步全部
                    pdo.uuids = room.FindUuidsExcept(fromid);
                }
                else
                {
                    ArrayList u = new ArrayList();
                    string uuid = room.FindUuid(toid);
                    if(uuid != null)
                    {
                        u.Add(uuid);
                        pdo.uuids = u;
                    }
                }

                pdo.AddPipeData(fromid, data);

                pipe.Add(toid,pdo);
            }
            
        }

        public void SyncClient()
        {
            foreach(PipeDataOne pdo in pipe.Values)
            {
                pdo.SendPipeData();
            }
        }

        public class PipeDataOne
        {
            public int toid = -1; // -1 表明多人
            public ArrayList uuids;
            public Hashtable data = new Hashtable();

            public void AddPipeData(int from, Hashtable data)
            {
                string key = from.ToString();
                if(this.data.ContainsKey(key))
                {
                    if(this.data[key] == null)
                    {
                        ArrayList aa = new ArrayList();
                        aa.Add(data);
                        this.data[key] = aa;
                    }
                    else
                    {
                        ((ArrayList)this.data[key]).Add(data);
                    }
                }
                else
                {
                    ArrayList a = new ArrayList();
                    a.Add(data);
                    this.data.Add(key, a);
                }
            }

            public void SendPipeData()
            {
                if(uuids == null)
                {
                    return;
                }

                if(data == null || data.Count <= 0)
                {
                    return;
                }

                // 同步管道数据
                hub.hub.gates.call_group_client(uuids, NetConfig.client_module_name, NetConfig.Pipe_func, (Int64)toid, data);

            }
        }
    }
}
