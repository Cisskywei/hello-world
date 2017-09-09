using System.Collections.Generic;
using UnityEngine;
using NodeInspector;
using PaintCraft.Canvas.Configs;


namespace PaintCraft.Tools.Filters.MaterialFilter{

    [NodeMenuItemAttribute("Material/SetMaterial")]
    public class SetMaterial : FilterWithNextNode {
        public Material Material;

        Material _material;

        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            if (brushLineContext.IsFirstPointInLine && _material == null){
                _material = new Material(Material);
            }

            if (_material.HasProperty("_RegionTex") && brushLineContext.IsFirstPointInLine
                && typeof(AdvancedPageConfig).IsAssignableFrom(brushLineContext.Canvas.PageConfig.GetType()) 
                && ((ColoringPageConfig)brushLineContext.Canvas.PageConfig).RegionTexture != null ){
                _material.SetTexture("_RegionTex", ((ColoringPageConfig)brushLineContext.Canvas.PageConfig).RegionTexture);

                _material.SetFloat("_OriginX", brushLineContext.FirstPointUVPosition.x);
                _material.SetFloat("_OriginY", brushLineContext.FirstPointUVPosition.y);
            }
                
            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){                
                node.Value.Material = _material;
                node = node.Previous;
            }
            return true;
        }
        #endregion
        
    }
}