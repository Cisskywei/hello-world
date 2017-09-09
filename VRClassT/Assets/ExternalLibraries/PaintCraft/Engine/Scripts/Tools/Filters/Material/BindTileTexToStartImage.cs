using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NodeInspector;


namespace PaintCraft.Tools.Filters.MaterialFilter{
    [NodeMenuItemAttribute("Material/BindTileTexToStartImage")]
    public class BindTileTexToStartImage : FilterWithNextNode {
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            
            if (brushLineContext.IsFirstPointInLine && brushLineContext.Canvas.PageConfig.StartImageTexture != null){
                brushLineContext.Points.First.Value.Material.SetTexture("_TileTex", brushLineContext.Canvas.PageConfig.StartImageTexture);
                brushLineContext.Points.First.Value.Material.SetTextureOffset("_TileTex", Vector2.zero);
                brushLineContext.Points.First.Value.Material.SetTextureScale ("_TileTex", Vector2.one);
            }
            return true;
        }
        #endregion
        
    }
}