using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeInspector;


namespace PaintCraft.Tools.CustomFilters.Logic{
    [NodeMenuItemAttribute("Custom/SetDifferentMaterials")]
    public class SetDifferentMaterials : FilterWithNextNode {
        public Material FirstPointMaterial ;
        public Material RemainingPointsMaterial;

        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {            
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            bool firstNotSet = true;
            while (node != null) {
                if (firstNotSet && !node.Value.IsBasePoint){                    
                    node.Value.Material = FirstPointMaterial;
                    firstNotSet = false;
                } else {                    
                    node.Value.Material = RemainingPointsMaterial;
                }
                node = node.Previous;
            }
            return true;
        }
        #endregion
        
       
    }
}
