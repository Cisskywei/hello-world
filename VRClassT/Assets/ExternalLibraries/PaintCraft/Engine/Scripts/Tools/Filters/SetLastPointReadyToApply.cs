using NodeInspector;


namespace PaintCraft.Tools.Filters{
    [NodeMenuItem("ChangePoint/SetLastPointReadyToApply")]
    public class SetLastPointReadyToApply : FilterWithNextNode {
		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			brushLineContext.Points.Last.Value.Status = PointStatus.ReadyToApply;
			return true;
		}
		#endregion
		

	}
}