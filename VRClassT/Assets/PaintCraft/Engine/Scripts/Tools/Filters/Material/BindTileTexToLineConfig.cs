using UnityEngine;
using NodeInspector;
using System.Collections.Generic;


namespace PaintCraft.Tools.Filters.MaterialFilter{
    [NodeMenuItem("Material/BindTileTexToLineConfig")]
    public class BindTileTexToLineConfig : FilterWithNextNode {
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
                node.Value.Material.SetTexture("_TileTex", brushLineContext.LineConfig.Texture);
                node = node.Previous;
            }
            return true;
        }
        #endregion
    	
    }
}