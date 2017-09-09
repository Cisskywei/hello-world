using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Randomizers
{
    [NodeMenuItem("Randomizers/RandomColor")]
    public class RandomColor : FilterWithNextNode
    {
        
        #region implemented abstract members of FilterWithNextNode

        public override bool FilterBody(BrushContext brushLineContext)
        {
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas)
            {
                node.Value.PointColor.Color = new Color(Random.value,Random.value,Random.value, 1.0f);
                node = node.Previous;
            }
            return true;
        }

        #endregion

    }
}