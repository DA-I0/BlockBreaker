using Godot;

public partial class Level : Node
{
	protected SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
	}

	private void CompleteLevel()
	{
		// refs.gameScore.AddBonusScore();
		// refs.levelManager.LoadMenuScene(); // load next level
	}
}
