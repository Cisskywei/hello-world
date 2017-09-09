using UnityEngine;
using NodeInspector;
using System.Collections.Generic;


namespace PaintCraft.Tools.Filters.MaterialFilter{
    [NodeMenuItem("Material/BindMainTexToLineConfig")]
    public class BindMainTexToLineConfig  : FilterWithNextNode {
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {

            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
                node.Value.Material.SetTexture("_MainTex", brushLineContext.LineConfig.Texture);
                node = node.Previous;
            }
            return true;
        }
        #endregion
    }
}