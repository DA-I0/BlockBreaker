using Godot;

namespace BoGK.Gameplay
{
	public partial class EnemyDeflector : Enemy
	{
		[Export] private float _deflectBoost;

		public override void BraceOnContact(Node2D attacker)
		{
			if (_state == EnemyState.hurt)
			{
				return;
			}

			RestorePreviousState();
			DeflectHit(attacker);
			PauseMovement();
		}

		private void DeflectHit(Node2D attacker)
		{
			((Ball)attacker)?.SetTempSpeedMultiplier(_deflectBoost);
			AnimateShieldBash();
		}

		private void AnimateShieldBash()
		{
			switch (SpriteDirection())
			{
				case Vector2(0f, -1f):
					_animator.Play("shield_bash_up");
					break;

				case Vector2(-1f, 0f):
					_animator.Play("shield_bash_left");
					break;

				case Vector2(1f, 0f):
					_animator.Play("shield_bash_right");
					break;

				default:
					_animator.Play("shield_bash_down");
					break;
			}
		}

		protected override void PauseMovement()
		{
			_lastState = _state;
			_state = EnemyState.idle;
			_invulnerabilityTimer.Stop();
			_invulnerabilityTimer.Start(0.1f);
		}
	}
}
