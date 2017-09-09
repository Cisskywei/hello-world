using PaintCraft.Tools;


namespace PaintCraft.UI.Sliders{
	public class BrushSizeSliderController : SliderValueBinder {
		public LineConfig LineConfig;
		#region implemented abstract members of SliderValueBinder
		public override float Value {
			get {
				return LineConfig.Scale;
			}
			set {
				LineConfig.Scale = value;
			}
		}
		#endregion


	    void Update()
	    {
	        Value = LineConfig.Scale;
	    }
	}
}
