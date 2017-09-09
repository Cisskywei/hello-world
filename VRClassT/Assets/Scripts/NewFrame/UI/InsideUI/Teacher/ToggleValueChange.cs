using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleValueChange : MonoBehaviour {

    public GameObject what;

    // Use this for initialization
    void Start()
    {
        if(what == null)
        {
            what = gameObject;
        }
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void ShowHide(Toggle to)
    {
        what.SetActive(to.isOn);
    }
}
