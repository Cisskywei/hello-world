using NodeInspector;


namespace PaintCraft.Tools.Filter{
	[NodeMenuItem("ChangePoint/IgnorePositionsCloseThanSpacing")]
    public class IgnorePositionsCloseThanSpacing : FilterWithNextNode {
		public SpacingProperty Spacing; 
		#region implemented abstract members of FilterWithNextNode

        public override bool FilterBody (BrushContext brushLineContext)
		{			
            bool latestPointRemoved = brushLineContext.RemoveLastPointIfDistanceToPreviousLessThan(Spacing.GetSpacingValue(brushLineContext));
            return !latestPointRemoved;
		}

		#endregion


	}
}