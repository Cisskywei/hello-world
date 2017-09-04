using PaintCraft.Tools;

namespace PaintCraft.UI.Sliders{
	public class BrushHueSliderController : SliderValueBinder {
		public LineConfig LineConfig;
		#region implemented abstract members of SliderValueBinder
		
		public override float Value {
			get {
				return LineConfig.Color.H;
			}
			set {
				LineConfig.Color.H = value;
			}
		}
		
		#endregion
	}
}