using Godot;

public partial class ObjectCleaner : Node2D
{
	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.levelManager.ResetSession += Destroy;
	}

	private void Destroy()
	{
		QueueFree();
	}
}
