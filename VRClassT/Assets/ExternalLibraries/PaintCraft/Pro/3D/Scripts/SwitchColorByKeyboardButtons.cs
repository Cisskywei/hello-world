using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Pro.Controllers;
using UnityEngine.Assertions;
using UnityEngine.UI;
using PaintCraft.Tools;

namespace PaintCraft.Pro.Controllers{
    [RequireComponent(typeof(LineConfig))]
    public class SwitchColorByKeyboardButtons : MonoBehaviour {
        public  GameObject UIPanelWithKeys;
        readonly KeyCode[] keys = new KeyCode[]{KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5};


        void Start(){
            Assert.IsNotNull(UIPanelWithKeys, "UIPanelWithKeys must be set");
            Assert.IsNotNull(GetComponent<LineConfig>(), "You must add LineConfig to the same object which use SwitchColorByKeyboardButtons");
            for (int i = 0; i < keys.Length; i++)
            {
                Outline outline = UIPanelWithKeys.transform.GetChild(i).gameObject.AddComponent<Outline>();
                outline.effectDistance = new Vector2(5,5);
                outline.enabled = false;
            }
            SetColorActive(2);
        }


        void Update () {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i])){
                    SetColorActive(i);
                }    
            }
                
        }

        int previousId = -1;
        void SetColorActive(int id){
            if (id == previousId){
                return;
            }
            if (previousId != -1){
                UIPanelWithKeys.transform.GetChild(previousId).GetComponent<Outline>().enabled = false;
            }

            GetComponent<LineConfig>().Color.Color = UIPanelWithKeys.transform.GetChild(id).GetComponent<Image>().color;
            UIPanelWithKeys.transform.GetChild(id).GetComponent<Outline>().enabled = true;
            previousId = id;
        }
    }

}
