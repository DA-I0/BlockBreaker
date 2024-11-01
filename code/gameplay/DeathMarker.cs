using Godot;

namespace BoGK.Gameplay
{
	public partial class DeathMarker : Node2D
	{
		private GameSystem.SessionController refs;

		public override void _Ready()
		{
			refs = GetNode<GameSystem.SessionController>("/root/GameController");
			refs.health.LifeChanged += ToggleVisibility;
			refs.GameSetup += SetupMarker;
		}

		private void ToggleVisibility(int livesLeft)
		{
			Visible = livesLeft < 2;
		}

		private void SetupMarker()
		{
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, refs.settings.EffectTransparency);
			ToggleVisibility(refs.SelectedDifficulty.StartingLives);
		}
	}
}