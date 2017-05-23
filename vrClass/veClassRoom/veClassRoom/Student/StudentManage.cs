using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom
{
    class StudentManage
    {
        public Dictionary<string, Student> _allstudent_token = new Dictionary<string, Student>();

        public void student_login(string token, string name, string uuid, string modelname, string callbackname)
        {
            Console.WriteLine("学生登陆 : " + name);

            if (!_allstudent_token.ContainsKey(token))
            {
                Student s = new Student(token, name, uuid);
                _allstudent_token.Add(token, s);
            }
            else
            {
                ((Student)_allstudent_token[token])._uuid = uuid;
            }

            //
            Hashtable h = new Hashtable();
            h.Add("result", "success");
            h.Add("token", token);
            h.Add("name", name);
            h.Add("uuid", uuid);

            hub.hub.gates.call_client(uuid, modelname, callbackname, h);
        }

        public void student_enter_room(string token, string name, string uuid, string teachername)
        {
            // 只为测试固定老师名字
            teachername = "onlyyou";
            Teacher t = find_teacher_toke(teachername);

            if (t == null)
            {
                // 暂时老师不存在  
                Console.WriteLine("暂时该老师不存在 : " + teachername);
                Teacher tt = find_teacher_defualt();

                if (tt == null)
                {
                    Console.WriteLine("选取默认老师失败");
                }
                else
                {
                    Console.WriteLine("选取默认老师成功");
                    Hashtable msg = tt.student_enter_room(_allstudent_token[token]);
                    hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);
                }
            }
            else
            {
                Hashtable msg = t.student_enter_room(_allstudent_token[token]);
                hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);
            }
        }

        private Teacher find_teacher_toke(string teachertoken)
        {
            return server.teachers.get_teacher_token(teachertoken);
        }

        private Teacher find_teacher_defualt()
        {
            return server.teachers.get_teacher_defualt();
        }

        public void clearAllStudent()
        {
            if(_allstudent_token.Count > 0)
            {
                _allstudent_token.Clear();
            }
        }

        public void student_leave_class(string token, string name, string uuid)
        {
            if(_allstudent_token.ContainsKey(token))
            {
                _allstudent_token.Remove(token);
            }
        }
    }
}
