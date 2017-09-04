
namespace PaintCraft.Tools{
	[System.Serializable]
	public class BrushFloatProperty {
		public BrushPropertyType PropertyType;
		public float Value;
	}

	public enum BrushPropertyType{
		Fixed,
		Adjustable
	}
}