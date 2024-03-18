using Godot;

namespace BoGK.UI
{
	public partial class UIOptionsBaseCategory : ScrollContainer
	{
		[Export] protected Control[] _focusTarget;

		protected int _focusIndex = 0;

		protected GameSystem.SessionController Refs
		{
			get { return _mainPanel.Refs; }
		}

		protected UIOptionsPanel _mainPanel;

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
			_mainPanel = GetParent().GetParent<UIOptionsPanel>();
			_mainPanel.UIController.RefreshUI += Focus;
		}

		public virtual void Enable()
		{
			Visible = true;
			UpdateSettings();
			Focus();
		}

		public virtual void Disable()
		{
			Visible = false;
		}

		public virtual void UpdateSettings() { }
		public virtual void ApplySettings() { }
		public virtual void RestoreDefaultSettings() { }

		protected virtual void Focus()
		{
			if (Visible && _focusTarget.Length > 0)
			{
				_focusTarget[0].GrabFocus();
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
	}
}