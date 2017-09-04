using UnityEngine;
using System.Collections.Generic;

namespace PaintCraft.Demo.ColoringBook{ 
    public class SplitScreenSwitcher : MonoBehaviour {
        public List<GameObject> SingleViewObjects;
        public List<GameObject> SplitScreenObjects;

        public bool ShowSplitScreenByDefault = false;
        	
    	void Start () {
            ShowSpleetScreen(ShowSplitScreenByDefault);
    	}	

        public void ShowSpleetScreen(bool showSplitScreen){     
            AnalyticsWrapper.CustomEvent("ShowSplitscreen", new Dictionary<string, object>
            {
                { "SplitScreenActive", showSplitScreen}
            });

            SplitScreenObjects.ForEach((obj) => obj.SetActive(showSplitScreen));
            SingleViewObjects.ForEach((obj) => obj.SetActive(!showSplitScreen));
        }
    }
}