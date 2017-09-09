using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Interpolators{
	[NodeMenuItem("Interpolators/LinearInterpolator")]
    public class LinearInterpolator : FilterWithNextNode {
		public SpacingProperty Spacing;
	
		List<LinkedListNode<Point>> interpolatedPoints = new List<LinkedListNode<Point>>();

		#region implemented abstract members of FilterWithNextNode

		public override bool FilterBody (BrushContext brushLineContext)
		{
			float spacing = Spacing.GetSpacingValue(brushLineContext);

			if (spacing  <= 0.0f){
				Debug.LogError("Spacing must be greater than 0 (setup on canvas)");
				return false;
			}

			if (brushLineContext.Points.Count < 1){
				return false;
			}


			if (brushLineContext.Points.Count == 1){
				//handle single input point
				LinkedListNode<Point> clone = GetNormalCloneFromBasePoint(brushLineContext.Points.First, brushLineContext);
                clone.Value.Status = brushLineContext.IsLastPointInLine ? PointStatus.ReadyToApply : PointStatus.Temporary;
				brushLineContext.Points.AddLast(clone);
				return true;
			}
		
			interpolatedPoints.Clear();

			LinkedListNode<Point> lastCopiedToCanvasPoint = FindLastApliedNode(brushLineContext);
			Point lastInterpolatedPoint;
			LinkedListNode<Point> segmentStartPoint, segmentEndPoint;
			if (lastCopiedToCanvasPoint == null){
				//this must happens only on begining of the line. so let's add new point
				// and need to handle input from spline interpolation here. so copy base point if next point base as well
				LinkedListNode<Point> firstBasePoint = FindFirstBasePoint(brushLineContext);
				if (firstBasePoint == null){
					Debug.LogWarning("can't find first base point or any points applied to canvas");
					return false;
				}

				if (firstBasePoint.Next != null 
				    && !firstBasePoint.Next.Value.IsBasePoint 
				    && firstBasePoint.Value.Position == firstBasePoint.Next.Value.Position){
					//We come here from e.g. spline interpolator. who already cloned first base point
					if (brushLineContext.Points.Count == 2){
						return true;// it's just one clone of base point here
					}
					interpolatedPoints.Add(firstBasePoint);
					interpolatedPoints.Add(GetNormalCloneFromBasePoint(firstBasePoint.Next, brushLineContext)); //clone because late it would be wiped out in cycle
					lastInterpolatedPoint = firstBasePoint.Value;
					segmentStartPoint = firstBasePoint.Next;
				} else {

					LinkedListNode<Point> basePointNormalClone = GetNormalCloneFromBasePoint(firstBasePoint, brushLineContext);
					interpolatedPoints.Add(firstBasePoint);
					interpolatedPoints.Add(basePointNormalClone);
					lastInterpolatedPoint = firstBasePoint.Value;


					// this stub required to find next point in late while iteration
					// need to create one more clone, because later it would be wipedout
					brushLineContext.Points.RemoveFirst();
					segmentStartPoint     = GetNormalCloneFromBasePoint(basePointNormalClone, brushLineContext);
					brushLineContext.Points.AddFirst(segmentStartPoint);
				}
			} else {
				// interpolate depends on copied point;
				segmentStartPoint = lastCopiedToCanvasPoint;
				lastInterpolatedPoint = lastCopiedToCanvasPoint.Value;
			}



			Vector2 segmentVector;
			float segmentMagnitude;
			float distanceAtStart;
			float segmentSpacing;
			float segmentSpacingStep;
			float timeDiff;
			float velocityDiff;
			float scaleDiff;
			float rotationDiff;
			Vector2 sizeDiff;
			LinkedListNode<Point> interpolatedNewNode;
			while (segmentStartPoint !=  null && segmentStartPoint.Next != null){

				if (segmentStartPoint.Value.IsBasePoint){
					interpolatedPoints.Add(segmentStartPoint);
				}
				segmentEndPoint = segmentStartPoint.Next;
				float distanceAtEnd = Vector2.Distance(lastInterpolatedPoint.Position, segmentEndPoint.Value.Position);
				if (distanceAtEnd > spacing){
					//find first position of interpolated point
					segmentVector = segmentEndPoint.Value.Position - segmentStartPoint.Value.Position;
					timeDiff      = segmentEndPoint.Value.Time     - segmentStartPoint.Value.Time;
					velocityDiff  = segmentEndPoint.Value.Velocity - segmentStartPoint.Value.Velocity;
					scaleDiff     = segmentEndPoint.Value.Scale    - segmentStartPoint.Value.Scale;
					sizeDiff	  = segmentEndPoint.Value.Size     - segmentStartPoint.Value.Size;
					rotationDiff  = segmentEndPoint.Value.Rotation - segmentStartPoint.Value.Rotation;

					segmentMagnitude = segmentVector.magnitude; 
					distanceAtStart = Vector2.Distance (lastInterpolatedPoint.Position, segmentStartPoint.Value.Position);
					segmentSpacing = (1.0f - (distanceAtStart / spacing)) * spacing / segmentMagnitude;
					segmentSpacingStep = spacing / segmentMagnitude;

					while (segmentSpacing < 1.0f){
						interpolatedNewNode = GetNormalCloneFromBasePoint(segmentEndPoint, brushLineContext);
						interpolatedNewNode.Value.Position = segmentStartPoint.Value.Position + segmentSpacing * segmentVector;
						interpolatedNewNode.Value.Time     = segmentStartPoint.Value.Time     + segmentSpacing * timeDiff;
						interpolatedNewNode.Value.Velocity = segmentStartPoint.Value.Velocity + segmentSpacing * velocityDiff;
						interpolatedNewNode.Value.Scale    = segmentStartPoint.Value.Scale    + segmentSpacing * scaleDiff;
						interpolatedNewNode.Value.Size     = segmentStartPoint.Value.Size     + segmentSpacing * sizeDiff;
						interpolatedNewNode.Value.Rotation = segmentStartPoint.Value.Rotation + segmentSpacing * rotationDiff;

						segmentSpacing += segmentSpacingStep;
						interpolatedPoints.Add(interpolatedNewNode);
						lastInterpolatedPoint = interpolatedNewNode.Value;
					}

				}
				segmentStartPoint = segmentEndPoint;
			}

			// ad final point if it's base point
			if (segmentStartPoint.Value.IsBasePoint){
				interpolatedPoints.Add(segmentStartPoint);
			}


			//clean all points until face first
			while (brushLineContext.Points.Count > 0 && brushLineContext.Points.Last != lastCopiedToCanvasPoint){
				if (brushLineContext.Points.Last.Value.IsBasePoint){
					brushLineContext.Points.RemoveLast();
				} else {
					brushLineContext.ForceRemoveLastNodePoint();
				}
			}


			// add interpolatedPoints
			for (int i = 0; i < interpolatedPoints.Count; i++) {
				brushLineContext.Points.AddLast(interpolatedPoints[i]);
			}

			return true;
		}


		LinkedListNode<Point> GetNormalCloneFromBasePoint(LinkedListNode<Point> basePoint, BrushContext brushLineContext){
			LinkedListNode<Point> baseClone = BrushContext.GetPointNode();
			baseClone.Value.CopyFrom(basePoint.Value);
			baseClone.Value.IsBasePoint = false;
			return baseClone;
		}


		#endregion

		LinkedListNode<Point> FindLastApliedNode(BrushContext brushLineContext){
			LinkedListNode<Point> result = brushLineContext.Points.Last;
			while (result != null) {
				if (result.Value.Status == PointStatus.CopiedToCanvas){
					return result;
				}
				result = result.Previous;
			}
			return null;
		}

		LinkedListNode<Point> FindFirstBasePoint (BrushContext brushLineContext)
		{
			LinkedListNode<Point> result = brushLineContext.Points.First;
			while (result != null) {
				if (result.Value.IsBasePoint){
					return result;
				}
				result = result.Next;
			}
			return null;
		}


        protected override void FilterFinalizer(BrushContext brushLineContext)
        {
            Stack<LinkedListNode<Point>> storedPoints = new Stack<LinkedListNode<Point>>();
            LinkedListNode<Point> node;
            while (brushLineContext.Points.Count > 0){
                node = brushLineContext.Points.Last;
                if (node.Value.IsBasePoint){
                    storedPoints.Push(node);
                    brushLineContext.Points.RemoveLast();
                } else {
                    if (node.Value.Status == PointStatus.CopiedToCanvas){
                        storedPoints.Push(node);
                        brushLineContext.Points.RemoveLast();
                        break;
                    } else {
                        brushLineContext.ForceRemoveLastNodePoint();
                    }
                }
            }

            while (brushLineContext.Points.Count > 0){
                brushLineContext.ForceRemoveLastNodePoint();
            }

            while (storedPoints.Count > 0){
                brushLineContext.Points.AddLast(storedPoints.Pop());
            }
        }
	}
}