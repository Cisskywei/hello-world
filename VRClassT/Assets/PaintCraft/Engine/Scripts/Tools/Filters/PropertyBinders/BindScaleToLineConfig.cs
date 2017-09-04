using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders{
	[NodeMenuItem("PropertyBinders/BindScaleToLineConfig")]
    public class BindScaleToLineConfig : FilterWithNextNode { 

		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				node.Value.Scale = brushLineContext.LineConfig.Scale;
				node = node.Previous;
			}
			return true;
		}
		#endregion
	}
}