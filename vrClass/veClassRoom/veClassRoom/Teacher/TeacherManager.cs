using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom
{
    class TeacherManager
    {
        private Dictionary<string, Teacher> _allTeachers_token = new Dictionary<string, Teacher>();
        private Dictionary<string, Teacher> _allTeachers_uuid = new Dictionary<string, Teacher>();

        public void teacher_login(string token, string name, string uuid, string modelname, string callbackname)
        {
            if (has_teacher(token))
            {

                Console.WriteLine("已经拥有该老师 ： " + name);

                string old_uuid = relogin(token, uuid);

                Teacher t = get_teacher_token(token);

                Hashtable msg = new Hashtable();
                msg.Add("connector", t.get_model_name());
                msg.Add("connectorfunc", t.get_modelfunc_name());
                msg.Add("result", "success");
                msg.Add("token", token);
                msg.Add("name", name);
                msg.Add("uuid", uuid);

                hub.hub.gates.call_client(uuid, modelname, callbackname, msg);
                hub.hub.gates.call_client(old_uuid, modelname, callbackname, msg);

            }
            else
            {
                Console.WriteLine("新创建该老师 ： " + name);

                Hashtable _query = new Hashtable();
                _query.Add("token", token);
                _query.Add("name", name);
                _query.Add("modelname", modelname);
                _query.Add("callbackname", callbackname);
                query_player_info(uuid, token, name, modelname, callbackname);
                //              hub.hub.dbproxy.getObjectInfo(_query, (ArrayList date_list) => { query_player_info(uuid, token, name, modelname, callbackname, date_list); });
            }
        }

        private bool has_teacher(string token)
        {
            if (_allTeachers_token.ContainsKey(token))
            {
                return true;
            }

            return false;
        }

        public void clearAllTeacher()
        {
            if(_allTeachers_token.Count > 0)
            {
                _allTeachers_token.Clear();
            }

            if (_allTeachers_uuid.Count > 0)
            {
                _allTeachers_uuid.Clear();
            }
        }

        public Teacher reg_teacher(string uuid, Hashtable _data)
        {
            if (!_allTeachers_token.ContainsKey((string)_data["token"]) && !_allTeachers_uuid.ContainsKey(uuid))
            {
                Teacher t = new Teacher(uuid, _data);

                _allTeachers_token.Add((string)_data["token"], t);
                _allTeachers_uuid.Add(uuid, t);

                return t;
            }

            Console.WriteLine("error teacher register info");

            return null;
        }

        public string relogin(string token, string client_uuid)
        {
            Teacher t = get_teacher_token(token);

            if (t != null)
            {
                string old_uuid = t.relogin(client_uuid);

                if (_allTeachers_uuid.ContainsKey(old_uuid))
                {
                    _allTeachers_uuid.Remove(old_uuid);
                }
                else
                {
                    Console.WriteLine("relogin:error teacher register info");
                }

                _allTeachers_uuid.Add(client_uuid, t);

                return old_uuid;
            }

            return "";
        }

        public Teacher get_teacher_token(string token)
        {
            if (_allTeachers_token.ContainsKey(token))
            {
                return _allTeachers_token[token];
            }

            return null;
        }

        public Teacher get_teacher_uuid(string uuid)
        {
            if (_allTeachers_uuid.ContainsKey(uuid))
            {
                return _allTeachers_uuid[uuid];
            }

            return null;
        }

        public Teacher get_teacher_defualt()
        {
            Teacher t = null;
            foreach(Teacher tt in _allTeachers_token.Values)
            {
                t = tt;
                break;
            }
            return t;
        }

        void query_player_info(string uuid, string token, string name, string modelname, string callbackname, ArrayList date_list = null)
        {
            Console.WriteLine("db rsp " + uuid + token + name);

            if (date_list != null && date_list.Count > 1)
            {
                Console.WriteLine("error: repeate token");
            }

            if (date_list == null || date_list.Count == 0)
            {
                Hashtable _data = new Hashtable();
                _data.Add("token", token);
                _data.Add("name", name);
                _data.Add("modelname", modelname);
                _data.Add("callbackname", callbackname);
                create_teacher(uuid, _data);
                //            hub.hub.dbproxy.createPersistedObject(_data, () => { create_teacher(uuid, _data); });
            }
            else
            {
                Hashtable _data = new Hashtable();
                _data.Add("token", token);
                _data.Add("name", name);
                _data.Add("modelname", modelname);
                _data.Add("callbackname", callbackname);
                reg_client_info(uuid, _data);
                //reg_client_info(uuid, (Hashtable)date_list[0]);
            }

        }

        void create_teacher(string uuid, Hashtable _data)
        {
            reg_client_info(uuid, _data);
        }

        void reg_client_info(string uuid, Hashtable _data)
        {
            Teacher t = reg_teacher(uuid, _data);

            Hashtable msg = new Hashtable();
            msg.Add("connector", t.get_model_name());
            msg.Add("connectorfunc", t.get_modelfunc_name());
            msg.Add("result", "success");
            msg.Add("token", _data["token"]);
            msg.Add("name", _data["name"]);
            msg.Add("uuid", uuid);

            string modelname = (string)_data["modelname"];
            string callbackname = (string)_data["callbackname"];

            if (modelname == null || callbackname == null)
            {
                hub.hub.gates.call_client(uuid, "login", "login_sucess", msg);
            }
            else
            {
                hub.hub.gates.call_client(uuid, modelname, callbackname, msg);
            }

            Console.WriteLine("rsp client " + uuid);
        }
    }
}
