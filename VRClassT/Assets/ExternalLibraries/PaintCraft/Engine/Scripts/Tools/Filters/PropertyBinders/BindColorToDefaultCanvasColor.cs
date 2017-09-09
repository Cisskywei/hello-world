using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders
{
    [NodeMenuItem("PropertyBinders/BindColorToDefaultCanvasColor")]
    public class BindColorToDefaultCanvasColor : FilterWithNextNode {

        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas)
            {

                node.Value.PointColor.Color = brushLineContext.Canvas.DefaultBGColor;

                node = node.Previous;
            }
            return true;
        }
        #endregion
    }
}
