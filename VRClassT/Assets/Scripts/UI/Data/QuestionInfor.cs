using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionInfor {

    public class OptionsABCD
    {
        public int belongid;  // 所属题目id
        public string[] contents; // 选项内容  A B C D

        public OptionsABCD()
        { }

        public OptionsABCD(int id)
        {
            this.belongid = id;
        }

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

    public QuestionInfor()
    {

    }

    /*
    public string question_id { get; set; }
    public string type { get; set; }
    public string stem { get; set; }
    public string options { get; set; }
    public string answer { get; set; }
    public string explanation { get; set; }
    */
    public QuestionInfor(DataType.QuestionInfor infor)
    {
        this.question_id = Convert.ToInt32(infor.question_id);
        this.typ = QuestionManager.ConvertQuestionTypeToEnum(infor.type);
        this.stem = infor.stem;
        this.options = ParsingOptions(infor.options);
        FindAnswer(infor.answer);
        this.explanation = infor.explanation;
        this.difficulty = infor.difficulty;
    }

    private string[] ParsingOptions(string option, char splitchar = '|')
    {
        if(option == null)
        {
            return null;
        }

        string[] op = option.Split(splitchar);

        return op;
    }

    public ArrayList CompareAnswer(string[] options, string[] anwsers)
    {
        if(options == null || options.Length <= 0 || anwsers == null || anwsers.Length <= 0)
        {
            return null;
        }

        ArrayList anwserid = new ArrayList();

        for(int i=0;i<anwers.Length;i++)
        {
            for(int j=0;j<options.Length;j++)
            {
                if(anwers[i] == options[j])
                {
                    // j 是选项id
                    anwserid.Add(j);
                }
            }
        }

        return anwserid;
    }

    private void FindAnswer(string anwser, char splitchar = '|')
    {
        this.anwers = anwser.Split(splitchar);
        this.correctanswers = CompareAnswer(this.options, this.anwers);
    }

    public int question_id = -1;
    public ComonEnums.QuestionType typ;
    public string stem;
    public string[] options;
    public string[] anwers;
    public string explanation;
    public string difficulty;
    public ArrayList correctanswers;

    public ComonEnums.InClassTestType category;
}
