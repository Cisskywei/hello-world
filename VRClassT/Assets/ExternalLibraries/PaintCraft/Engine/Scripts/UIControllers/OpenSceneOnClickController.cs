using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using PaintCraft.Canvas.Configs;
using PaintCraft.Canvas;


namespace PatinCraft.UI{ 
    public class OpenSceneOnClickController : MonoBehaviour, IPointerClickHandler {
        public string SceneName;
    
        #region IPointerClickHandler implementation
        public void OnPointerClick(PointerEventData eventData)
        {            
            AppData.SelectedPageConfig = null;
            SceneManager.LoadScene(SceneName);
        }
        #endregion
    }
}