﻿using System.Collections.Generic;
using UnityEngine;
using NodeInspector;

namespace PaintCraft.Tools.Filters.Randomizers{
    [NodeMenuItem("Randomizers/JitterScale")]
    public class JitterScale : FilterWithNextNode {
		public float ScaleMax = 1.0f;
		public float ScaleMin = 0.1f;
		
		#region implemented abstract members of FilterWithNextNode
		
		public override bool FilterBody (BrushContext brushLineContext)
		{			
			LinkedListNode<Point> node = brushLineContext.Points.Last;			
			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas) {
				node.Value.Scale *= Random.Range(ScaleMin, ScaleMax) ;
				node = node.Previous;
			}
			return true;
		}
		
		#endregion
		
	}
}