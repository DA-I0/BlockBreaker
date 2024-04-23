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

		protected GameSystem.SessionController refs;

		public override void _Ready()
		{
			SetupBaseReferences();
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("ui_toggle_focus"))
			{
				ToggleFocus();
			}
		}

		public void Enable()
		{
			ProcessMode = ProcessModeEnum.Inherit;
			Visible = true;
			Focus();
			UpdateDisplayedValues();
		}

		public void Disable()
		{
			ProcessMode = ProcessModeEnum.Disabled;
			Visible = false;
		}

		protected virtual void SetupBaseReferences()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			_returnButton.Pressed += () => Return();
		}

		protected virtual void Focus()
		{
			if (_focusTarget.Length > 0)
			{
				_focusTarget[0]?.GrabFocus();
			}
		}

		protected virtual void ToggleFocus()
		{
			if (_focusTarget == null || _focusTarget.Length < 1)
			{
				return;
			}

			_focusTarget[_focusIndex]?.ReleaseFocus();
			_focusIndex = (_focusIndex < _focusTarget.Length - 1) ? _focusIndex + 1 : 0;

			_focusTarget[_focusIndex]?.GrabFocus();
		}

		protected virtual void UpdateDisplayedValues() { }

		protected virtual void Return()
		{
			uiController.TogglePanel(_returnTarget);
		}
	}
}
