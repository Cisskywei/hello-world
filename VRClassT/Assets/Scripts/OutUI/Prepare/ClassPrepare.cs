using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassPrepare : OutUIBase {

    private static GameObject selfgo;
    private static ClassPrepare _instance;
    public static ClassPrepare getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("prepare");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<ClassPrepare>();
            }
        }

        return _instance;

    }

    public enum UIPrepare
    {
        PersonnelManagement = 0,
        ClassPersonInfor,
        DataPush,
        ClassDataPushInfor,
    }

    //界面系列
    [SerializeField]
    public OutUIBase[] uilist;

	// Use this for initialization
	void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

    public override void ShowSelf(params System.Object[] args)
    {
        ShowUI(UIPrepare.PersonnelManagement);
        base.ShowSelf();
    }

    public void ShowUI(UIPrepare id, params System.Object[] args)
    {
        if (uilist == null || uilist.Length <= 0)
        {
            Debug.LogError("uilist == null || uilist.Length <= 0");
            return;
        }

        int uiid = (int)id;

        if (uiid > uilist.Length)
        {
            return;
        }

        for (int i = 0; i < uilist.Length; i++)
        {
            if (i == uiid)
            {
                uilist[i].ShowSelf(args);
            }
            else
            {
                uilist[i].HideSelf();
            }
        }
    }

    public void HideUI(UIPrepare id)
    {
        if (uilist == null || uilist.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilist.Length)
        {
            return;
        }

        uilist[uiid].HideSelf();
    }

    public void OnClickPerson()
    {
        ShowUI(UIPrepare.PersonnelManagement);
    }

    public void OnClickData()
    {
        ShowUI(UIPrepare.DataPush);
    }

    public void OnClickEnter()
    {
        // 进入教学
        HideSelf();
        OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Teaching);
    }
}
