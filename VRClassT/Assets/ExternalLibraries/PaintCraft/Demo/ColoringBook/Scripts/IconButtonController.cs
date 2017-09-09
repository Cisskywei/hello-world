using UnityEngine;
using System.Collections.Generic;
using System.IO;
using PaintCraft.Canvas;
using PaintCraft.Canvas.Configs;
using PaintCraft;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PaintCraft.Demo.ColoringBook{ 

    public class IconButtonController : MonoBehaviour
    {
        public PageConfig Page;
    	
    	void Start ()
    	{
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
            

            if (File.Exists(Page.IconSavePath) && Page.name!=null)
    	    {
                Texture2D tex = new Texture2D(440,330, TextureFormat.RGB24, false);
    	        if (tex.LoadImage(File.ReadAllBytes(Page.IconSavePath)))
    	        {
                    tex.Apply(false, true);
                    button.targetGraphic.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, 440, 330), new Vector2(0.5f, 0.5f));
                }            
    	    }

            
    	    if (Page is ColoringPageConfig)
    	    {
                transform.GetChild(0).GetComponent<Image>().sprite = (Page as ColoringPageConfig).Icon;                
    	    }            
        }

        void OnButtonClick()
        {
            AppData.SelectedPageConfig = Page;
            AnalyticsWrapper.CustomEvent("SelectPicture", new Dictionary<string, object>
            {
                { "PictureName", Page.name}        
            });
            SceneManager.LoadScene("ColoringBook");
        }        
    }
}