using Godot;

public partial class UIPanel : Panel
{
	[Export] protected Control _focusTarget;

	protected SessionController refs;
	protected UIController uiController;

	public override void _Ready()
	{
		SetupReferences();
	}

	protected virtual void SetupReferences()
	{
		refs = GetNode("/root/GameController") as SessionController;
		uiController = (UIController)GetNode("../..");
		uiController.RefreshUI += Focus;
	}

	protected virtual void Focus()
	{
		if (Visible)
		{
			_focusTarget.GrabFocus();
		}
	}
}