using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom
{
    class Student
    {
        public Teacher _teacher;
        public Group _group;
        public ClassRoom _classroom;

        public string _token;
        public string _name;
        public string _uuid;

        public Student(string token, string name, string uuid)
        {
            _token = token;
            _name = name;
            _uuid = uuid;
        }
    }
}
