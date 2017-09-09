

namespace PaintCraft.Tools{
	[System.Serializable]
	public class SpacingProperty : BrushFloatProperty {

		public float GetSpacingValue(BrushContext brushLineContext){
			switch (PropertyType){
			case BrushPropertyType.Fixed:
				return Value;
			default:
				return brushLineContext.LineConfig.Spacing;
			}
		}
	}
}