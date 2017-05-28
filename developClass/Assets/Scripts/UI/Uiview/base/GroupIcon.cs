using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupIcon : MonoBehaviour {

    public Text namenum;

    private string name;
    private int num;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Init(string name, int num)
    {
        this.name = name;
        this.num = num;

        namenum.text = name + "(" + num.ToString() + "人" + ")";
    }
}
