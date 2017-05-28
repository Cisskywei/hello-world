using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QuestionManager {

    public static QuestionManager getInstance()
    {
        return Singleton<QuestionManager>.getInstance();
    }

    // 获取题目信息
    public List<QuestionInfor> GetQuestionList()
    {
        List<QuestionInfor> questions = new List<QuestionInfor>();

        return questions;
    }

    public List<QuestionInfor> GetQuestionList(Enums.InClassTestType typ)
    {
        List<QuestionInfor> questions = new List<QuestionInfor>();

        switch (typ)
        {
            case Enums.InClassTestType.Test:
                break;
            case Enums.InClassTestType.Ask:
                break;
            case Enums.InClassTestType.Fast:
                break;
            default:
                break;
        }

        return questions;
    }

    public QuestionInfor GetQuestionByTypId(Enums.InClassTestType typ, int id)
    {
        QuestionInfor question = new QuestionInfor();

        return question;
    }

    public QuestionInfor GetQuestionById(int id)
    {
        QuestionInfor question = new QuestionInfor();

        return question;
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
