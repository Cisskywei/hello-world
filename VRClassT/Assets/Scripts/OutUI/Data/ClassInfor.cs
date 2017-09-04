using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfor {

    public Dictionary<int, PlayerInfor> playerlist = new Dictionary<int, PlayerInfor>(); // 所有学生列表
    public int Count
    {
        get
        {
            return playerlist.Count;
        }
    }

    public string classname = "班级1";
    public int classid = 1;

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void InitStudent()
    {
        DataType.StudentInfor[] s = UserInfor.getInstance().courseinfor.students;

        if (s == null || s.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < s.Length; i++)
        {
            PlayerInfor p = new PlayerInfor(s[i]);
            playerlist.Add((int)p.userid, p);
        }

        classname = "班级1";
        classid = 1;
    }

}
