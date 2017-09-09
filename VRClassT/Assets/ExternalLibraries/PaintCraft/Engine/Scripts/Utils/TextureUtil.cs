using UnityEngine;
using System.Collections;


namespace PaintCraft.Utils
{
	public class TextureUtil {
		
		public static RenderTexture SetupRenderTextureOnMaterial(Material mat, float width, float height){			
			mat.mainTexture = CreateRenderTexture(width, height);		
			return mat.mainTexture as RenderTexture;
		}

		public static RenderTexture CreateRenderTexture(float width, float height){
			RenderTexture result = new RenderTexture(Mathf.CeilToInt(width), Mathf.CeilToInt(height), 0, RenderTextureFormat.ARGB32);
            result.filterMode = FilterMode.Point;
            return result;
		}

        public static RenderTexture UpdateRenderTextureSize(RenderTexture renderTexture, float width, float height){
            RenderTexture result;
            if (renderTexture.width != Mathf.CeilToInt(width) || renderTexture.height != Mathf.CeilToInt(height)){
                renderTexture.Release();
                result = new RenderTexture(Mathf.CeilToInt(width), Mathf.CeilToInt(height), 0, RenderTextureFormat.ARGB32);
                result.filterMode = FilterMode.Point;
                return result;
            } else {
                return renderTexture;
            }
        }
	}
}