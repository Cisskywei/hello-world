using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters{
    /// <summary>
    /// Base point to normal.
    /// Base points used in interpolators, and they are not used in renderer.
    /// If you use this filters it means this points will be rendered, but interpolators wont work after that.
    /// 
    /// Common use case is: stamps - you don't need to draw the line, but you need to show user interaction with swatch
    /// </summary>
    [NodeMenuItem("ChangePoint/ConvertAllBasePointsToNormal")]
    public class ConvertAllBasePointsToNormal : FilterWithNextNode {
        
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody (BrushContext brushLineContext)
        {

            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null){                
                node.Value.IsBasePoint = false;
                node = node.Previous;
            }
            return true;
        }
        #endregion
    }
}