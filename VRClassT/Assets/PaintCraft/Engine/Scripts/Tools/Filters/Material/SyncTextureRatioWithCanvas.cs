using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NodeInspector;

namespace PaintCraft.Tools.Filters.MaterialFilter{
    public enum TextureName{
        Main,
        Tile
    }

    [NodeMenuItemAttribute("Material/SyncTextureRatioWithCanvas")]
    public class SyncTextureRatioWithCanvas : FilterWithNextNode {
        public TextureName TextureNameInShader = TextureName.Main;
        public float HorizontalScale = 1.0f;
        public float VerticalScale = 1.0f;
        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody(BrushContext brushLineContext)
        {
            string textureName = TextureNameInShader == TextureName.Main ? "_MainTex" : "_TileTex";

            LinkedListNode<Point> node = brushLineContext.Points.Last;
            while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
                Texture materialTexture = node.Value.Material.GetTexture(textureName);
                Assert.IsNotNull(materialTexture, textureName + " texture must be set");
                float ratioX = brushLineContext.Canvas.Width / (float)materialTexture.width / HorizontalScale;
                float ratioY = brushLineContext.Canvas.Height / (float)materialTexture.height / VerticalScale;
                node.Value.Material.SetTextureScale(textureName, new Vector2(ratioX, ratioY));
                node = node.Previous;
            }
            return true;
        }
        #endregion
        
    }
}