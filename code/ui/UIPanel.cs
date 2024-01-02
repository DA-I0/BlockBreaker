using Godot;

namespace BoGK.UI
{
	public partial class UIPanel : Panel
	{
		[Export] protected UIController uiController;
		[Export] protected Control _focusTarget;
		[Export] private Button _returnButton;
		[Export] private string _returnTarget = string.Empty;

		protected SessionController refs;

		public override void _Ready()
		{
			SetupBaseReferences();
		}

		protected virtual void SetupBaseReferences()
		{
			refs = GetNode("/root/GameController") as SessionController;
			uiController.RefreshUI += Focus;
			_returnButton.Pressed += () => uiController.TogglePanel(_returnTarget);
		}

		protected virtual void Focus()
		{
			if (Visible)
			{
				_focusTarget.GrabFocus();
			}
		}
	}
}