using Godot;

namespace BoGK.Gameplay
{
	public partial class ObjectCleaner : Node2D
	{
		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.levelManager.ResetSession += Destroy;
		}

		public void Destroy()
		{
			refs.levelManager.ResetSession -= Destroy;
			QueueFree();
		}
	}
}