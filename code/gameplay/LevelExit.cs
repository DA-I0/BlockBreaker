using BoGK.GameSystem;
using Godot;

namespace BoGK.Gameplay
{
	public partial class LevelExit : VariantController
	{
		private bool _isActive = false;

		[Export] private AnimationPlayer _animator;

		private Node _blockParent;

		public override void _Ready()
		{
			refs = GetNode<SessionController>("/root/GameController");
			ApplyVariant();

			_blockParent = GetNode("../Blocks");
			_blockParent.ChildExitingTree += CheckLevelProgress;

			UpdateLevelExit();
		}

		private void OnBodyEntered(Node2D body)
		{
			if ((Ball)body != null)
			{
				refs.gameScore.AddBonusScore();
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
}