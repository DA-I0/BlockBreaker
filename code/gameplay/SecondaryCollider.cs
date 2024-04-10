using BoGK.Interfaces;
using Godot;

namespace BoGK.Gameplay
{
	public partial class SecondaryCollider : Node, IBreakable
	{
		private Enemy _parent;

		public override void _Ready()
		{
			_parent = GetParent<Enemy>();
		}

		public void Damage(int value)
		{
			_parent.PauseMovement();
		}
	}
}