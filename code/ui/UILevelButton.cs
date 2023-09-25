using Godot;

public partial class UILevelButton : Button
{
	private int _levelIndex;

	SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
	}

	public void ButtonSetup(int index, string fileName)
	{
		_levelIndex = index;
		Text = fileName.Replace(".tscn", "");
	}

	private void OnPressed()
	{
		refs.SelectLevel(_levelIndex);
	}
}
