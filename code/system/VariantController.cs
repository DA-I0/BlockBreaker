using Godot;

public enum Variant { none, basic, garden };

namespace BoGK.GameSystem
{
	public partial class VariantController : Node2D
	{
		[Export] private Variant _variant;
		[Export] private Sprite2D[] _sprites;

		public override void _Ready()
		{
			ApplyVariant();
		}

		protected void ApplyVariant()
		{
			if (_variant == Variant.none)
			{
				return;
			}

			foreach (Sprite2D sprite in _sprites)
			{
				string spritePath = sprite.Texture.ResourcePath;
				string oldSpriteVariant = spritePath.Split("_")[^1];
				spritePath = spritePath.Replace(oldSpriteVariant, $"{_variant}.png");
				sprite.Texture = ResourceLoader.Load<Texture2D>(spritePath);
			}
		}
	}
}