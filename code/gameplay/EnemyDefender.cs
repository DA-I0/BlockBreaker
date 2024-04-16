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
		}
	}
}
