using Godot;

namespace BoGK.Gameplay
{
	public partial class ScreenShake : Skill
	{
		private const string PrefabPath = "res://prefabs/skills/screenshake_debris.tscn";
		private const int DebrisCount = 5;
		private const int MaxOffesetHor = 90;
		private const int MaxOffesetVer = 60;

		public ScreenShake()
		{
			_activationPointsCost = 10;
		}

		protected override void ApplySkillEffect()
		{
			refs.paddle.SetPaddleState(PaddleState.confused, 3f);

			for (int i = 0; i < refs.Balls.Count; i++)
			{
				float angleChange = GD.RandRange(5, 15);
				int direction = GD.RandRange(-1, 1) < 0 ? -1 : 1;
				((Ball)refs.Balls[i]).ChangeRotation(angleChange * direction);
			}

			Node level = refs.GetNode("CurrentScene").GetChild(0);

			for (int index = 0; index < DebrisCount; index++)
			{
				Node2D debris = (Node2D)ResourceLoader.Load<PackedScene>(PrefabPath).Instantiate();
				debris.Position = new Vector2(GD.RandRange(-MaxOffesetHor, MaxOffesetHor), GD.RandRange(-MaxOffesetVer, MaxOffesetVer));
				level.AddChild(debris);
			}

			refs.audioController.PlayAudio(3);
			OnActivation();
		}
	}
}