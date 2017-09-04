using UnityEngine;
using PaintCraft.Controllers;
using NodeInspector;
using System.Collections.Generic;

namespace PaintCraft.Tools.Filters
{
    [NodeMenuItem("Camera/CameraControllFilter")]
    public class CameraControllFilter : FilterWithNextNode
    {   
        struct WSPosition {
            public Vector2 worldPosition;
            public Vector2 screenPosition;
        }
            

        public float ScreenMagnitudeToCameraStepRatio = 0.5f;
        Vector2 startPoint;
        Vector2 startCamSize;

        Dictionary<int, WSPosition> startPositions = new Dictionary<int, WSPosition>();

        List<int> GetCommonPointsID(Dictionary<int, WSPosition> collection1, Dictionary<int, WSPosition> colleciton2){
            List<int> result = new List<int>();
            foreach(KeyValuePair<int, WSPosition> kvp in collection1){
                if (colleciton2.ContainsKey(kvp.Key)){
                    result.Add(kvp.Key);
                }
            }
            return result;
        }
                   

        public override bool FilterBody(BrushContext brushLineContext)
        {
            if (!brushLineContext.IsFinalAcrossAllTouches){                
                return false;
            }

            if (!(brushLineContext.SourceInputHandler is ScreenCameraController)){
                return false;
            }
            ScreenCameraController screenCamera = brushLineContext.SourceInputHandler as ScreenCameraController;
            Camera camera = screenCamera.Camera;

            Dictionary<int, WSPosition> currentPositions = new Dictionary<int, WSPosition>();
            foreach (KeyValuePair<int, BrushContext> kvp in brushLineContext.SourceInputHandler.ContextByLineId){
                Point lastPoint = kvp.Value.Points.Last.Value;
                currentPositions.Add(kvp.Key, new WSPosition(){
                    worldPosition = lastPoint.Position, 
                    screenPosition = camera.WorldToScreenPoint(lastPoint.Position)
                });
            }





            if (startPositions.Count != currentPositions.Count){
                startPositions = currentPositions;
                screenCamera.CameraSize.ForceDisableMove();

                startCamSize = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            } else {
                // fix scale
                if (currentPositions.Count > 1){
                    
                    Vector2 currentScreenDiff =  (currentPositions[1].screenPosition - currentPositions[0].screenPosition);
                    currentScreenDiff.x = Mathf.Abs(currentScreenDiff.x);
                    currentScreenDiff.y = Mathf.Abs(currentScreenDiff.y);

                    Vector2 originalScreenDiff = (startPositions[1].screenPosition - startPositions[0].screenPosition);
                    originalScreenDiff.x = Mathf.Abs(originalScreenDiff.x);
                    originalScreenDiff.y = Mathf.Abs(originalScreenDiff.y);

                    Vector2 screenMoveDiff = currentScreenDiff - originalScreenDiff;
                    if (screenMoveDiff.magnitude > 0){

                        float newSize;
                        if (Mathf.Abs(screenMoveDiff.y) > Mathf.Abs(screenMoveDiff.x)){
                            float offset = screenMoveDiff.y;
                            newSize = startCamSize.y - offset;
                        } else {
                            float offset = screenMoveDiff.x;
                            newSize = (startCamSize .x- offset) / camera.aspect;
                        }    
                        screenCamera.CameraSize.SetCameraNewOrthoSize(newSize);
                    }
                }
                    
                // fix position
                List<int> commonPoints = GetCommonPointsID(currentPositions, startPositions);
                Vector2 avergeStartWorldPosition = Vector2.zero;
                Vector2 averageCurrentScreenPosition = Vector2.zero;
                for (int i = 0; i < commonPoints.Count; i++)
                {
                    int key = commonPoints[i];
                    avergeStartWorldPosition  += startPositions[key].worldPosition;
                    averageCurrentScreenPosition += currentPositions[key].screenPosition;
                }                    
                avergeStartWorldPosition   /= commonPoints.Count;
                averageCurrentScreenPosition  /= commonPoints.Count;
                SetScreenPositionToWorldPosition(avergeStartWorldPosition, averageCurrentScreenPosition, screenCamera);
            }

            if (brushLineContext.IsLastPointInLine)
            {
                startPositions.Clear();
                screenCamera.CameraSize.CheckBounds();
            }
            return true;
        }

        void SetScreenPositionToWorldPosition(Vector2 worldPosition, Vector2 screenPosition, ScreenCameraController inputHandler)
        {
            Vector2 currentGlobalPosition = inputHandler.Camera.ScreenToWorldPoint(screenPosition);


            Vector2 diff = worldPosition  - currentGlobalPosition;
            inputHandler.transform.Translate(diff);
        }

    }
}