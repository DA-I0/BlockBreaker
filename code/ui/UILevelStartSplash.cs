using Godot;

namespace BoGK.UI
{
	public partial class UILevelStartSplash : Control
	{
		[Export] private Label _levelName;
		[Export] private AnimationPlayer _animator;

		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.levelManager.SceneChangedWithInfo += Display;
		}

		private void Display(string sceneName)
		{
			_levelName.Text = sceneName.Replace("level_", $"{Tr("LEVEL")} ").Replace(".tscn", "").Replace(".remap", "");
			_animator.Play("activate");
		}
	}
}