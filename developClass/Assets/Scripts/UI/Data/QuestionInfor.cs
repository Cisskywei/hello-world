using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionInfor {

    public class OptionsABCD
    {
        public int belongid;  // 所属题目id
        public string[] contents; // 选项内容  A B C D

        public void initOptions(string[] c)
        {
            this.contents = c;
        }

        public void initOptions(string a, string b, string c, string d = null)
        {
            if(this.contents == null)
            {
                this.contents = new string[4];
            }

            this.contents[0] = a;
            this.contents[1] = b;
            this.contents[2] = c;
            this.contents[3] = d;
        }
    }

    public string content;
    public Enums.QuestionType typ;
    public Enums.InClassTestType category;
    public int _id = -1;

    public int answerid;
}
