using UnityEngine;
using PaintCraft.Tools;
using System.Collections.Generic;
using PaintCraft.Utils;
using NodeInspector;
using PaintCraft.Controllers;


namespace PaintCraft.Tools.Filters{
    

	[NodeMenuItem("Renderer/RenderSwatchWithPointMaterial")]
    public class RenderSwatchWithPointMaterial : FilterWithNextNode {        
		int lastUsedFrame = -1;

		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
            Camera tempCamera = null;
            if (brushLineContext.SourceInputHandler is ScreenCameraController){
                tempCamera = (brushLineContext.SourceInputHandler as ScreenCameraController).Camera;
            }


			LinkedListNode<Point> point = null;

			if (Time.frameCount != lastUsedFrame){
				FlushUsedMeshes();
			}


			point = brushLineContext.Points.Last;
            Mesh mesh;
            while (point != null ){                
				if (!point.Value.IsBasePoint){
                    Vector3 pointPosition = point.Value.Position;
                    pointPosition.z = brushLineContext.Canvas.transform.position.z + brushLineContext.Canvas.BrushOffset;
                    mesh = GetMesh(brushLineContext, point.Value);
                    if (point.Value.Status == PointStatus.ReadyToApply){                                                                        
                        Graphics.DrawMesh(mesh, pointPosition, Quaternion.Euler(0,0, point.Value.Rotation), point.Value.Material, 
                            brushLineContext.Canvas.BrushLayerId, brushLineContext.Canvas.CanvasCameraController.Camera);                       
						point.Value.Status = PointStatus.CopiedToCanvas;                                 
                        usedMeshes.Enqueue(mesh);
                    } else if (point.Value.Status == PointStatus.Temporary && tempCamera != null){                           
                        Graphics.DrawMesh(mesh, pointPosition, Quaternion.Euler(0,0, point.Value.Rotation), point.Value.Material, 
                            brushLineContext.Canvas.TempRenderLayerId, tempCamera);
						usedMeshes.Enqueue(mesh);
					}
				}

				point = point.Previous;
			}

            if (brushLineContext.IsLastPointInLine){
                meshPool.Clear();
                usedMeshes.Clear();
            }
			return true;
		}

        Queue<Mesh> meshPool = new Queue<Mesh>();
        Queue<Mesh> usedMeshes = new Queue<Mesh>();
		Mesh GetMesh(BrushContext brushLineContext, Point point){
			float width = point.Size.x * point.Scale;
			float height = point.Size.y * point.Scale;
			width = Mathf.Max(brushLineContext.Brush.MinSize.x, width);
			height = Mathf.Max(brushLineContext.Brush.MinSize.y, height);
			Mesh result;
			if (meshPool.Count > 0){
				result = meshPool.Dequeue();
				MeshUtil.ChangeMeshSize(result, width, height);
			} else {
                result = MeshUtil.CreatePlaneMesh(width, height);
            }
			MeshUtil.ChangeMeshColor(result,  point.PointColor.Color);
            MeshUtil.UpdateMeshUV2(result, width, height, point.Position, point.Rotation, (float)brushLineContext.Canvas.Width, (float)brushLineContext.Canvas.Height, brushLineContext.Canvas.transform.position);
			return result;

		}

		void FlushUsedMeshes(){            
			while (usedMeshes.Count > 0){
				meshPool.Enqueue(usedMeshes.Dequeue());
			}
            lastUsedFrame = Time.frameCount;
		}

		#endregion
	}
}
