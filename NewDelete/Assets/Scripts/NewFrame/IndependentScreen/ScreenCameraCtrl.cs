using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制大屏显示摄像机
/// </summary>
public class ScreenCameraCtrl : MonoBehaviour {

    // 摄像机接受指令对应的位置、角度
    public Transform[] places;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void ChangePlace(int id)
    {
        if(places == null || places.Length <= id || places[id] == null)
        {
            return;
        }

        transform.position = places[id].position;
        transform.rotation = places[id].rotation;
    }
}
