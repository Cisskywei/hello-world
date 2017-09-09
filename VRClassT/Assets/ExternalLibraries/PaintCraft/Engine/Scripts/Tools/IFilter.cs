using NodeInspector;

namespace PaintCraft.Tools{
	[OneWay]
    public abstract class IFilter : ScriptableObjectNode {
		/// <summary>
		/// Apply the specified brushLineContext and drawEnded.
		/// return:
		/// - true : if brush need to continue aply remaining filters
		/// - false: don't apply remaining filters
		/// </summary>
		/// <param name="brushLineContext">Brush line context.</param>
		/// <param name="drawEnded">If set to <c>true</c> draw ended.</param>
		public abstract void Apply(BrushContext brushLineContext);	
	}
}
