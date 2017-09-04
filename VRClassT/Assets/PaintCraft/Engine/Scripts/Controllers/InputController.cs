using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using PaintCraft.Tools;
using PaintCraft.Canvas;
using UnityEngine.EventSystems;


namespace PaintCraft.Controllers{   
    public abstract class InputController : MonoBehaviour {
         
        public CanvasController Canvas;

        Dictionary<int, BrushContext> contextByLineId  = new Dictionary<int, BrushContext>();
        public Dictionary<int, BrushContext> ContextByLineId
        {
            get { return contextByLineId; } 
        }

        /// <summary>
        /// Donts the allow interaction. This check works only if you use line methods with screen coordinates
        /// </summary>
        /// <returns><c>true</c>, if allow interaction was donted, <c>false</c> otherwise.</returns>
        /// <param name="worldPosition">Input position.</param>
        public abstract bool DontAllowInteraction(Vector2 worldPosition);            

        Vector2 previousPosition;

        /// <summary>
        /// Begins the line.
        /// </summary>
        /// <param name="lineConfig">Line config.</param>
        /// <param name="lineId">Line identifier.</param>
        /// <param name="inputPosition">Input position (in world space).</param>
        /// <param name="interactionAllowCheck">Set it to true if this input called from camera and then override DontAllowInteraction method to prevent event handling if cursor e.g. on top of UI element</param>
        public void BeginLine (LineConfig lineConfig, int lineId, Vector2 inputPosition, bool interactionAllowCheck = false)
        {
            if (EventSystem.current == null){
                Debug.LogError("you have to add event system to the scene. e.g. from Unity UI");
                return;
            } 

            if (finalSnapshotInProgress){
                return;
            }

            if (interactionAllowCheck && DontAllowInteraction(inputPosition)){
                return; // handle on different camera or ignore
            }

            if (ContextByLineId.ContainsKey(lineId)){
                EndLine(lineId, inputPosition);
            }

            AnalyticsWrapper.CustomEvent("TouchBegan", new Dictionary<string, object>
            {
                { "HandlerName", gameObject.name},
                { "ToolName", lineConfig.Brush.name },
                { "TouchId", lineId}/*,
                { "TotalTouch", e.Touches.Count}*/
            });


            BrushContext bc =  new BrushContext(Canvas, lineConfig, this);              
            if (ContextByLineId.Count == 0){
                StoreStateBeforeSnapshot();
            }
            ContextByLineId.Add(lineId, bc);
            bc.ResetBrushContext();

            bc.AddPoint(inputPosition);
            bc.ApplyFilters(false);             
            previousPosition = inputPosition;
            lineTerminated = false;
        }


        bool lineTerminated = false;
        public void ContinueLine (int lineId, Vector2 inputPosition)
        {     
            if (!contextByLineId.ContainsKey(lineId) || finalSnapshotInProgress){
                return; //initiated on different camera
            }   
            BrushContext bc = contextByLineId[lineId];
            if (!Canvas.isCoordWithinRect(inputPosition)){
                
                if (lineTerminated){
                    Debug.Log("ignore");
                    return;
                } else {
                    Debug.Log("terminate line");
                    EndLine(lineId, previousPosition);
                    lineTerminated = true;
                }
            } else {                
                if (lineTerminated){
                    Debug.Log("resume line");
                    BeginLine(bc.LineConfig, lineId, inputPosition, false);
                    return;
                }
            }

            bc.AddPoint(inputPosition);
            bc.ApplyFilters(false);
            previousPosition = inputPosition;
        }


       
        public void EndLine (int lineId, Vector2 inputPosition)
        {           
            if (!contextByLineId.ContainsKey(lineId) || finalSnapshotInProgress){
                return; //initiated on different camera
            }   
            BrushContext bc = contextByLineId[lineId];

            AnalyticsWrapper.CustomEvent("TouchEnbded", new Dictionary<string, object>
            {
                { "HandlerName", gameObject.name},
                { "ToolName", bc.LineConfig.Brush.name },
                { "TouchId", lineId}/*,
                { "TotalTouch", e.Touches.Count}*/
            });

            bc.AddPoint(inputPosition);

            bc.ApplyFilters(true);

            contextByLineId.Remove(lineId);
            if (ContextByLineId.Count == 0){
                StartCoroutine(MakeSnapshot());  
            }
        }




        SnapshotCommand snapCommand;
        public void StoreStateBeforeSnapshot(){            
            snapCommand = new SnapshotCommand(Canvas.UndoManager);
            snapCommand.BeforeCommand();
        }

        bool finalSnapshotInProgress = false;
        IEnumerator MakeSnapshot(){
            yield return null;
            finalSnapshotInProgress = true;
            snapCommand.AfterCommand();
            Canvas.UndoManager.AddNewCommandToHistory(snapCommand);
            Canvas.SaveChangesToDisk();
            finalSnapshotInProgress = false;
        } 
    }
}
