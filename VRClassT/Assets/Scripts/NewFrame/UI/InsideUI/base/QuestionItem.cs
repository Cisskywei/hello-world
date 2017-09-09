using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour {

    public bool isvoice = false;
    public string content;
    public ComonEnums.QuestionType typ;
    public ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test;
    private int _id = -1;

    // UI控件
    public Text qstem;
    public Text qtyp;

    public void InitQuestion(string content, ComonEnums.QuestionType typ, int id, ComonEnums.InClassTestType catage)
    {
        this.catage = catage;
        InitQuestion(content, typ, id);
    }

    public void InitQuestion(string content, ComonEnums.QuestionType typ, int id)
    {
        this.content = content;
        this.typ = typ;
        this._id = id;

        qtyp.text = QuestionManager.ConvertQuestionType(typ);
        ShowContent();

        // 设置位置 防止位置改变
        Vector3 p = gameObject.GetComponent<RectTransform>().anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;
    }

    public void InitQuestion(QuestionInfor q, ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test)
    {
        this.content = q.stem;
        this.typ = q.typ;
        this._id = q.question_id;

        this.catage = catage;

        if(qtyp != null)
        {
            qtyp.text = QuestionManager.ConvertQuestionType(this.typ);
        }

        ShowContent();

        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    public void ShowContent()
    {
        if(this.content == null || qstem == null)
        {
            return;
        }

        if(this.content.Length > 10)
        {
            qstem.text = this.content.Substring(0,10) + "...";
        }
        else
        {
            qstem.text = this.content;
        }
    }

    public void Onclick()
    {
        if(this.isvoice)
        {
            // 语音
            EventDispatcher.GetInstance().MainEventManager.TriggerEvent<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, ComonEnums.InClassTestType.Ask, this.typ, this._id);
            return;
        }

        if (this._id < 0)
        {
            return;
        }

        // 调用 发送 随堂测试题
        UiDataManager.getInstance().TestInClass(this.catage,this._id);
        // 通知 uimanager 显示 反馈界面
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.catage, this.typ, this._id);
    }
}
