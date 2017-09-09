using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeInspector;
using UnityEngine.Serialization;


namespace PaintCraft.Tools.Filters.Randomizers{
    [NodeMenuItemAttribute("Material/RandomTileTexOffsetOnBegining")]
    public class RandomTileTexOffsetOnBegining : FilterWithNextNode {
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            if (brushLineContext.IsFirstPointInLine
                && brushLineContext.Points.First.Value.IsBasePoint
                && brushLineContext.Points.First.Value.BasePointId == 0){
                Vector2 offset = new Vector2(Random.value * 100.0f, Random.value * 100.0f);                         
                brushLineContext.Points.First.Value.Material.SetTextureOffset("_TileTex", offset);
            } 
            return true;
        }
        #endregion
    }
}