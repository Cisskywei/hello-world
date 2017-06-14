using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class BaseSceneOrder
    {
        public int name;
        public int commondid;

        public string typ;
        public string commond;
        public int userid;
        public string other;

        // 序列化的信息
        public Hashtable serializedata = new Hashtable();

        public BaseSceneOrder()
        {
            serializedata.Add("commondid", commondid);
            serializedata.Add("userid", userid);
            serializedata.Add("typ", typ);
            serializedata.Add("commond", commond);
            serializedata.Add("other", other);
        }

        public BaseSceneOrder(int commondid, int userid, string typ, string commond, string other)
        {
            this.commondid = commondid;
            this.userid = userid;
            this.typ = typ;
            this.commond = commond;
            this.other = other;

            serializedata.Add("commondid", commondid);
            serializedata.Add("userid", userid);
            serializedata.Add("typ", typ);
            serializedata.Add("commond", commond);
            serializedata.Add("other", other);
        }

        public virtual Hashtable Serialize()
        {
            serializedata["commondid"] = commondid;
            serializedata["userid"] = userid;
            serializedata["typ"] = typ;
            serializedata["commond"] = commond;
            serializedata["other"] = other;

            return serializedata;
        }

        public virtual void Deserialization(Hashtable t)
        {
            if (t == null || t.Count <= 0)
            {
                return;
            }

            commondid = (int)serializedata["commondid"];
            userid = (int)serializedata["userid"];
            typ = (string)serializedata["typ"];
            commond = (string)serializedata["commond"];
            other = (string)serializedata["other"];
        }
    }
}
