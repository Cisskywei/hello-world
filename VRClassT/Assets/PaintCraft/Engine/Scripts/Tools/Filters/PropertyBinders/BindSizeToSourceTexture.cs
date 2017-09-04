using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Tools;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders{
    [NodeMenuItemAttribute("PropertyBinders/BindSizeToMainTexture")]
    public class BindSizeToSourceTexture : FilterWithNextNode {
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null)
            {

                node.Value.Size.x = node.Value.Material.mainTexture.width;
                node.Value.Size.y = node.Value.Material.mainTexture.height;    
                node = node.Previous;
            }
            return true;
        }
        #endregion
        
    }    
}
