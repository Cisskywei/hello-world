using System.Collections.Generic;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
	public enum ChangeType{
		Red,
		Green,
		Blue,
		Alpha,
		Hue,
		Saturation,
		Value
	}

    [NodeMenuItem("PropertyBinders/BindColorToScale")]
    public class BindColorToScale : FilterWithNextNode {
		public ChangeType ChangeType;


		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				switch(ChangeType){
				case ChangeType.Alpha     : node.Value.PointColor.Alpha = node.Value.Scale; break;
				case ChangeType.Red       : node.Value.PointColor.R     = node.Value.Scale; break;
				case ChangeType.Green     : node.Value.PointColor.G     = node.Value.Scale; break;
				case ChangeType.Blue      : node.Value.PointColor.B     = node.Value.Scale; break;
				case ChangeType.Hue       : node.Value.PointColor.H     = node.Value.Scale; break;
				case ChangeType.Saturation: node.Value.PointColor.S     = node.Value.Scale; break;
				case ChangeType.Value     : node.Value.PointColor.V     = node.Value.Scale; break;
				}
				node = node.Previous;
			}
			return true;
		}
		#endregion
	}
}