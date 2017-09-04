using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoardUI : uibase {

    public DrawingBoardUI drawingboard;

	// Use this for initialization
	void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    // 初始化界面
    public void ShowSelf()
    {
        // 显示白板
        //TODO

        if (drawingboard != null)
        {
            drawingboard.ShowSelf();
        }

        if(UserInfor.getInstance().isTeacher)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            // 测试
            MsgModule.getInstance().reqWhiteBoard(1);
        }

    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        if(drawingboard != null)
        {
            drawingboard.HideSelf();
        }

        if (UserInfor.getInstance().isTeacher)
        {
            // 测试
            MsgModule.getInstance().reqWhiteBoard(0);
        }
    }
}
