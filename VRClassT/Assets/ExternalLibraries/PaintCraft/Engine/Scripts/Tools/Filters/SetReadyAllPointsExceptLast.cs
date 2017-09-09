using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters{
	[NodeMenuItem("ChangePoint/SetReadyAllPointsExceptLast")]
    public class SetReadyAllPointsExceptLast : FilterWithNextNode {
		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				node.Value.Status = PointStatus.ReadyToApply;
				node = node.Previous;
			}
			return true;
		}
		#endregion
		

	}
}
