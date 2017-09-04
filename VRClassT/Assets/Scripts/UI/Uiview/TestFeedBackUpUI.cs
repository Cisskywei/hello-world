using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFeedBackUpUI : uibase
{
    public Text stem;
    public Text[] options;
    public Image[] optionimage; // 正确选项的图片

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void ShowSelf(string content, string []option, ArrayList anwser)
    {
        Init(content, option, anwser);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init(string content, string[] option, ArrayList correct)
    {
        if(content == null || option == null || option.Length <= 0)
        {
            return;
        }

        if(stem != null)
        {
            stem.text = content;
        }

        if (options != null && options.Length > 0)
        {
            for(int i=0;i<option.Length;i++)
            {
                if(options[i] != null)
                {
                    options[i].text = option[i];
                }

                if(optionimage[i]!=null)
                {
                    if(correct.Contains(i))
                    {
                        optionimage[i].enabled = true;
                    }
                    else
                    {
                        optionimage[i].enabled = false;
                    }
                }
            }
        }
    }
}
