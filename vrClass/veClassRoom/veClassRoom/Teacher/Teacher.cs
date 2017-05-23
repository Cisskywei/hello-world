using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom
{
    class Teacher : imodule
    {
        public Teacher(string uuid, Hashtable _data)
        {
            this.uuid = uuid;
            this.infor = _data;

            modelname = (string)_data["name"];
            _connect_client = (string)_data["modelname"];
            _connect_client_func = (string)_data["callbackname"];

            // 添加model
            server.add_Hub_Model(modelname, this);
        }

        public string relogin(string client_uuid)
        {
            string tmp = uuid;
            uuid = client_uuid;

            return tmp;
        }

        public string get_model_name()
        {
            return modelname;
        }

        public string get_modelfunc_name()
        {
            return "connector_func";
        }

        public void connector_func(Hashtable msg)
        {

        }

        // 学生进入个房间
        public Hashtable student_enter_room(Student s)
        {
            if (s == null)
            {
                return null;
            }

            if (has_student(s))
            {
                Student dels = _student_token[s._token];
                if (dels != null)
                {
                    if (_uuid_of_student.Contains(dels._uuid))
                    {
                        _uuid_of_student.Remove(dels._uuid);
                    }

                }
                _student_token[s._token] = s;
                _student_uuid[s._uuid] = s;
                _uuid_of_student.Add(s._uuid);
            }
            else
            {
                _student_token.Add(s._token, s);
                _student_uuid.Add(s._uuid, s);
                _uuid_of_student.Add(s._uuid);
            }

            Console.WriteLine("学生进入教室" + s._uuid + "当前学生数" + _uuid_of_student.Count);

            Hashtable msg = new Hashtable();
            msg.Add("connector", modelname);
            msg.Add("teachername", modelname);

            return msg;
        }

        private bool has_student(Student s)
        {
            if (s == null)
            {
                return false;
            }

            if (s._token != null)
            {
                return _student_token.ContainsKey(s._token);
            }
            else if (s._uuid != null)
            {
                return _student_uuid.ContainsKey(s._uuid);
            }

            return false;
        }

        private bool has_student_token(string token)
        {
            return _student_token.ContainsKey(token);
        }

        private bool has_student_uuid(string uuid)
        {
            return _student_uuid.ContainsKey(uuid);
        }

        public void teacher_enter_class(string token, string name, string uuid, string teachername)
        {
            Hashtable msg = new Hashtable();
            msg.Add("result", "success");

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);
        }

        public void teacher_begin_class(string token, string name, string uuid)
        {
            Hashtable msg = new Hashtable();
            msg.Add("result", "success");

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);
        }

        public void teacher_leave_class(string token, string name, string uuid, string teachername)
        {
            if(_uuid_of_student.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "classclose");

                _uuid_of_student.Clear();
                _student_token.Clear();
                _student_uuid.Clear();
            }
        }

        public void student_leave_class(string token, string name, string uuid)
        {
            if (_uuid_of_student.Contains(uuid))
            {
                _uuid_of_student.Remove(uuid);
            }

            if(_student_token.ContainsKey(token))
            {
                _student_token.Remove(token);
            }

            if (_student_uuid.ContainsKey(uuid))
            {
                _student_uuid.Remove(uuid);
            }
        }

        // 教师选择跳转返回
        public void teacher_choose_scene(string uuid, string scenename)
        {
            if (uuid != this.uuid)
            {
                return;
            }

            if (_uuid_of_student.Count > 0)
            {
                hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "teacherchosescene", scenename);
            }

            hub.hub.gates.call_client(uuid, "cMsgConnect", "teacherchosescene", scenename);
        }

        // 课堂测验相关
        public void teacher_class_test(string token, string name, string uuid, string questionid)
        {
            Console.WriteLine("全体测试 老师名字 ： " + name);
            // 全体测试
            Hashtable msg = new Hashtable();
            msg.Add("result", "success");

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);

            msg.Add("question", questionid);
            msg.Add("questiontype", "classtest");
            boradcast_msg(msg);
        }

        public void teacher_respond_test(string token, string name, string uuid, string question)
        {
            ischosestudent = false;

            Console.WriteLine("抢答测试 老师名字 ： " + name);
            // 全体测试
            Hashtable msg = new Hashtable();
            msg.Add("result", "success");

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);

            msg.Add("question", question);
            msg.Add("questiontype", "respondtest");
            boradcast_msg(msg);
        }

        public void teacher_chooseone_test(string token, string name, string uuid, string question)
        {
            Console.WriteLine("指派测试 老师名字 ： " + name);
            // 全体测试
            Hashtable msg = new Hashtable();
            msg.Add("result", "success");

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);

            msg.Add("question", question);
            msg.Add("questiontype", "choosetest");
            boradcast_msg(msg);
        }

        // 打开文件相关
        public void teacher_open_apk(string token, string name, string uuid, string apkname)
        {
            Console.WriteLine("老师打开 apk 名字 ： " + apkname);

            Hashtable msg = new Hashtable();
            msg.Add("result", "openapk");
            msg.Add("apkname", apkname);
            boradcast_msg(msg);

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);
        }

        // 广播消息
        public void boradcast_msg(Hashtable msg)
        {
            hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "ret_msg", msg);
        }

        // 接收学生选择结果
        public void student_class_test(string token, string name, string uuid, Int64 abc)
        {
            Hashtable msg = new Hashtable();
            msg.Add("result", "studentoption");
            msg.Add("name", name);
            msg.Add("option", abc.ToString());
            hub.hub.gates.call_client(this.uuid, "cMsgConnect", "ret_msg", msg);
        }

        // 接收学生抢答结果
        private bool ischosestudent = false;
        public void student_respond_test(string token, string name, string uuid)
        {
            if (ischosestudent)
            {
                return;
            }

            ischosestudent = true;

            Hashtable msg = new Hashtable();
            msg.Add("result", "studentoption");
            msg.Add("name", name);
            hub.hub.gates.call_client(this.uuid, "cMsgConnect", "ret_msg", msg);
        }


        // 数据转发 同步
        public void ret_sync_msg(Double x, Double y, Double z, Int64 prs, Int64 selfid, string uuid)
        {
            if (uuid != this.uuid)
            {
                Console.WriteLine("uuid 不是老师的");
                return;
            }

            //string connector = "cMsgConnect";
            //string callbackfunc = "ret_sync_vector3";
            //foreach (KeyValuePair<string, Student> student in _student_uuid)
            //{
            //    if (student.Key != uuid)
            //    {
            //        //            connector = student.Value.Connector;
            //        if (connector == null)
            //        {
            //            connector = "cMsgConnect";
            //        }

            //        if (callbackfunc == null)
            //        {
            //            callbackfunc = "ret_sync_vector3";
            //        }
            //        hub.hub.gates.call_client(student.Key, connector, callbackfunc, x, y, z, prs, selfid);
            //    }
            //}

            //Console.WriteLine(_uuid_of_student.Count);

            hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "ret_sync_vector3", x, y, z, prs, selfid);
        }

        public void ret_sync_vector4(Double x, Double y, Double z, Double w, Int64 selfid, string uuid)
        {
            if (uuid != this.uuid)
            {
                Console.WriteLine("uuid 不是老师的");
                return;
            }

            hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "ret_sync_vector4", x, y, z, w, selfid);
        }

        public void ret_sync_vector6(Double x, Double y, Double z, Double sx, Double sy, Double sz, Double sw, Int64 selfid, string uuid)
        {
            if (uuid == null)
            {
                return;
            }
            if (uuid != this.uuid)
            {
                Console.WriteLine("uuid 不是老师的");
                return;
            }

            hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "ret_sync_vector6", x, y, z, sx, sy, sz, sw, selfid);
        }


        // 同步指令
        public void ret_sync_commond(string typ, string commond, string name, Int64 selfid, string uuid)
        {
            if (uuid != this.uuid)
            {
                Console.WriteLine("uuid 不是老师的");
                return;
            }

            //Console.WriteLine((string)_uuid_of_student[0] + " ret_sync_commond" + "typ " + typ + "commond   3333333 XXXX --- " + commond);

            hub.hub.gates.call_group_client(_uuid_of_student, "cMsgConnect", "ret_sync_commond", typ, commond, name, selfid);

            //hub.hub.gates.call_client((string)_uuid_of_student[0],)
        }


        // 变量区   ////////////////////////////////////////////////////////////////////////////////////////
        public string modelname;

        private string uuid;
        private Hashtable infor;

        // 和客户端的连接者
        public string _connect_client;
        public string _connect_client_func;

        // 房间和组
        public ClassRoom _classroom;
        public Group _group;

        // 所有学生
        public Dictionary<string, Student> _student_token = new Dictionary<string, Student>();
        public Dictionary<string, Student> _student_uuid = new Dictionary<string, Student>();
        public ArrayList _uuid_of_student = new ArrayList();

    }
}
