using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class CourseIcon : MonoBehaviour {

    public Image icon;
    public Text id;
    public Text coursename;

    private int courseid = -1;
    private string avatar;

    public bool forceupdate = false;
    private bool isinit = false;

    private Sprite sp;

    //   // Use this for initialization
    //   void Start () {

    //}

    private void OnEnable()
    {
        if(avatar != null)
        {
            InitIcon(this.avatar);
        }
    }

    public void Init(DataType.CourseListData data)
    {
        if(data == null)
        {
            return;
        }

        if (forceupdate || !isinit)
        {
            id.text = data.course_id;
            coursename.text = data.course_name;

            courseid = Convert.ToInt32(data.course_id);
            avatar = data.cover;

            if (gameObject.activeInHierarchy)
            {
                InitIcon(data.cover);
            }

            isinit = true;
        }
    }
	
    public void InitIcon(string avatar)
    {
        if(sp == null || forceupdate)
        {
            if (UserInfor.RootUrl != null && avatar != null)
            {
                string url = UserInfor.RootUrl + avatar;
                StartCoroutine(LoadIamge(url));
            }
        }

        else if(sp != null)
        {
            if (icon != null)
            {
                icon.overrideSprite = sp;
            }
        }

        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;

    }

    // 点击函数
    public void OnClick()
    {
        if(courseid < 0)
        {
            return;
        }

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.ChooseCourse, this.courseid);
    }

    /// 辅助函数
    IEnumerator LoadIamge(string path)
    {
        WWW www = new WWW(path);
        yield return www;

        if(www.error != null)
        {
            Debug.Log(www.error);
            yield return null;
        }
        else
        {
            Texture2D texture = www.texture;

            if (texture != null)
            {
                sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                if (icon != null && sp != null)
                {
                    icon.overrideSprite = sp;
                }
            }
        }

    }
}
