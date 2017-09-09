using UnityEngine;
using PaintCraft.Utils;


namespace PaintCraft.Controllers
{
	public class BackLayerController : MonoBehaviour {
        
        public RenderTexture RenderTexture { get ; private set;}
		CanvasController canvas;
    

		public void Init(CanvasController canvas){
			this.canvas = canvas;
			UpdateMeshSize();	
		}

        void UpdateMeshSize(){    
			MeshFilter mf = GOUtil.CreateComponentIfNoExists<MeshFilter>(gameObject);
			Mesh mesh = MeshUtil.CreatePlaneMesh(canvas.Width, canvas.Height);
			mf.mesh = mesh;
			MeshRenderer mr = GOUtil.CreateComponentIfNoExists<MeshRenderer>(gameObject);

	        string shaderName = canvas.DefaultBGColor.a == 1.0 ? "Unlit/Texture" : "Unlit/Transparent";
	        mr.material = new Material(Shader.Find(shaderName));
            RenderTexture = TextureUtil.SetupRenderTextureOnMaterial(mr.material, canvas.RenderTextureSize.x, canvas.RenderTextureSize.y);
		}

        public void SetNewSize(){                
            canvas.CanvasCameraController.Camera.targetTexture = null;
            RenderTexture = TextureUtil.UpdateRenderTextureSize(RenderTexture, canvas.RenderTextureSize.x, canvas.RenderTextureSize.y);
            GetComponent<MeshRenderer>().material.mainTexture = RenderTexture;
            canvas.CanvasCameraController.Camera.targetTexture = RenderTexture;
            MeshUtil.ChangeMeshSize(GetComponent<MeshFilter>().mesh, canvas.RenderTextureSize.x, canvas.RenderTextureSize.y);
        }
	}
}
