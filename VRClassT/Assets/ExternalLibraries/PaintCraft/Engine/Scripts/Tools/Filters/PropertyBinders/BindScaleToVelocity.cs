using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders{
	[NodeMenuItem("PropertyBinders/BindScaleToVelocity")]
    public class BindScaleToVelocity : FilterWithNextNode {
		public VelocityScaleProp Min = new VelocityScaleProp(){Scale = 1.0f, Velocity = 500.0f};
		public VelocityScaleProp Max = new VelocityScaleProp(){Scale = 0.1f, Velocity = 10000.0f };

		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{
			float nodeVelocity;
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			float scaleDiff = Min.Scale - Max.Scale;
			float velocityDiff = Max.Velocity - Min.Velocity;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				nodeVelocity = node.Value.Velocity;
				if (nodeVelocity <= Min.Velocity){
					node.Value.Scale = Min.Scale;
				} else if (nodeVelocity >= Max.Velocity){
					node.Value.Scale = Max.Scale;
				} else {
					node.Value.Scale = Min.Scale -  scaleDiff * (nodeVelocity - Min.Velocity)/ velocityDiff;
				}
			    node.Value.Scale *= brushLineContext.LineConfig.Scale;
				node = node.Previous;
			}
			return true;
		}
		#endregion

	}

	[System.Serializable]
	public class VelocityScaleProp{
		[Tooltip("scale of the swatch (like 0.5 = 50% of original size")]
		public float Scale;
		[Tooltip("Velocity (Pixels per Seconds)")]
		public float Velocity;
	}
}