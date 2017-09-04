using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PaintCraft.Controllers;
using PaintCraft.Canvas.Configs;

namespace Paintcraft.UI{
    public class SetActivePageconfig : MonoBehaviour, IPointerClickHandler {
        public CanvasController PaintcraftCanvas;
        public PageConfig PageConfig;


        #region IPointerClickHandler implementation
        public void OnPointerClick(PointerEventData eventData)
        {
            PaintcraftCanvas.SetActivePageConfig(PageConfig);
        }
        #endregion
    }
}
