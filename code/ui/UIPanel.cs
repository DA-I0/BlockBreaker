using Godot;

namespace BoGK.UI
{
	public partial class UIPanel : Panel
	{
		[Export] protected UIController uiController;
		[Export] protected Control[] _focusTarget;
		[Export] private Button _returnButton;
		[Export] protected string _returnTarget = string.Empty;

		protected int _focusIndex = 0;

		protected SessionController refs;

		public override void _Ready()
		{
			SetupBaseReferences();
		}

		public override void _Input(InputEvent @event)
		{
			if (!Visible)
			{
				return;
			}

			if (@event.IsActionPressed("ui_toggle_focus"))
			{
				ToggleFocus();
			}
		}

		protected virtual void SetupBaseReferences()
		{
			refs = GetNode("/root/GameController") as SessionController;
			uiController.RefreshUI += Focus;
			_returnButton.Pressed += () => Return();
		}

		protected virtual void Focus()
		{
			if (Visible && _focusTarget.Length > 0)
			{
				_focusTarget[0].GrabFocus();
			}
		}

		protected virtual void ToggleFocus()
		{
			_focusTarget[_focusIndex]?.ReleaseFocus();
			_focusIndex = (_focusIndex < _focusTarget.Length - 1) ? _focusIndex + 1 : 0;

			_focusTarget[_focusIndex]?.GrabFocus();
		}

		protected virtual void Return()
		{
			uiController.TogglePanel(_returnTarget);
		}
	}
}
