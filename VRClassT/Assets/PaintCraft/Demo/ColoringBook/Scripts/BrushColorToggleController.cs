using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using PaintCraft.Tools;
using PaintCraft;

namespace PaintCraft.Demo.ColoringBook{   
    public class BrushColorToggleController : MonoBehaviour {
    	public LineConfig LineConfig;
        public String ColorName;
        public Text ColorNameText;
        public Color Color;
        Toggle toggle;

        void Start () {
    		toggle = GetComponent<Toggle> ();
    		toggle.onValueChanged.AddListener (OnValueChanged);
            
    		if (toggle.isOn) {
    			OnValueChanged(true);
    		}
    	}

        void Update()
        {
            toggle.isOn = (LineConfig.Color.Color == Color);
        }

        
        void OnValueChanged (bool arg0)
    	{		
    		LineConfig.Color.Color = Color;
            if (ColorNameText != null){            
                ColorNameText.text = ColorName;
            }
            AnalyticsWrapper.CustomEvent("ColorChange", new Dictionary<string, object>
            {
                { "ColorName", ColorName}
             });
        }

        [ContextMenu("setup colors")]
        public void SetupColors()
        {
            string data = @".Cayenne { background-color: #800000; }
    .Asparagus { background-color: #808000; }
    .Clover  { background-color: #008000; }
    .Teal  { background-color: #008080; }
    .Midnight { background-color: #000080; }
    .Plum  { background-color: #800080; }
    .Tin  { background-color: #7F7F7F; }
    .Nickel  { background-color: #808080; }
    .Mocha  { background-color: #804000; }
    .Fern  { background-color: #408000; }
    .Moss  { background-color: #008040; }
    .Ocean  { background-color: #004080; }
    .Eggplant { background-color: #400080; }
    .Maroon  { background-color: #800040; }
    .Steel  { background-color: #666666; }
    .Aluminum { background-color: #999999; }
    .Marascino { background-color: #FF0000; }
    .Lemon  { background-color: #FFFF00; }
    .Spring  { background-color: #00FF00; }
    .Turquoise { background-color: #00FFFF; }
    .Blueberry { background-color: #0000FF; }
    .Magenta { background-color: #FF00FF; }
    .Iron  { background-color: #4C4C4C; }
    .Magnesium { background-color: #B3B3B3; }
    .Tangerine { background-color: #FF8000; }
    .Lime  { background-color: #80FF00; }
    .SeaFoam { background-color: #00FF80; }
    .Aqua  { background-color: #0080FF; }
    .Grape  { background-color: #8000FF; }
    .Strawberry { background-color: #FF0080; }
    .Tungsten { background-color: #333333; }
    .Silver  { background-color: #CCCCCC; }
    .Salmon  { background-color: #FF6666; }
    .Banana  { background-color: #FFFF66; }
    .Flora  { background-color: #66FF66; }
    .Ice  { background-color: #66FFFF; }
    .Orchid  { background-color: #6666FF; }
    .Bubblegum { background-color: #FF66FF; }
    .Lead  { background-color: #191919; }
    .Mercury { background-color: #E6E6E6; }
    .Cantaloupe { background-color: #FFCC66; }
    .Honeydew { background-color: #CCFF66; }
    .Spindrift { background-color: #66FFCC; }
    .Sky  { background-color: #66CCFF; }
    .Lavender { background-color: #CC66FF; }
    .Carnation { background-color: #FF6FCF; }
    .Licorice { background-color: #000000; }
    .Snow  { background-color: #FFFFFF; }";

            
            foreach (var line in data.Split(new string[] { Environment.NewLine},
            StringSplitOptions.RemoveEmptyEntries  ))
            {
                string name = line.Split(new char[] {' '})[0];
                name = name.Substring(1, name.Length - 1).ToLower();
                string hexCode = line.Split(new char[] {'#'})[1].Substring(0, 6); 
                int r= Convert.ToInt32(hexCode.Substring(0,2), 16);
                int g = Convert.ToInt32(hexCode.Substring(2, 2), 16);
                int b = Convert.ToInt32(hexCode.Substring(4, 2), 16);            
                GameObject newGo = GameObject.Instantiate(gameObject);
                
                BrushColorToggleController bc = newGo.GetComponent<BrushColorToggleController>();
                bc.ColorName = name;
                bc.Color = newGo.GetComponent<Toggle>().targetGraphic.color = new Color32((byte)r, (byte)g, (byte)b, 255);
                bc.transform.parent = gameObject.transform.parent;
                bc.transform.SetSiblingIndex(0);
                newGo.transform.localScale = Vector3.one;
                newGo.name = name;
            }
        
        }

    }
}