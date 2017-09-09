using NodeInspector;

namespace PaintCraft.Tools.Filter{

    [NodeMenuItem("ChangePoint/RemoveAppliedPointsExceptLast")]
    public class RemoveAppliedPointsExceptLast : FilterWithNextNode {
		#region implemented abstract members of FilterWithNextNode

		public override bool FilterBody (BrushContext brushLineContext)
		{
			while (brushLineContext.Points.Count > 1 
			       && brushLineContext.Points.First.Value.Status == PointStatus.CopiedToCanvas 
			       && brushLineContext.Points.First.Next.Value.Status == PointStatus.CopiedToCanvas){
				brushLineContext.ForceRemoveFirstNodePoint();
			}
			return true;
		}

		#endregion



	}
}