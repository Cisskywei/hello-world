using UnityEngine;
using UnityEngine.UI;

namespace PaintCraft.Demo.ColoringBook{ 
    public class OpenURLButtonController : MonoBehaviour
    {
        public string Url;

        void Start()
        {
    #if UNITY_WINRT_8_1
            GetComponent<Button>().onClick.AddListener(() => UnityEngine.WSA.Launcher.LaunchUri(Url, false));
    #else
            GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(Url));
    #endif
        }
    }
}