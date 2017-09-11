using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制大屏显示摄像机
/// </summary>
public class ScreenCameraCtrl : MonoBehaviour {

    // 摄像机接受指令对应的位置、角度
    public Transform[] places;

    public void ChangePlace(int id)
    {
        if (places == null || places.Length <= id || places[id] == null)
        {
            return;
        }

        if(transform.parent != null)
        {
            transform.parent = null;
        }

        transform.position = places[id].position;
        transform.rotation = places[id].rotation;
    }

    public void ChangeView(int userid)
    {
        GameObject role = RoleManager.getInstance().GetPlayerById(userid);
        Transform head = role.transform.GetChild(0);

        transform.parent = head;
        transform.position = Vector3.zero;
    }
}
