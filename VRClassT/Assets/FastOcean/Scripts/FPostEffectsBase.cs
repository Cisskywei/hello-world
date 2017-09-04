/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/


using UnityEngine;
using System.Collections;

namespace FastOcean
{
    [RequireComponent(typeof(Camera))]
    public class FPostEffectsBase : MonoBehaviour 
	{	
		protected bool supportHDRTextures = true;
		protected bool isSupported = true;
        
        /// <summary>
        /// A reference to the camera this component is added to.
        /// </summary>
        protected Camera m_Camera;

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	    {
			if (!s) { 
				Debug.Log("Missing shader in " + this.ToString ());
				enabled = false;
				return null;
			}
				
			if (s.isSupported && m2Create && m2Create.shader == s) 
				return m2Create;
			
			if (!s.isSupported) {
				NotSupported ();
				Debug.Log("The shader " + s.ToString() + " on effect "+this.ToString()+" is not supported on this platform!");
				return null;
			}
			else {
				m2Create = new Material (s);
#if UNITY_EDITOR
                m2Create.hideFlags = HideFlags.DontSave;
#endif
                if (m2Create) 
					return m2Create;
				else return null;
			}
		}

	    protected Material CreateMaterial(Shader s, Material m2Create)
	    {
			if (!s) { 
				Debug.Log ("Missing shader in " + this.ToString ());
				return null;
			}
				
			if (m2Create && (m2Create.shader == s) && (s.isSupported)) 
				return m2Create;
			
			if (!s.isSupported) {
				return null;
			}
			else {
				m2Create = new Material (s);
#if UNITY_EDITOR
                m2Create.hideFlags = HideFlags.DontSave;
#endif
                if (m2Create) 
					return m2Create;
				else return null;
			}
		}
		
		void OnEnable() {
			isSupported = true;
		}

	    protected virtual bool CheckResources()
	    {
			Debug.LogWarning ("CheckResources () for " + this.ToString() + " should be overwritten.");
			return isSupported;
		}
		
		protected virtual void Start () {
			 CheckResources ();
		}

        protected virtual void CheckEnable()
        {
        }

        protected  bool CheckSupport ()  {
			isSupported = true;
			supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
			
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
				NotSupported ();
				return false;
			}		
			
			if(!SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth)) {
				NotSupported ();
				return false;
			}
			
			return true;
		}

	    protected bool CheckSupport(bool needHdr)
	    {
			if(!CheckSupport())
				return false;
			
			if(needHdr && !supportHDRTextures) {
				NotSupported ();
				return false;		
			}
			
			return true;
		}

	    protected void ReportAutoDisable()
	    {
			Debug.LogWarning ("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
		}
				
		// deprecated but needed for old effects to survive upgrading
	    protected bool CheckShader(Shader s) 
	    {
			Debug.Log("The shader " + s.ToString () + " on effect "+ this.ToString () + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");		
			if (!s.isSupported) {
				NotSupported ();
				return false;
			} 
			else {
				return false;
			}
		}

	    protected void NotSupported()
	    {
			enabled = false;
			isSupported = false;
			return;
		}

	    protected void DrawBorder (RenderTexture dest, Material material) {
			float x1;	
			float x2;
			float y1;
			float y2;

            RenderTexture tmp = RenderTexture.active;

            RenderTexture.active = dest;
	        bool invertY = true; // source.texelSize.y < 0.0f;
	        // Set up the simple Matrix
	        GL.PushMatrix();
	        GL.LoadOrtho();		
	        
	        for (int i = 0; i < material.passCount; i++)
	        {
	            material.SetPass(i);
		        
		        float y1_; float y2_;
		        if (invertY)
		        {
		            y1_ = 1.0f; y2_ = 0.0f;
		        }
		        else
		        {
		            y1_ = 0.0f; y2_ = 1.0f;
		        }
		        	        
		        // left	        
		        x1 = 0.0f;
		        x2 = 0.0f + 1.0f/(dest.width*1.0f);
		        y1 = 0.0f;
		        y2 = 1.0f;
		        GL.Begin(GL.QUADS);
		        
		        GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
		        GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
		        GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
		        GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);
		
		        // right
		        x1 = 1.0f - 1.0f/(dest.width*1.0f);
		        x2 = 1.0f;
		        y1 = 0.0f;
		        y2 = 1.0f;

		        GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
		        GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
		        GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
		        GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);	        
		
		        // top
		        x1 = 0.0f;
		        x2 = 1.0f;
		        y1 = 0.0f;
		        y2 = 0.0f + 1.0f/(dest.height*1.0f);

		        GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
		        GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
		        GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
		        GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);
		        
		        // bottom
		        x1 = 0.0f;
		        x2 = 1.0f;
		        y1 = 1.0f - 1.0f/(dest.height*1.0f);
		        y2 = 1.0f;

		        GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
		        GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
		        GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
		        GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);	
		                	              
		        GL.End();	
	        }	
	        
	        GL.PopMatrix();

            RenderTexture.active = tmp;
		}
		
	}
}
