using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeInspector;
using PaintCraft.Utils;
using UnityEngine.Rendering;
using PaintCraft.Controllers;



namespace PaintCraft.Tools.Filters{
    /// <summary>
    /// Render swatch using command buffer
    /// </summary>
    /// 
    [NodeMenuItemAttribute ("Renderer/RenderSwatchWithPointMaterial(commandbuffer)")]
    public class RenderSwatchCB : FilterWithNextNode {        
        int lastUsedFrame = -1;

        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody (BrushContext brushLineContext)
        {
            CommandBuffer tempCB = null;
            if (brushLineContext.SourceInputHandler is ScreenCameraController){
                tempCB = (brushLineContext.SourceInputHandler as ScreenCameraController).CommandBuffer;
            }
           


            LinkedListNode<Point> point = null;


            if (Time.frameCount != lastUsedFrame){
                FlushUsedMeshes();
            }
            point = brushLineContext.Points.Last;
            Mesh mesh;
            Matrix4x4 matrix;
            int i = 0;
            int j = 0;
            while (point != null ){                
                if (!point.Value.IsBasePoint){
                    Vector3 pointPosition = point.Value.Position;
                    pointPosition.z = brushLineContext.Canvas.transform.position.z + brushLineContext.Canvas.BrushOffset;
                    matrix = Matrix4x4.TRS(pointPosition, Quaternion.Euler(0,0, point.Value.Rotation), Vector3.one);
                    mesh = GetMesh(brushLineContext, point.Value);
                    if (point.Value.Status == PointStatus.ReadyToApply){                                            
                        brushLineContext.Canvas.CanvasCameraController.CommandBuffer.DrawMesh(mesh, matrix, point.Value.Material);

                        point.Value.Status = PointStatus.CopiedToCanvas;                                 
                        usedMeshes.Enqueue(mesh);
                        i++;
                    } else if (point.Value.Status == PointStatus.Temporary && tempCB != null){                           
                        tempCB.DrawMesh(mesh, matrix, point.Value.Material);
                        usedMeshes.Enqueue(mesh);
                        j++;
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
