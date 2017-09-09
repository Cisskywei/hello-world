using UnityEngine;
using System.Collections.Generic;
using PaintCraft.Utils;
using NodeInspector;
using System;
using System.Collections;


namespace PaintCraft.Tools.Filters.MaterialFilter{

    [Obsolete("please use RenderSwatchWithPointMaterial")]
    [NodeMenuItem("Renderer/RenderSwatch (Obsolete, use RenderSwatchWithPointMaterial)")]
    public class RenderSwatch : FilterWithNextNode {
        public Material NormalMaterial;
        public Material RegionMaterial;        

		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> point = null;
			Queue<Mesh> usedMeshes = new Queue<Mesh>();
			point = brushLineContext.Points.Last;
			Vector2 firstPointUV = brushLineContext.FirstPointUVPosition;
			Mesh mesh;
            Material mat;
			while (point != null ){
				if (!point.Value.IsBasePoint){
                    Vector3 pointPosition = point.Value.Position;
                    pointPosition.z = brushLineContext.Canvas.transform.position.z + brushLineContext.Canvas.BrushOffset;
					
                    mesh = GetMesh(brushLineContext, point.Value, firstPointUV);
					usedMeshes.Enqueue(mesh);
                    mat = MaterialCache.GetMaterial(brushLineContext, NormalMaterial, RegionMaterial);
                    if (point.Value.Status == PointStatus.ReadyToApply){                                                
                        Graphics.DrawMesh(mesh, pointPosition, Quaternion.Euler(0,0, point.Value.Rotation), mat, 
                            brushLineContext.Canvas.BrushLayerId, brushLineContext.Canvas.CanvasCameraController.Camera);                       
						point.Value.Status = PointStatus.CopiedToCanvas;                                 
					} else if (point.Value.Status == PointStatus.Temporary){                                                						
                        Graphics.DrawMesh(mesh, pointPosition, Quaternion.Euler(0,0, point.Value.Rotation), mat, 
                            brushLineContext.Canvas.TempRenderLayerId);						
					}
				}

				point = point.Previous;
			}
			brushLineContext.Canvas.StartCoroutine(FlushMeshesOnNextFrame(usedMeshes));
			return true;
		}

	    IEnumerator FlushMeshesOnNextFrame(Queue<Mesh> meshes)
	    {
		    		    
		    yield return new WaitForEndOfFrame();		    
		    FlushUsedMeshes(meshes);		    
	    }


	    private static Queue<Mesh> _meshPool;
	    private static Queue<Mesh> MeshPool
	    {
		    get
		    {
			    if (_meshPool == null)
			    {				    
				    _meshPool = new Queue<Mesh>();
			    }
			    return _meshPool;
		    }		    
	    }

	    private Queue<Mesh> _flushMeshInProgress = null;

	    Mesh GetMesh(BrushContext brushLineContext, Point point, Vector2 firstPointUv){
			float width = point.Size.x * point.Scale;
			float height = point.Size.y * point.Scale;
			width = Mathf.Max(brushLineContext.Brush.MinSize.x, width);
			height = Mathf.Max(brushLineContext.Brush.MinSize.y, height);
			Mesh result;
			if (MeshPool.Count > 0){
				result = MeshPool.Dequeue();
				MeshUtil.ChangeMeshSize(result, width, height);
			} else {				
                result = MeshUtil.CreatePlaneMesh(width, height);
            }
			MeshUtil.ChangeMeshColor(result,  point.PointColor.Color);
            MeshUtil.UpdateMeshUV2(result, width, height, point.Position, point.Rotation, (float)brushLineContext.Canvas.Width, (float)brushLineContext.Canvas.Height, brushLineContext.Canvas.transform.position);
			MeshUtil.UpdateMeshUV3(result, firstPointUv);
			return result;
		}

		void FlushUsedMeshes(Queue<Mesh> usedMeshes){            			
			while (usedMeshes.Count > 0){				
				MeshPool.Enqueue(usedMeshes.Dequeue());
			}            
		}

		#endregion
	}
}
