using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Controllers;
using PaintCraft.Tools;
using UnityEngine.Assertions;


namespace PaintCraft.Pro.Controllers{ 
    [RequireComponent(typeof(LineConfig))]
    public class Drawing3DController : InputController {
        public Camera       SelfCamera;
        public int          LineUniqueId = 99442211; 
        LineConfig   LineConfig;

        void Start(){
            LineConfig = GetComponent<LineConfig>();
            Assert.IsNotNull(LineConfig, "You must add LineConfig to the same object which have Drawing3DController");
        }


        #region implemented abstract members of InputController

        public override bool DontAllowInteraction(Vector2 worldPosition)
        {
            return false;
        }

        #endregion

        PaintcraftCanvas3DPlane paintcraftPlane;       
        Vector2 paintcraftCoordinates;
        bool drawingStarted = false;
        void Update(){
            
            if (Input.GetMouseButtonDown(0)){
                if (DoRaycast(out paintcraftPlane, out paintcraftCoordinates)){
                    drawingStarted = true;
                    Canvas = paintcraftPlane.PaintcraftCanvas;
                    BeginLine(LineConfig, LineUniqueId, paintcraftCoordinates);
                }
            } else if ( drawingStarted){
                Ray ray = SelfCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                Vector2 uv = paintcraftPlane.GetUVUsingPlane(ray);
                Debug.Log(uv + " uv");
                paintcraftCoordinates = GetGlobalPositionFromUV(paintcraftPlane, uv);
                Debug.Log(paintcraftCoordinates + " paintcraftCoordinates");
                if ( Input.GetMouseButton(0)){
                    ContinueLine(LineUniqueId, paintcraftCoordinates);                   
                } else if (Input.GetMouseButtonUp(0)){
                    EndLine(LineUniqueId, paintcraftCoordinates);    
                    drawingStarted = false;
                }              
            } 
        }


        bool DoRaycast(out PaintcraftCanvas3DPlane paintcraftPlane, out Vector2 paintcraftCoords){
            paintcraftPlane = null;
            paintcraftCoords = new Vector2();
            Ray ray = SelfCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)){
                paintcraftPlane = hitInfo.collider.gameObject.GetComponent<PaintcraftCanvas3DPlane>();
                if (paintcraftPlane == null){
                    return false;
                }               

                Vector2 uv = paintcraftPlane.GetUVUsingPlane(ray);
                paintcraftCoords = GetGlobalPositionFromUV(paintcraftPlane, uv);
                return true;
            }
            return false;
        }


        Vector2 GetGlobalPositionFromUV(PaintcraftCanvas3DPlane paintcraftPlane, Vector2 uv){
            Vector3 result = paintcraftPlane.PaintcraftCanvas.transform.position;
            result.x -=paintcraftPlane.PaintcraftCanvas.Width / 2;
            result.y -=paintcraftPlane.PaintcraftCanvas.Height / 2;
            result.x += uv.x * paintcraftPlane.PaintcraftCanvas.Width;
            result.y += uv.y * paintcraftPlane.PaintcraftCanvas.Height;
            return result;
        }
    }
}
