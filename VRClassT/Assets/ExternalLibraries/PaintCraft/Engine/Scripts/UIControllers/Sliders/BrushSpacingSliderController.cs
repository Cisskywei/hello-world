using PaintCraft.Tools;

namespace PaintCraft.UI.Sliders{
	public class BrushSpacingSliderController : SliderValueBinder {
		public LineConfig LineConfig;
		#region implemented abstract members of SliderValueBinder

		public override float Value {
			get {
				return LineConfig.Spacing;
			}
			set {
				LineConfig.Spacing = value;
			}
		}

		#endregion


	}
}
