using UnityEngine;
using System.Collections.Generic;
using PaintCraft.Tools;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
	[NodeMenuItem("Predefined Regions/FloodFillRegion")]
    public class FloodFillFilter : FilterWithNextNode {
		#region implemented abstract members of FilterWithNextNode
        public override bool FilterBody (BrushContext brushLineContext)
		{
			while(brushLineContext.Points.Count > 0){
				brushLineContext.ForceRemoveLastNodePoint();			
			}

            if (brushLineContext.IsLastPointInLine) {
				LinkedListNode<Point> node = BrushContext.GetPointNode();
				node.Value.Position = brushLineContext.Canvas.transform.position;
				node.Value.Status = PointStatus.ReadyToApply;
				node.Value.Size   = brushLineContext.Canvas.Size;
				node.Value.Scale  = 1.0f;
			    node.Value.PointColor = brushLineContext.LineConfig.Color;
			    brushLineContext.Points.AddLast(node);			   
				return true;
			} else {
				return false;
			}
		}
		#endregion



	}
}