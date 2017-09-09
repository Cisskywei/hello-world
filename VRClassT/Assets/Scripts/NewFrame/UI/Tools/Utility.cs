using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility {

    public static Utility getInstance()
    {
        return Singleton<Utility>.getInstance();
    }

    // 计算某一物体水平前方指定距离的vector3 
    public Vector3 CalculateAheadDistancePoint(Transform start, float distance)
    {
        //Quaternion rotation = new Quaternion(0, start.rotation.y, 0, start.rotation.w);
        Quaternion rotation = new Quaternion(0, start.rotation.y, 0, start.rotation.w);
        //rotation = Quaternion.Euler(90f, 0f, 0f) * rotation;
        Vector3 newPos = start.forward *distance;
        Debug.Log(newPos);
        return newPos;
    }

    // ui 设置图片透明度  默认变灰
    public void SetImageGray(Image img, float a = 0.4f)
    {
        if(img == null)
        {
            return;
        }

        Color toc = img.color;
        if (toc != null)
        {
            toc.a = a;
        }

        img.color = toc;
    }
}
