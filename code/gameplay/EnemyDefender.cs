using Godot;

namespace BoGK.Gameplay
{
	public partial class EnemyDefender : Enemy
	{
		public override void BraceOnContact(Node2D attacker)
		{
			if (_state == EnemyState.hurt)
			{
				return;
			}

			PauseMovement();
			AnimateShieldHit();
			Vector2 newVelocity = _moveDirection * -1;
			KinematicCollision2D collision = MoveAndCollide(newVelocity, false, 0.01f);
		}

		private void AnimateShieldHit()
		{
			switch (SpriteDirection())
			{
				case Vector2(0f, -1f):
					_animator.Play("brace_up");
					break;

				case Vector2(-1f, 0f):
					_animator.Play("brace_left");
					break;

				case Vector2(1f, 0f):
					_animator.Play("brace_right");
					break;

				default:
					_animator.Play("brace_down");
					break;
			}
		}
	}
}
