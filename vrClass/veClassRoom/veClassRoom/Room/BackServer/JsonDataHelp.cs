using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace veClassRoom.Room
{
    class JsonDataHelp
    {
        public static JsonDataHelp getInstance()
        {
            return Singleton<JsonDataHelp>.getInstance();
        }

        private JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public T JsonDeserialize<T>(string jsondata)
        {
            T v = _serializer.Deserialize<T>(jsondata);

            return v;
        }

        public string JsonSerialize<T>(T jsonobjectdata)
        {
            string v = _serializer.Serialize(jsonobjectdata);

            return v;
        }
    }
}
