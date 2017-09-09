using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeColor : MonoBehaviour {

    public enum UIType
    {
        None = 0,
        Text,
        Image,
        Toggle,
        Button,
    }

    public enum ChangeType
    {
        None = 0,
        TextColor,
        ObjectShowHide,
    }

    public ChangeType changewhat = ChangeType.None;
    public UIType uiwhat = UIType.None;
    public GameObject changewho;

    public Color _colorto;
    private Color _colororign;

    // Use this for initialization
    void Start () {
		if(changewho == null)
        {
            changewho = gameObject;
        }
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void ChangeTextColor(bool ischange)
    {
        switch(uiwhat)
        {
            case UIType.Text:
                Text t = changewho.GetComponent<Text>();
                if (_colororign == null)
                {
                    _colororign = t.color;
                }

                if(ischange)
                {
                    t.color = _colorto;
                }
                else
                {
                    t.color = _colororign;
                }
                break;
            case UIType.Image:
                break;
            case UIType.Toggle:
                break;
            case UIType.Button:
                break;
            default:
                break;
        }
        
    }

    public void ChangeTextColor(GameObject go)
    {
        if (changewho == null)
        {
            changewho = gameObject;
        }

        switch (uiwhat)
        {
            case UIType.Toggle:
                bool ison = go.GetComponent<Toggle>().isOn;
                Text t = changewho.GetComponent<Text>();
                if (ison)
                {
                    t.color = _colorto;
                }
                else
                {
                    t.color = _colororign;
                }
                break;
            case UIType.Button:
                break;
            default:
                break;
        }
    }

    public void ChangeShowHide(bool isshow)
    {
        if(changewho == null)
        {
            changewho = gameObject;
        }

        changewho.SetActive(isshow);
    }

    public void ChangeShowHide(GameObject go)
    {
        if (changewho == null)
        {
            changewho = gameObject;
        }

        switch (uiwhat)
        {
            case UIType.Toggle:
                bool ison = go.GetComponent<Toggle>().isOn;
                changewho.SetActive(ison);
                break;
            case UIType.Button:
                break;
            default:
                break;
        }
    }
}
