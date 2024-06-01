using Godot;

namespace BoGK.UI
{
	public partial class UIColorPicker : Control
	{
		[Export] private ColorPicker _colorPicker;
		[Export] private TextureRect _targetSprite;

		private Control _target;

		public override void _Ready()
		{
			Deactivate();
		}

		public void Activate(Control target)
		{
			_target = target;
			_colorPicker.Color = _target.Modulate;
			_targetSprite.Texture = ((Button)target).Icon;
			_targetSprite.Modulate = _target.Modulate;
			Visible = true;
			_colorPicker.GrabFocus();
			GetTree().Paused = true;
		}

		private void Deactivate()
		{
			GetTree().Paused = false;
			Visible = false;
			_target?.GrabFocus();
		}

		private void UpdateColor(Color newColor)
		{
			_targetSprite.Modulate = newColor;
		}

		private void Confirm()
		{
			_target.Modulate = _targetSprite.Modulate;
			Deactivate();
		}
	}
}
