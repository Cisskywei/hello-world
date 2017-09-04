using UnityEngine;
using UnityEngine.UI;
using PaintCraft.Tools;
using UnityEngine.EventSystems;

namespace PatinCraft.UI{
    
    public class ChangeBrushOnClickController : MonoBehaviour, IPointerClickHandler {
        public LineConfig LineConfig;
        public Brush Brush;

        void Awake(){            
            if (LineConfig == null){
                Debug.LogError("LineConfig must be provided", gameObject);
            }

            if (Brush == null){
                Debug.LogError("Brush could not be null here", gameObject);
            }
        
            Toggle t = GetComponent<Toggle>();
            if (t!= null){
                string usedBrushName = PlayerPrefs.GetString(LineConfig.name);
                if (string.IsNullOrEmpty(usedBrushName)){
                    t.isOn = Brush == LineConfig.Brush;
                } else {                    
                    t.isOn = Brush.name == usedBrushName;
                    if (t.isOn){                        
                        LineConfig.Brush = Brush;
                    }
                }
            }

        }            
            
        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            LineConfig.Brush = Brush;
            PlayerPrefs.SetString(LineConfig.name, Brush.name);
            PlayerPrefs.Save();
        }

        #endregion
               
    }        
}