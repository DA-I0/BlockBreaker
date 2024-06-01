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
			ClearOldEntries();

			int position = 1;

			foreach (Models.HighScore entry in refs.gameData.Leaderboard)
			{
				ConstructHighScoreEntry(entry, position);
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

		private void ConstructHighScoreEntry(Models.HighScore entry, int position)
		{
			TextureRect scoreEntry = new TextureRect
			{
				CustomMinimumSize = new Vector2(0, 12),
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				SelfModulate = new Color(0f, 0f, 0f, 0.05f),
				Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/game_area.png")
			};

			// Name and diff
			string difficulty = entry.UsedCustomDifficulty ? $"{entry.DifficultyName}*" : Tr(entry.DifficultyName);

			Label playerName = new Label
			{
				AnchorRight = 0.5f,
				AnchorTop = 0,
				AnchorBottom = 1,
				OffsetLeft = 5,
				Text = $"{position}. {entry.PlayerName} ({difficulty})\n",
				VerticalAlignment = VerticalAlignment.Center,
				GrowVertical = GrowDirection.Both
			};

			scoreEntry.AddChild(playerName);

			// Icons
			HBoxContainer iconContainer = new HBoxContainer
			{
				AnchorLeft = 0.5f,
				AnchorTop = 0.5f,
				AnchorRight = 0.8f,
				AnchorBottom = 0.5f,
				GrowVertical = GrowDirection.Both,
				CustomMinimumSize = new Vector2(0, 12),
				Alignment = BoxContainer.AlignmentMode.End,
				Name = $"{entry.PlayerName}SessionIcons"
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
				OffsetRight = -5,
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
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
				StretchMode = TextureRect.StretchModeEnum.KeepAspect,
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				Texture = ResourceLoader.Load<Texture2D>($"{ProjectSettings.GetSetting("global/CustomSessionIconsFilePath")}/{iconName}.png")
			};
		}
	}
}