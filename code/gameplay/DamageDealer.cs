using Godot;

namespace BoGK.Gameplay
{
	public partial class DamageDealer : Node2D
	{
		[Export] protected int _strength = 3;
		[Export] protected float _delay = 0;
		[Export] protected bool _randomizeDelay = false;

		[Export] protected AnimationPlayer _animator;
		[Export] protected Timer _timer;

		public override void _Ready()
		{
			_animator.AnimationFinished += Cleanup;
			RandomizeDelay();
		}

		private void Activate()
		{
			_animator.Play("idle");
		}

		private void Cleanup(StringName animName)
		{
			GetNode<GameSystem.SessionController>("/root/GameController").paddle.VibrateController(0.35f, 0.15f, 0.12f);

			_animator.AnimationFinished -= Cleanup;
			QueueFree();
		}

		private void ApplyDamageToObject(Node2D target)
		{
			if (target as Breakable != null)
			{
				((Breakable)target).Damage(_strength);
				return;
			}
		}

		private void RandomizeDelay()
		{
			if (_randomizeDelay)
			{
				_timer.Start(GD.RandRange(0, _delay));
			}
			else
			{
				_timer.Start(_delay);
			}
		}
	}
}