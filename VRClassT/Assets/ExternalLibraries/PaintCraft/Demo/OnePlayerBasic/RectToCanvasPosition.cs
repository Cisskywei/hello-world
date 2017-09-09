using UnityEngine;
using System.Collections;
using PaintCraft.Controllers;
using UnityEngine.Assertions;


[RequireComponent(typeof(RectTransform))]
public class RectToCanvasPosition : MonoBehaviour {
    public ScreenCameraController ScreenCameraController;
    public Camera UICamera;
    RectTransform rt;

    void Awake(){
        rt = gameObject.transform as RectTransform;
        rt.hasChanged = true;
        Assert.IsNotNull(rt);
    }
	
	// Update is called once per frame
	void Update () {        
        rt.hasChanged = false;
        Vector3[] corners = GetScreenRect(rt);
        ScreenCameraController.CameraSize.ViewPortOffset.left = (int)corners[0].x;
        ScreenCameraController.CameraSize.ViewPortOffset.bottom = (int)corners[0].y;
        ScreenCameraController.CameraSize.ViewPortOffset.right = Screen.width - (int)corners[2].x;
        ScreenCameraController.CameraSize.ViewPortOffset.top = Screen.height - (int)corners[2].y;
        ScreenCameraController.CameraSize.Init(ScreenCameraController.Camera, ScreenCameraController.Canvas);
	}

    public Vector3[] GetScreenRect(RectTransform rectTransform) {
        Vector3[] corners  = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            corners[i] =RectTransformUtility.WorldToScreenPoint(UICamera, corners[i]);
        }
        return corners;
       
    }
}
