using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeInspector;
using System;
using PaintCraft.Utils;

namespace PaintCraft.Tools.Filters{
    [System.Flags]
    public enum PointState {
        FirstPoint      = 1<<0,
        MiddlePoints    = 1<<1,
        LastPoint       = 1<<2
    }


    [NodeMenuItemAttribute("ChangePoint/SetPointStatus")]
    public class SetPointStatus : FilterWithNextNode {
        public PointStatus PointStatus = PointStatus.ReadyToApply;

        [EnumFlags]
        public PointState PointState = PointState.FirstPoint | PointState.MiddlePoints | PointState.LastPoint;


        #region implemented abstract members of FilterWithNextNode
        public override bool FilterBody (BrushContext brushLineContext)
        {
            bool handleFirst = (brushLineContext.IsFirstPointInLine && ((PointState & PointState.FirstPoint) == PointState.FirstPoint));
            bool handleLast = (brushLineContext.IsLastPointInLine && ((PointState & PointState.LastPoint) == PointState.LastPoint));
            bool handleMiddle = (brushLineContext.IsFirstPointInLine == false && brushLineContext.IsLastPointInLine == false 
                && ((PointState & PointState.MiddlePoints) == PointState.MiddlePoints));
            
            if (handleFirst || handleLast || handleMiddle){
                LinkedListNode<Point> node = brushLineContext.Points.Last;
                while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){                
                    node.Value.Status = PointStatus;
                    node = node.Previous;
                }                
            }
            return true;
        }
        #endregion
    }
}