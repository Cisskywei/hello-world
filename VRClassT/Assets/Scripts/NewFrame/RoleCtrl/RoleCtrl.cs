using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrl : MonoBehaviour {

    public string playername = string.Empty;  // 头上的名字
    public int userid = -1;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Init(int userid)
    {
        this.userid = userid;

        PlayerInfor pi = ClassManager.getInstance().FindPlayerById(userid);

        if(pi != null)
        {
            playername = pi.student_name;
        }
    }

    public void Init(int userid, string name)
    {
        this.userid = userid;
        this.name = name;
    }
}
