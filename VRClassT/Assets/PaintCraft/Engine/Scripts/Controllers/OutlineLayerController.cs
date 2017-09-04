using UnityEngine;
using PaintCraft.Utils;


namespace PaintCraft.Controllers
{
	public class OutlineLayerController: MonoBehaviour {
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
			mr.material = canvas.OutlineMaterial;
			canvas.OutlineMaterial.mainTexture = canvas.OutlineTexture;			
		}

        public void SetNewSize(){
            MeshFilter mf = GOUtil.CreateComponentIfNoExists<MeshFilter>(gameObject);
            MeshUtil.ChangeMeshSize( mf.mesh, canvas.Width, canvas.Height);
            canvas.OutlineMaterial.mainTexture = canvas.OutlineTexture;
        }

	}
}
