using UnityEngine;
using System.Collections;

public class FullScreenWinFixer : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (Screen.fullScreen && (Screen.width != Screen.currentResolution.width || Screen.height != Screen.currentResolution.height ))
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
	}
}
