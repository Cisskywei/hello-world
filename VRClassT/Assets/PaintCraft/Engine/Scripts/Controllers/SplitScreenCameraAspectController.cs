using UnityEngine;

namespace ColoringBook.Controllers
{
    public class SplitScreenCameraAspectController : MonoBehaviour {
        Camera cam;

        void Start(){
            cam = GetComponent<Camera> ();
        }
	
        // Update is called once per frame
        void Update () {
            cam.aspect =   (float)Screen.width * 0.5f / (float)Screen.height;
        }
    }
}
