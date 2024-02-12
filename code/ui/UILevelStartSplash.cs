using Godot;

namespace BoGK.UI
{
	public partial class UILevelStartSplash : Control
	{
		[Export] private Label _levelName;
		[Export] private AnimationPlayer _animator;

		private SessionController refs;

		public override void _Ready()
		{
			refs = GetNode("/root/GameController") as SessionController;
			refs.levelManager.SceneChangedWithInfo += Display;
		}

		private void Display(string sceneName)
		{
			_levelName.Text = sceneName.Replace("level_", $"{Tr("LEVEL")} ").Replace(".tscn", "");
			_animator.Play("activate");
		}
	}
}