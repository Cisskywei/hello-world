using UnityEngine;
using System.Collections.Generic;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
    [NodeMenuItem("Randomizers/MoveToRandomOffset")]
    public class MoveToRandomOffset : FilterWithNextNode {
		[Tooltip("Radius in pixels")]
		public float Radius = 50;

	    public bool MultiplyToBrushSize = false;
		#region implemented abstract members of FilterWithNextNode

		public override bool FilterBody(BrushContext brushLineContext)
		{

			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas) {
			    if (MultiplyToBrushSize)
			    {
                    node.Value.Position += Random.insideUnitCircle * Radius * brushLineContext.LineConfig.Scale;
                }
			    else
			    {
			        node.Value.Position += Random.insideUnitCircle * Radius;
			    }
			    node = node.Previous;
			}
			return true;
		}

		#endregion


	}
}