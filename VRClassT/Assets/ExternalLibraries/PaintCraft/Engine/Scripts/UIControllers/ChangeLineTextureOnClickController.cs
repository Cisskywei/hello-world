using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Tools;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;


namespace PatinCraft.UI{    
    public class ChangeLineTextureOnClickController : MonoBehaviour, IPointerClickHandler {
        public LineConfig LineConfig;
        public Texture Texture;
    	
        #region IPointerClickHandler implementation
        public void OnPointerClick(PointerEventData eventData)
        {
            LineConfig.Texture = Texture;
        }
        #endregion


        [ContextMenu("setup icon")]
        public void SetupIcon(){
            Object.DestroyImmediate( gameObject.transform.GetChild(0).gameObject.GetComponent<Text>());
            gameObject.transform.GetChild(0).gameObject.AddComponent<RawImage>().texture = Texture;
        }
    }
}