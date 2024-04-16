using Godot;

namespace BoGK.Gameplay
{
	public partial class SecondaryCollider : Node
	{
		private Enemy _parent;

		public override void _Ready()
		{
			_parent = GetParent<Enemy>();
		}

		public void HitContact(Node2D source)
		{
			_parent.BraceOnContact(source);
		}
	}
}