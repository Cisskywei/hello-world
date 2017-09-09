using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseContentManager {

    public static CourseContentManager getInstance()
    {
        return Singleton<CourseContentManager>.getInstance();
    }

    public string coursename;
    public string chapter;

    public Dictionary<int, ContentInfor> contentlist = new Dictionary<int, ContentInfor>();

    public void InitDefault()
    {
        ContentInfor c = new ContentInfor();
        c.InitContent();

        contentlist.Add(c.id, c);
    }

    public ContentInfor GetContentById(int id)
    {
        if (contentlist.ContainsKey(id))
        {
            return contentlist[id];
        }

        return null;
    }

    public Dictionary<int, ContentInfor> GetAllContent()
    {
        return contentlist;
    }
}
