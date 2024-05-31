using Godot;

namespace BoGK.UI
{
	public partial class UILeaderboard : UIPanel
	{
		[Export] private Control _playerSessionIcons;

		public override void _Ready()
		{
			SetupBaseReferences();
		}

		protected override void UpdateDisplayedValues()
		{
			int position = 1;

			foreach (Models.HighScore entry in refs.gameData.Leaderboard)
			{
				GenerateCustomSessionIcons(entry, position);

				position++;
			}
		}

		private void ClearOldEntries()
		{
			foreach (Node entry in _playerSessionIcons.GetChildren())
			{
				entry.QueueFree();
			}
		}

		private void GenerateCustomSessionIcons(Models.HighScore entry, int position)
		{
			Control scoreEntry = new Control
			{
				CustomMinimumSize = new Vector2(0, 12)
			};

			// Name and diff
			string difficulty = entry.UsedCustomDifficulty ? $"{entry.DifficultyName}*" : Tr(entry.DifficultyName);

			Label playerName = new Label
			{
				AnchorRight = 0.5f,
				AnchorTop = 0,
				AnchorBottom = 1,
				Text = $"{position}. {entry.PlayerName} ({difficulty})\n",
				VerticalAlignment = VerticalAlignment.Center,
				GrowVertical = GrowDirection.Both
			};

			scoreEntry.AddChild(playerName);

			// Icons
			HBoxContainer iconContainer = new HBoxContainer
			{
				AnchorLeft = 0.5f,
				AnchorRight = 0.8f,
				Name = $"{entry.PlayerName}SessionIcons",
				CustomMinimumSize = new Vector2(0, 16),
				Alignment = BoxContainer.AlignmentMode.End
			};

			if (entry.UsedCustomSessionLenght)
			{
				iconContainer.AddChild(CreateIcon("session_length"));
			}

			if (entry.UsedStageShuffle)
			{
				iconContainer.AddChild(CreateIcon("stage_shuffle"));
			}

			if (entry.UsedDisablePickups)
			{
				iconContainer.AddChild(CreateIcon("disable_pickups"));
			}

			if (entry.UsedDisappearingBall)
			{
				iconContainer.AddChild(CreateIcon("disappearing_ball"));
			}

			scoreEntry.AddChild(iconContainer);

			// Score
			Label playerDifficulty = new Label
			{
				AnchorLeft = 0.8f,
				AnchorTop = 0,
				AnchorRight = 1f,
				AnchorBottom = 1,
				Text = $"{entry.Score}",
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Center,
				GrowVertical = GrowDirection.Both
			};

			scoreEntry.AddChild(playerDifficulty);

			_playerSessionIcons.AddChild(scoreEntry);
		}

		private TextureRect CreateIcon(string iconName)
		{
			return new TextureRect
			{
				CustomMinimumSize = new Vector2(8, 8),
				StretchMode = TextureRect.StretchModeEnum.KeepAspect,
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				Scale = new Vector2(0.5f, 0.5f),
				Texture = ResourceLoader.Load<Texture2D>($"{ProjectSettings.GetSetting("global/CustomSessionIconsFilePath")}/{iconName}.png")
			};
		}
	}
}