using System;
using System.Collections.Generic;
using PaintCraft.Tools.Filter;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Interpolators{
	/// <summary>
	/// Spline interpolation. Use Catmul-Rom interpolation, until we have 4 points, use linear temp interpolation
	/// </summary>
	[NodeMenuItem("Interpolators/SplineInterpolator")]
    public class SplineInterpolator : FilterWithNextNode {
		public SpacingProperty Spacing;
		[NonSerialized]
		private LinearInterpolator linearInterpolator;
		[NonSerialized]
		private IgnorePositionsCloseThanSpacing ignorePositionsCloseThanSpacing;

		[NonSerialized]
		bool initialized = false;
		void InitIfRequired(){
			if (initialized) {
				return;
			}
            linearInterpolator = ScriptableObject.CreateInstance<LinearInterpolator> ();
			linearInterpolator.Spacing = Spacing;
            ignorePositionsCloseThanSpacing = ScriptableObject.CreateInstance< IgnorePositionsCloseThanSpacing> ();
			ignorePositionsCloseThanSpacing.Spacing = Spacing;
			initialized = true;
		}


		LinkedListNode<Point> P1,P2,P3,P4;
		int numberOfBasePoints;
		
		float diffX, diffY, maxD, step;
		//Point p1,p2,p3,p4;
		//Point previousBasePoint;

		Vector2 diff;
		#region implemented abstract members of FilterWithNextNode
		
        public override bool FilterBody (BrushContext brushLineContext)
		{
			InitIfRequired ();
            if (!ignorePositionsCloseThanSpacing.FilterBody (brushLineContext)) {                
                return false;
			}
			
			//1st. we need to take base points.
			SetupBrushPointsAndRemoveFromContext(brushLineContext);
			switch (numberOfBasePoints){
			case 0:
				Debug.LogWarning("No input base points here");
				return false; 
			case 1:
                HandleOnePoint(brushLineContext, brushLineContext.IsLastPointInLine);
				break;
			case 2:
                HandleTwoPoints(brushLineContext, brushLineContext.IsLastPointInLine);
				break;
			case 3:
                HandleThreePoints(brushLineContext, brushLineContext.IsLastPointInLine );
				break;
			case 4:
                HandleFourPoints(brushLineContext, brushLineContext.IsLastPointInLine);
				break;
			}

            return linearInterpolator.FilterBody(brushLineContext); //it must return false everytime
		}

        protected override void FilterFinalizer(BrushContext brushLineContext)
        {
            if (brushLineContext.Points.Count == 0){
                // array was 0 or removed all points
                return;
            }                        

            int foundNodes = 0;
            Stack<LinkedListNode<Point>> basePoints = new Stack<LinkedListNode<Point>>();
            LinkedListNode<Point> point = null;
            LinkedListNode<Point> previousPoint = null;
            while (brushLineContext.Points.Count > 0){
                point = brushLineContext.Points.Last;
                if (point.Value.IsBasePoint || 
                    (!point.Value.IsBasePoint && point.Value.Status == PointStatus.CopiedToCanvas 
                        && (previousPoint != null && previousPoint.Value.Position != point.Value.Position))) {


                    basePoints.Push(point);
                    previousPoint = point;
                    brushLineContext.Points.RemoveLast();
                    foundNodes ++;
                    if (foundNodes == 3){
                        break;
                    }
                } else {
                    brushLineContext.ForceRemoveLastNodePoint();
                }
            }




            while (brushLineContext.Points.Count > 0){
                brushLineContext.ForceRemoveLastNodePoint();
            }

            //add basepoints from stack back

            while (basePoints.Count > 0 ){
                brushLineContext.Points.AddLast (basePoints.Pop());
            }
        }

		void HandleOnePoint (BrushContext brushLineContext, bool drawEnded)
		{
			AddFirstNodePoint(P4, brushLineContext, !drawEnded);
		}

		void AddFirstNodePoint(LinkedListNode<Point> point, BrushContext brushLineContext, bool makeTempPoint){
			brushLineContext.Points.AddLast(point);
			LinkedListNode<Point> renderPoint = BrushContext.GetPointNode();
			renderPoint.Value.CopyFrom (point.Value);
			renderPoint.Value.IsBasePoint = false;
			renderPoint.Value.Status = makeTempPoint ? PointStatus.Temporary : PointStatus.ReadyToApply;
			brushLineContext.Points.AddLast(renderPoint);
		}


		void HandleTwoPoints (BrushContext brushLineContext, bool drawEnded)
		{
			AddFirstNodePoint(P3, brushLineContext, !drawEnded); 
			diff = P4.Value.Position - P3.Value.Position;
			maxD = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) * 3.0f;
			step = 1.0f / maxD;
			for (float i = step; i < 1.0f; i+=step) {
				AddInterpolatedPoint(P3.Value, P3.Value, P4.Value, P4.Value, i, brushLineContext, !drawEnded);
			}
			brushLineContext.Points.AddLast(P4);
		}

		void HandleThreePoints (BrushContext brushLineContext, bool drawEnded)
		{
			AddFirstNodePoint(P2, brushLineContext, false); 
			// P2->P3 as readyToApply (it's begining of the line
			diff = P3.Value.Position - P2.Value.Position;
			maxD = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) * 3.0f;
			step = 1.0f / maxD;
			for (float i = step; i < 1.0f; i+=step) {
				AddInterpolatedPoint(P2.Value, P2.Value, P3.Value, P4.Value, i, brushLineContext, false);
			}
			brushLineContext.Points.AddLast(P3);

			DrawP3P4TempOrReadysegment(brushLineContext, drawEnded);
		}

		void HandleFourPoints (BrushContext brushLineContext, bool drawEnded)
		{

			//P1-P2 should be already there (from previous iteration)
			brushLineContext.Points.AddLast(P1);

			//P2->P3 readyToApply here it's a middle segment
			AddFirstNodePoint(P2, brushLineContext, false); 
			diff = P3.Value.Position - P2.Value.Position;
			maxD = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) * 3.0f;
			step = 1.0f / maxD;
			for (float i = step; i < 1.0f; i+=step) {
				AddInterpolatedPoint(P1.Value, P2.Value, P3.Value, P4.Value, i, brushLineContext, false);
			}
			brushLineContext.Points.AddLast(P3);

			DrawP3P4TempOrReadysegment(brushLineContext, drawEnded);
		}

		void DrawP3P4TempOrReadysegment(BrushContext brushLineContext, bool drawEnded){
			//P3->P4 temp if !drawEnd
			diff = P4.Value.Position - P3.Value.Position;
			maxD = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) * 3.0f;
			step = 1.0f / maxD;
			for (float i = step; i < 1.0f; i+=step) {
				AddInterpolatedPoint(P2.Value, P3.Value, P4.Value, P4.Value, i, brushLineContext, !drawEnded);
			}
			brushLineContext.Points.AddLast(P4);
		}


		float t0,t1,t2,t3;
		void AddInterpolatedPoint(Point _p0,Point _p1, Point _p2, Point _p3, float t, BrushContext brushLineContext, bool temp ){
			t0 = ((-t + 2.0f) * t - 1.0f) * t * 0.5f;
			t1 = (((3.0f * t - 5.0f) * t) * t + 2.0f) * 0.5f;
			t2 = ((-3.0f * t + 4.0f) * t + 1.0f) * t * 0.5f;
			t3 = ((t - 1f) * t * t) * 0.5f;
			LinkedListNode<Point> point = BrushContext.GetPointNode();
			point.Value.Status = temp ? PointStatus.Temporary : PointStatus.ReadyToApply;
			point.Value.Position.x = t0 * _p0.Position.x + t1 * _p1.Position.x + t2 * _p2.Position.x + t3 * _p3.Position.x;
            point.Value.Position.y = t0 * _p0.Position.y + t1 * _p1.Position.y + t2 * _p2.Position.y + t3 * _p3.Position.y;


            point.Value.Time     = t0 * _p0.Time     + t1 * _p1.Time     + t2 * _p2.Time     + t3 * _p3.Time;		
			point.Value.Velocity = t0 * _p0.Velocity + t1 * _p1.Velocity + t2 * _p2.Velocity + t3 * _p3.Velocity;		
			point.Value.Scale    = t0 * _p0.Scale    + t1 * _p1.Scale    + t2 * _p2.Scale    + t3 * _p3.Scale;	
			point.Value.Size.x   = t0 * _p0.Size.x   + t1 * _p1.Size.x   + t2 * _p2.Size.x   + t3 * _p3.Size.x;
            point.Value.Size.y   = t0 * _p0.Size.y   + t1 * _p1.Size.y   + t2 * _p2.Size.y   + t3 * _p3.Size.y;
            point.Value.Rotation = t0 * _p0.Rotation + t1 * _p1.Rotation + t2 * _p2.Rotation + t3 *_p3.Rotation;
            point.Value.Material = _p0.Material;
			brushLineContext.Points.AddLast(point);
		}


		void SetupBrushPointsAndRemoveFromContext(BrushContext brushLineContext){		
			int pointId = 4;
			P1 = P2 = P3 = P4 = null;
			LinkedListNode<Point> node;
            while ( brushLineContext.Points.Count > 0){
                node = brushLineContext.Points.Last;
				if (pointId > 0){
					switch(pointId--){
					case 4: P4 = node; break;
					case 3: P3 = node; break;
					case 2: P2 = node; break;
					case 1: P1 = node; break;
					}
                    brushLineContext.Points.RemoveLast();
				} else {
                    brushLineContext.ForceRemoveLastNodePoint();
				}
			}
			numberOfBasePoints = 4 - pointId;
            P4.Value.Status = brushLineContext.IsLastPointInLine ? PointStatus.ReadyToApply : PointStatus.Temporary;
			if (numberOfBasePoints > 1){
				P3.Value.Status = PointStatus.ReadyToApply;			
			}

		}
		
		#endregion
	}
	
}