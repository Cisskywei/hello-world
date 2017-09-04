using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders{

	[NodeMenuItem("PropertyBinders/BindColorToLineConfig")]
    public class BindColorToLineConfig : FilterWithNextNode { 
		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				node.Value.PointColor.CopyFrom(brushLineContext.LineConfig.Color);
				node = node.Previous;
			}
			return true;
		}
		#endregion
	}
}