using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class OutUiManager : MonoBehaviour {

    private static GameObject selfgo;
    private static OutUiManager _instance;
    public static OutUiManager getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("Canvas");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<OutUiManager>();
            }
        }

        return _instance;

    }

    public enum UIList
    {
        Login = 0,
        CourseList,
        Prepare,
        Teaching,
        StudentUI,
        DrawingBoardUI,
    }

    // 界面变量
    [SerializeField]
    public OutUIBase[] alloutui;

    // 进入对应教室
    public EnterCourse ec;

    public void ShowUI(UIList id, params Object[] args)
    {
        if (alloutui == null || alloutui.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if(uiid > alloutui.Length)
        {
            return;
        }

        for(int i=0;i<alloutui.Length;i++)
        {
            if(i == uiid)
            {
                alloutui[i].ShowSelf(args);
            }
            else
            {
                alloutui[i].HideSelf();
            }
        }

    }

    public void HideUI(UIList id)
    {
        if (alloutui == null || alloutui.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > alloutui.Length)
        {
            return;
        }

        alloutui[uiid].HideSelf();
    }

    void OnEnable()
    {
        RegisterEvent();
        UiDataManager.getInstance().RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEvent();
        UiDataManager.getInstance().UnRegisterEventListener();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ChooseCourse, this.ChooseCourse);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ChooseCourse, this.ChooseCourse);
    }

    public void ChooseCourse(int courseid)
    {
        // 
        Debug.Log("选择课程 id : " + courseid);

        if(ec != null)
        {
   //         BackFromVrExe.getInstance().InitSaveData(courseid);

            ec.PlayerEnterCourse(courseid);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            ChooseCourse(227);
        }
    }
}
