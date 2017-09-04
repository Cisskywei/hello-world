using PaintCraft.Tools;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
	[NodeMenuItem("ChangePoint/DrawFirstPoint")]
    public class DrawFirstPoint : FilterWithNextNode {        
		#region implemented abstract members of FilterWithNextNode
        public override bool FilterBody (BrushContext brushLineContext)
		{
			if (brushLineContext.Points.First.Value.IsFirstBasePointFromInput){
				if (brushLineContext.Points.First.Value.Status != PointStatus.CopiedToCanvas){
					if (brushLineContext.Points.Count > 1){
						brushLineContext.Points.First.Value.Status = PointStatus.ReadyToApply;
					} else {
						brushLineContext.Points.First.Value.Status = PointStatus.Temporary;
					}
				}
			}
            return true;
		}
		#endregion
	}
}
