using Godot;

namespace BoGK.UI
{
	public partial class UIColorPicker : Control
	{
		[Export] private ColorPicker _colorPicker;
		[Export] TextureRect _targetSprite;

		private Control _target;

		public void Activate(Control target)
		{
			_target = target;
			_colorPicker.Color = _target.Modulate;
			_targetSprite.Texture = ((Button)target).Icon;
			_targetSprite.Modulate = _target.Modulate;
			Visible = true;
		}

		private void Deactivate()
		{
			Visible = false;
		}

		private void UpdateColor(Color newColor)
		{
			_target.Modulate = newColor;
			_targetSprite.Modulate = newColor;
		}
	}
}
