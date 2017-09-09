using System.Collections;
using System.Collections.Generic;
using NodeInspector;
using UnityEngine;


namespace PaintCraft.Tools.Filters.MaterialFilter
{
	[NodeMenuItemAttribute("Conditions/IsDefaultColorOpaque")]
	public class IsDefaultColorOpaque : IFilter
	{
		[OneWay]
		public IFilter Yes;
		
		[OneWay]
		public IFilter No;


		public override void Apply(BrushContext brushLineContext)
		{
			if (brushLineContext.Canvas.DefaultBGColor.a == 1.0f)
			{
				Yes.Apply(brushLineContext);
			}
			else
			{
				No.Apply(brushLineContext);
			}
		}
	}
}