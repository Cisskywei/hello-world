using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Randomizers{
	[NodeMenuItem("Randomizers/RandomHue")]
    public class RandomHue : FilterWithNextNode {

		#region implemented abstract members of FilterWithNextNode
		
		public override bool FilterBody (BrushContext brushLineContext)
		{			
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas) {
				node.Value.PointColor.H = Random.Range(0, 1.0f);
				node = node.Previous;
			}
			return true;
		}
		
		#endregion
		
	}
}