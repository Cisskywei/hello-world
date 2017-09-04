using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class ClassManager {

    public static ClassManager getInstance()
    {
        return Singleton<ClassManager>.getInstance();
    }

    public Dictionary<int, ClassInfor> classlist = new Dictionary<int, ClassInfor>();

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void InitDefault()
    {
        ClassInfor c = new ClassInfor();
        c.InitStudent();

        classlist.Add(c.classid, c);
    }

    public ClassInfor GetClassById(int id)
    {
        if(classlist.ContainsKey(id))
        {
            return classlist[id];
        }

        return null;
    }

    public Dictionary<int, ClassInfor> GetAllClass()
    {
        return classlist;
    }

    public List<ClassInfor> GetAllClassList()
    {
        return new List<ClassInfor>(classlist.Values);
    }
}
