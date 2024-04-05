using Godot;

public enum Variant { basic, garden };

namespace BoGK.GameSystem
{
	public partial class VariantController : Node2D
	{
		[Export] private Variant _variant;
		[Export] private Sprite2D[] _sprites;

		protected SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<SessionController>("/root/GameController");
			ApplyVariant();
		}

		protected void ApplyVariant()
		{
			foreach (Sprite2D sprite in _sprites)
			{
				string spritePath = sprite.Texture.ResourcePath;
				string oldSpriteVariant = spritePath.Split("_")[^1];
				string mutedSpritePath = spritePath.Replace(oldSpriteVariant, $"{_variant}_muted.png");

				if (refs.settings.UseAlternativeColorPalette && FileAccess.FileExists(mutedSpritePath))
				{
					spritePath = mutedSpritePath;
				}
				else
				{
					spritePath = spritePath.Replace(oldSpriteVariant, $"{_variant}.png");
				}

				sprite.Texture = ResourceLoader.Load<Texture2D>(spritePath);
			}
		}
	}
}