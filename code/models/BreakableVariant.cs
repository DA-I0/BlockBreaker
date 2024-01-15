namespace BoGK.Models
{
	public struct BreakableVariant
	{
		public string TypeName;
		public int SpriteVariant;
		public Godot.Color CustomColor;

		public BreakableVariant(string typeName, int variantId, Godot.Color color)
		{
			TypeName = typeName;
			SpriteVariant = variantId;
			CustomColor = color;
		}
	}
}