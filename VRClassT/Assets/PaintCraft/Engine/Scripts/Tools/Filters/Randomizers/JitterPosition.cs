using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Randomizers{
    [NodeMenuItem("Randomizers/JitterPosition")]
    public class JitterPosition : FilterWithNextNode {
		public float Offset = 5.0f;
		#region implemented abstract members of FilterWithNextNode
		
        public override bool FilterBody(BrushContext brushLineContext)
		{
			
			LinkedListNode<Point> node = brushLineContext.Points.Last;
		    while (node != null && node.Value.Status != PointStatus.CopiedToCanvas) {
				float t = 2.0f * Mathf.PI * Random.value;
				float u = Random.value + Random.value;
				float r = Offset * (u > 1 ? 2 - u : u);

				node.Value.Position.x += r * Mathf.Cos(t);
				node.Value.Position.y += r * Mathf.Sin(t);
				node = node.Previous;
			}
			return true;
		}
		
		#endregion
		
	}
}