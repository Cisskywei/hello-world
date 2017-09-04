using System.Collections.Generic;
using PaintCraft.Utils;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.PropertyBinders{
	[NodeMenuItem("PropertyBinders/BindHueOffsetToVelocity")]
    public class BindHueOffsetToVelocity : FilterWithNextNode {
		public VelocityHueProp Min = new VelocityHueProp(){ Hue = 0.0f, Velocity = 500.0f};
		public VelocityHueProp Max = new VelocityHueProp(){ Hue = 0.1f, Velocity = 1000.0f};


		#region implemented abstract members of FilterWithNextNode
		public override bool FilterBody (BrushContext brushLineContext)
		{

			float newHue;
			float nodeVelocity;
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			float hueDiff = Min.Hue - Max.Hue;
			float velocityDiff = Max.Velocity - Min.Velocity;
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){
				nodeVelocity = node.Value.Velocity;
				if (nodeVelocity <= Min.Velocity){
					newHue = Min.Hue;
				} else if (nodeVelocity >= Max.Velocity){
					newHue = Max.Hue;
				} else {
					newHue = Min.Hue -  hueDiff * (nodeVelocity - Min.Velocity)/ velocityDiff;
				}

				node.Value.PointColor.H  = MathUtil.LoopValue( newHue + node.Value.PointColor.H, 0.0f, 1.0f);
				node = node.Previous;
			}
			return true;
		}
		#endregion
			


		[System.Serializable]
		public class VelocityHueProp{
			public float Hue;
			[Tooltip("Velocity (Pixels per Seconds)")]
			public float Velocity;
		}
	}
}