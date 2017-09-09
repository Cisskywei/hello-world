using UnityEngine;
using UnityEngine.UI;

namespace PatinCraft.UI{    
    public class FullScreenButtonController : MonoBehaviour {
        void Start(){
            GetComponent<Button> ().onClick.AddListener (OnButtonClick);
        }

        void OnButtonClick ()
        {
            if (Screen.fullScreen) {
                Screen.SetResolution(Screen.width, Screen.height, false);
            } else {
                Screen.SetResolution(Screen.width, Screen.height, true);
            }
        }
    }    
}