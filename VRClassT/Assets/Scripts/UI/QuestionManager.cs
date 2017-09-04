using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QuestionManager {

    public static QuestionManager getInstance()
    {
        return Singleton<QuestionManager>.getInstance();
    }

    private Dictionary<int, QuestionInfor> questionlist = new Dictionary<int, QuestionInfor>();

    // 题目信息 只为测试

    public void InitQuestionList(string jsondata)
    {
        if(jsondata == null)
        {
            return;
        }

        jsondata = JsonDataHelp.getInstance().DecodeBase64(null, jsondata);
        DataType.QuestionInforRetData question = JsonDataHelp.getInstance().JsonDeserialize<DataType.QuestionInforRetData>(jsondata);

        if(question != null && question.data != null)
        {
            int id;
            for(int i=0;i<question.data.Length;i++)
            {
                QuestionInfor q = new QuestionInfor(question.data[i]);
                if(q != null)
                {
                    questionlist.Add(q.question_id, q);
                }
            }
        }
    }

    // 获取题目信息
    public Dictionary<int, QuestionInfor> GetQuestionList()
    {
        return questionlist;
    }

    public QuestionInfor GetQuestionById(int id)
    {
        QuestionInfor question = null;

        if(questionlist.ContainsKey(id))
        {
            question = questionlist[id];
        }

        return question;
    }

    public List<QuestionInfor> GetQuestionByTyp(Enums.QuestionType typ)
    {
        List<QuestionInfor> q = new List<QuestionInfor>();

        foreach(QuestionInfor di in questionlist.Values)
        {
            if (di.typ == typ)
            {
                q.Add(di);
            }
        }

        return q;
    }

    public static string ConvertQuestionType(Enums.QuestionType typ)
    {
        string ret = null;

        if(typ == Enums.QuestionType.None)
        {
            return ret;
        }

        switch(typ)
        {
            case Enums.QuestionType.TrueOrFalse:
                ret = "判断题";
                break;
            case Enums.QuestionType.SingleChoice:
                ret = "单选题";
                break;
            case Enums.QuestionType.MultipleChoice:
                ret = "多选题";
                break;
            case Enums.QuestionType.ShortAnswer:
                ret = "简答题";
                break;
            default:
                break;
        }

        return ret;
    }

    public static Enums.QuestionType ConvertQuestionTypeToEnum(string typ)
    {
        Enums.QuestionType ret = Enums.QuestionType.None;

        if (typ == null)
        {
            return ret;
        }

        switch (typ)
        {
            case "select":
                ret = Enums.QuestionType.SingleChoice;
                break;
            case "multiselect":
                ret = Enums.QuestionType.MultipleChoice;
                break;
            case "judge":
                ret = Enums.QuestionType.TrueOrFalse;
                break;
            case "text":
                ret = Enums.QuestionType.ShortAnswer;
                break;
            default:
                break;
        }

        return ret;
    }

    public static int GetHanNumFromString(string str)
    {
        int count = 0;
        Regex regex = new Regex(@"^[u4E00-u9FA5]{0,}$");
        for (int i = 0; i < str.Length; i++)
        {
            if (regex.IsMatch(str[i].ToString()))
            {
                count++;
            }
        }
        return count;
    }
}
