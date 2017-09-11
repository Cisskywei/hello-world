using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class SwitchModeUI : OutUIBase {

    //public Button btn2;
    //public Button btn3;
    //public Button btn4;
    //public Button btn5;
    //public Button btn8;
    //public Button btn9;
    //public Button btn10;
    //public Button btn16;
    //public Button btn17;
    //public Button btn18;
    //public Button btn19;

    public void SwitchMode(int m)
    {
        ComonEnums.TeachingMode mode = (ComonEnums.TeachingMode)m;
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<ComonEnums.TeachingMode>(EventId.SwitchMode, mode);
    }
}
