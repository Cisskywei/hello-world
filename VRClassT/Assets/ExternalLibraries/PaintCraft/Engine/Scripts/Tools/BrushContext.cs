using PaintCraft.Controllers;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Tools;

namespace PaintCraft.Tools{
	public class BrushContext {
		//todo: maybe remove this prop ?
		public CanvasController  Canvas {get; private set;}
		public Brush			 Brush {
            get{
                return LineConfig.Brush;
            }
        }
		public LineConfig 		 LineConfig{get; private set;}
		public LinkedList<Point> Points {get; private set;}

	    public Material Material { get; private set; }

	    static Queue<LinkedListNode<Point>> PointPool = new Queue<LinkedListNode<Point>>();
        public InputController SourceInputHandler { get; private set; }

        public Vector2 FirstPointPixelPosition;
        public Vector2 FirstPointUVPosition;

        public Vector2 ClippingMaskOffset;

        //this value is true only for first point. usually used in regional line to specify regions.
        public bool IsFirstPointInLine; 

        //this value is true only for latest point.
        public bool IsLastPointInLine; 

        //if we have multitouch this method will show is this point made by final fingerId
        public bool IsFinalAcrossAllTouches{
            get{
                int maxTouchId = int.MinValue;
                foreach (var key in SourceInputHandler.ContextByLineId.Keys)
                {
                    if (key > maxTouchId){
                        maxTouchId = key;
                    }
                }
                return SourceInputHandler.ContextByLineId[maxTouchId] == this;
            }
        }


        public BrushContext(CanvasController canvas, LineConfig lineConfig, InputController sourceInputHandler){
			this.Canvas = canvas;
		    this.SourceInputHandler = sourceInputHandler;
			Points = new LinkedList<Point>();
			LineConfig = lineConfig;
		}

		static public LinkedListNode<Point> GetPointNode(){
			LinkedListNode<Point> result = null;
			if (PointPool.Count > 0){
				result = PointPool.Dequeue();
				result.Value.Reset();
			} else {
				result = new LinkedListNode<Point>(new Point());
			}
			return result;
		}

		static public void ReleasePointNode(LinkedListNode<Point> node){
			node.Value.Reset();
			PointPool.Enqueue(node);
		}

        public bool IsDistanceBetweenLastToPointLessThan(float distance){
            if (Points.Count > 1){
                float vectDistance = Vector2.Distance(Points.Last.Value.Position, Points.Last.Previous.Value.Position);
                return vectDistance < distance;                    
            }
            return false;
        }

        /// <summary>
        /// Removes the last point if distance to previous less than.
        /// </summary>
        /// <returns><c>true</c>,if latest point has been removed <c>false</c> otherwise.</returns>
        /// <param name="distance">Distance.</param>
        public bool RemoveLastPointIfDistanceToPreviousLessThan(float distance){
            if (IsDistanceBetweenLastToPointLessThan(distance)){
                ForceRemoveLastNodePoint();
                return true;
            }    
            return false;
        }


        public void ForceRemoveLastNodePoint(){
			if (Points.Count > 0){
				ReleasePointNode(Points.Last);
				Points.RemoveLast();
			}
		}

		public void ForceRemoveFirstNodePoint(){
			if (Points.Count > 0){
				ReleasePointNode(Points.First);
				Points.RemoveFirst();
			}
		}

		public void Reset(){
			while (Points.Last != null){
				ReleasePointNode(Points.Last);
				Points.RemoveLast();
			}
		}

		public override string ToString ()
		{
			string result= " ---- " + Time.timeSinceLevelLoad.ToString() +" ----- \n";
			LinkedListNode<Point> node = Points.First;

			while(node != null){
				//if (node.Value.IsBasePoint){
					result+= node.Value.ToString()+"\n";
				//}
				node = node.Next;
			}
			return result;
		}



#region Drawing implementation

		float pointTime, previousPointTime, timeDiff;
		Vector2 previouPointPosition;
		int pointId = 0;

//		Vector2 lastInputPosition;
		bool firstPointSetup = false;


	    public void ResetBrushContext()
	    {           
	        Reset();
	        pointId = 0;
	        pointTime = Time.realtimeSinceStartup;
//	        lastInputPosition = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
            firstPointSetup = true;
        }


        public void AddPoint(Vector2 worldPoint){
			if (firstPointSetup){
				firstPointSetup = false;
                FirstPointPixelPosition = worldPoint - (Vector2)Canvas.transform.position;
                FirstPointUVPosition = new Vector2((worldPoint.x -Canvas.transform.position.x)  / Canvas.Width + 0.5f
                    , (worldPoint.y - Canvas.transform.position.y) / Canvas.Height + 0.5f);
				IsFirstPointInLine = true;
            } else {
                IsFirstPointInLine = false;
            }                
			
			var node = GetPointNode();
			node.Value.BasePointId = pointId++;
			node.Value.IsBasePoint = true;
			node.Value.Position = worldPoint;		    
			node.Value.Scale = 1.0f;
			node.Value.Size = Brush.BaseSize;
			node.Value.Time = Time.realtimeSinceStartup - pointTime;
			if (node.Value.BasePointId == 0){
				node.Value.Velocity = 0.0f;
			} else {
				timeDiff = node.Value.Time - previousPointTime;
				node.Value.Velocity = Vector2.Distance(previouPointPosition, worldPoint) / Mathf.Min(0.05f, timeDiff);
			}
			Points.AddLast(node);
			
			previouPointPosition = worldPoint;
			previousPointTime = node.Value.Time;					
		}

	    public void ApplyFilters(bool lastPoint)
	    {
            IsLastPointInLine = lastPoint;
            Brush.StartFilter.Apply(this);
        }
        #endregion
    }
}