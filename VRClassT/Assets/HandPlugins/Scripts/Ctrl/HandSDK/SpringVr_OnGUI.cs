using UnityEngine;
using System.Collections;

public class SpringVr_OnGUI : SpringVr_Base {

    private void OnGUI()
    {
        onGUI();
    }
    protected virtual void onGUI() { }
}
