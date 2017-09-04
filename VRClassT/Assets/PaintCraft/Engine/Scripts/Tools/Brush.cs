using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

namespace PaintCraft.Tools{
	[CreateAssetMenu(menuName="PaintCraft/Brush")]
	public class Brush : ScriptableObject
	{	    
        [Graph("StartFilter")]
	    public List<IFilter> Filters;
        public IFilter StartFilter;
		public Vector2 MinSize = Vector2.one;		
		public Vector2 BaseSize = new Vector2(100.0f, 100.0f);
	}
}


