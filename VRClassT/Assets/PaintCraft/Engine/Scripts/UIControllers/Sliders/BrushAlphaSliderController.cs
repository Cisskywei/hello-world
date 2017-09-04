using PaintCraft.Tools;


namespace PaintCraft.UI.Sliders{
	public class BrushAlphaSliderController : SliderValueBinder {
		public LineConfig LineConfig;
		#region implemented abstract members of SliderValueBinder

		public override float Value {
			get {
				return LineConfig.Color.Alpha;
			}
			set {
				LineConfig.Color.Alpha = value;
			}
		}

		#endregion



	}
}