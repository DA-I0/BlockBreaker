namespace BoGK.Models
{
	public struct BreakableVariant
	{
		public string TypeName;
		public bool UseDefaultSprite;
		public Godot.Color CustomColor;

		public BreakableVariant(string typeName, bool defaultSprite, Godot.Color color)
		{
			TypeName = typeName;
			UseDefaultSprite = defaultSprite;
			CustomColor = color;
		}
	}
}