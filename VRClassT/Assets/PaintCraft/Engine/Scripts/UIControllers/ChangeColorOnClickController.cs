using UnityEngine;
using System.Collections;
using PaintCraft.Tools;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace PatinCraft.UI{
    public class ChangeColorOnClickController : MonoBehaviour, IPointerClickHandler {
        public LineConfig LineConfig;
        public Color Color;
    	
    	void Start () {
            if (LineConfig == null){
                Debug.LogError("LineConfig must be provided", gameObject);
            }
    	}
    	

        #region IPointerClickHandler implementation
        public void OnPointerClick(PointerEventData eventData)
        {
            LineConfig.Color.Color = Color;
        }
        #endregion

        [ContextMenu("Copy Color from Image")]
        public void CopyColorImageComponent(){
            Image image = GetComponent<Image>();
            if (image != null){                
                this.Color = image.color;
            }
        }
    }
}