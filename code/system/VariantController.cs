using Godot;

public enum Variant { basic, room, garden };

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
				string defaultSpriteVariant = spritePath.Split("_")[^1];
				string customSpritePath = spritePath.Replace(defaultSpriteVariant, $"{_variant}_{refs.settings.BackgroundColorPalette}.png");

				if (FileAccess.FileExists(customSpritePath))
				{
					spritePath = customSpritePath;
				}
				else
				{
					spritePath = spritePath.Replace(defaultSpriteVariant, $"{_variant}.png");
				}

				sprite.Texture = ResourceLoader.Load<Texture2D>(spritePath);
			}
		}
	}
}