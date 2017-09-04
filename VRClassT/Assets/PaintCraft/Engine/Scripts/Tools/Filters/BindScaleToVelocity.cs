using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools.Filters{
	/// <summary>
	/// Velocity to scale relation.
	/// It take velocity and and change scale. Scale combined with canvas brush scale 
	/// </summary>
	[NodeMenuItem("PropertyBinders/BindScaleToDistanceToPreviousPoint")]
    public class BindScaleToDistanceToPreviousPoint : FilterWithNextNode {
		public float TimePerPixel = 0.0005f;
		public float TimeMult = 100.0f;
		public float ScaleMin = 0.1f;
		public float ScaleMax = 1.0f;
		const float HalfPi = Mathf.PI/ 2.0f;
		#region implemented abstract members of FilterWithNextNode

		public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> point = brushLineContext.Points.First;
			float diffTime, scalePerPixel, distance, newScale;
			float len;
			while (point.Next != null){
				if (point.Next.Value.Status != PointStatus.CopiedToCanvas){
					diffTime = point.Next.Value.Time - point.Value.Time;
					distance = Vector2.Distance(point.Next.Value.Position, point.Value.Position);					
					scalePerPixel  = Mathf.Atan((diffTime / distance - TimePerPixel) * TimeMult)/ Mathf.PI;

					newScale = point.Value.Scale;
					if (distance >=1.0f){
						len = 1.0f;
						for (; len <= distance; len+=1.0f) {
							newScale+= scalePerPixel;
						}
						newScale += scalePerPixel * (distance - len);
					} else {
						newScale += scalePerPixel * distance;
					}

					point.Next.Value.Scale = Mathf.Clamp( newScale , ScaleMin, ScaleMax);
				}

				point = point.Next;
			}
			return true;
		}

		#endregion


	}
}