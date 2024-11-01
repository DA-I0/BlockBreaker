using Godot;

namespace BoGK.Gameplay
{
	public partial class Interactable : GameSystem.VariantController
	{
		[Export] protected bool _isActived = true;
		[Export] protected float _cooldown = 1f;

		[Export] protected AnimationPlayer _animator;
		[Export] protected Timer _timer;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			ApplyVariant();
			UpdateState();
		}

		protected virtual void UpdateState()
		{
			string state = _isActived ? "idle_enabled" : "idle_disabled";
			_animator.Play(state);
		}

		protected virtual void Toggle()
		{
			_isActived = !_isActived;
			UpdateState();
		}
	}
}