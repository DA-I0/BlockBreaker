using Godot;

public partial class UIChangelog : RichTextLabel
{
	private SessionController refs;

	public override void _Ready()
	{
		((UIController)GetParent().GetParent().GetParent()).RefreshUI += LoadChangelog;
		refs = GetNode("/root/GameController/SessionController") as SessionController;
	}

	public void LoadChangelog()
	{
		Text = refs.gameData.PatchNotes;
	}
}
