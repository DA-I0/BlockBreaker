using Godot;

public partial class UIChangelog : RichTextLabel
{
	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		((UIController)GetNode("../../../..")).RefreshUI += LoadChangelog; // TODO: replace the GetParent chain
	}

	public void LoadChangelog()
	{
		Text = refs.gameData.PatchNotes;
	}
}
