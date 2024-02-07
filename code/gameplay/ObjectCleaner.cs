using Godot;

public partial class ObjectCleaner : Node2D
{
	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController") as SessionController;
		refs.levelManager.ResetSession += Destroy;
	}

	public void Destroy()
	{
		refs.levelManager.ResetSession -= Destroy;
		QueueFree();
	}
}
