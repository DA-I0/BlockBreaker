using Godot;

public partial class UIOptions : ScrollContainer
{
	[Export] private CheckButton _fullscreen;
	[Export] private OptionButton _resolution;

	private SessionController refs;

	public override void _Ready()
	{
		((UIController)GetParent().GetParent()).RefreshUI += UpdateSettings;
		refs = GetNode("/root/GameController/SessionController") as SessionController;
	}

	public void UpdateSettings()
	{
		// Text = refs.gameData.PatchNotes;
	}
}
