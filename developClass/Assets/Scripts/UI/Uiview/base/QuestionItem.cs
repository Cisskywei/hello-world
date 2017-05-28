using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour {

    public string content;
    public Enums.QuestionType typ;
    private int _id = -1;

    // UI控件
    public Text qstem;
    public Text qtyp;

    // Use this for initialization
    void Start () {
    }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void InitQuestion(string content, Enums.QuestionType typ, int id)
    {
        this.content = content;
        this.typ = typ;
        this._id = id;

        qtyp.text = QuestionManager.ConvertQuestionType(typ);
        ShowContent();
    }

    public void InitQuestion(QuestionInfor q)
    {
        this.content = q.content;
        this.typ = q.typ;
        this._id = q._id;

        qtyp.text = QuestionManager.ConvertQuestionType(this.typ);
        ShowContent();
    }

    public void ShowContent()
    {
        if(this.content == null)
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
        if(this._id < 0)
        {
            return;
        }

        // 调用 发送 随堂测试题
        UiDataManager.getInstance().TestInClass(this._id);
        // 通知 uimanager 显示 反馈界面
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Enums.InClassTestType, int>(EventId.ChooseQuestion, Enums.InClassTestType.Test, this._id);
    }
}
