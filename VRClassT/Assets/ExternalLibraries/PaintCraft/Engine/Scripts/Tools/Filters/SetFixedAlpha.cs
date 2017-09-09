using UnityEngine;
using System.Collections.Generic;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
	
    [NodeMenuItem("ChangePoint/SetFixedAlpha")]
    public class SetFixedAlpha : FilterWithNextNode {
		[Range(0f,1f)]
		public float Alpha;
		#region implemented abstract members of FilterWithNextNode

		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){				
				node.Value.PointColor.Alpha = Alpha;				
				node = node.Previous;
			}
			return true;
		}

		#endregion


	}
}