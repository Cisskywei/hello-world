using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Randomizers{
	[NodeMenuItem("Randomizers/RandomRotation")]
    public class RandomRotation : FilterWithNextNode {
		public float MinAngle = 0.0f;
		public float MaxAngle = 360.0f;
		#region implemented abstract members of FilterWithNextNode
		
		public override bool FilterBody (BrushContext brushLineContext)
		{
			
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas) {
				
				node.Value.Rotation = Random.Range(MinAngle, MaxAngle);
				node = node.Previous;
			}
			return true;
		}
		
		#endregion
		
	}
}