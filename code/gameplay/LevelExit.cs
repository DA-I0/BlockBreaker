using Godot;

public partial class LevelExit : Node
{
	private bool _isActive = false;

	[Export] private AnimationPlayer _animator;

	private Node _blockParent;
	protected SessionController refs;

	public override void _Ready()
	{
		_blockParent = GetNode("../Blocks");
		_blockParent.ChildExitingTree += CheckLevelProgress;
		refs = GetNode("/root/GameController") as SessionController;

		UpdateLevelExit();
	}

	private void OnBodyEntered(Node2D body)
	{
		if ((Ball)body != null)
		{
			refs.gameScore.AddBonusScore();
			refs.AdvanceCurrentLevel();
		}
	}

	private void CheckLevelProgress(Node node)
	{
		_isActive = _blockParent.GetChildCount() == 1;
		UpdateLevelExit();
	}

	private void UpdateLevelExit()
	{
		string state = _isActive ? "idle_enabled" : "idle_disabled";
		_animator.Play(state);

		if (_isActive)
		{
			refs.gameScore.InvokeExitTimer();
		}
	}
}
