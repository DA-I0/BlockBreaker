using System.Linq;
using Godot;

namespace BoGK.UI
{
	public partial class UIGameSetupPanel : UIPanel
	{
		[Export] private Node _levelGrid;
		[Export] private Button _paddleButton;
		[Export] private Button _difficultyButton;
		[Export] private Button _skillButton;

		private int _lastActivePanel = -1;

		private UIController uIControler;

		public override void _Ready()
		{
			SetupBaseReferences();
			SetupReferences();
			PopulateLevels();
		}

		private void PopulateLevels()
		{
			for (int index = 0; index < refs.gameData.Levels.Count; index++)
			{
				Button newButton = new Button
				{
					Text = refs.gameData.Levels[index].Replace("level_", "").Replace(".tscn", "").Replace(".remap", ""),
					CustomMinimumSize = new Vector2(22, 16)
				};

				int levelIndex = index;
				newButton.Pressed += () => refs.SelectLevel(levelIndex);

				newButton.SizeFlagsHorizontal = SizeFlags.ShrinkCenter | SizeFlags.Expand;
				newButton.SizeFlagsVertical = SizeFlags.ShrinkCenter;

				_levelGrid.AddChild(newButton);

				if (index > 0)
				{
					Button previousNode = _levelGrid.GetChild<Button>(_levelGrid.GetChildCount() - 2);
					newButton.FocusNeighborLeft = previousNode.GetPath();
					previousNode.FocusNeighborRight = newButton.GetPath();
				}
			}

			_levelGrid.GetChild<Button>(0).FocusNeighborLeft = _levelGrid.GetChild(_levelGrid.GetChildCount() - 1).GetPath();
			_levelGrid.GetChild<Button>(_levelGrid.GetChildCount() - 1).FocusNeighborRight = _levelGrid.GetChild(0).GetPath();
			_focusTarget = _focusTarget.Append(_levelGrid.GetChild<Control>(0)).ToArray();
		}

		protected void SetupReferences()
		{
			uiController = GetNode<UIController>("../..");
			_paddleButton.Pressed += () => DisplayPaddlePanel();
			_skillButton.Pressed += () => DisplaySkillPanel();
			_difficultyButton.Pressed += () => DisplayDifficultyPanel();
		}

		protected override void Focus()
		{
			if (Visible)
			{
				switch (_lastActivePanel)
				{
					case 0:
						_paddleButton.GrabFocus();
						break;

					case 1:
						_skillButton.GrabFocus();
						break;

					case 2:
						_difficultyButton.GrabFocus();
						break;

					default:
						((Button)_levelGrid.GetChild(0)).GrabFocus();
						_focusIndex = 1;
						break;
				}

				UpdateSelectedPaddle();
				UpdateSelectedDifficulty();
				UpdateSelectedSkill();
				_lastActivePanel = -1;
			}
		}

		private void UpdateSelectedPaddle()
		{
			if (ResourceLoader.Exists($"res://assets/sprites/paddles/paddle_{refs.SelectedPaddleIndex}_icon.png"))
			{
				_paddleButton.Icon = ResourceLoader.Load<Texture2D>($"res://assets/sprites/paddles/paddle_{refs.SelectedPaddleIndex}_icon.png");
			}
			else
			{
				_paddleButton.Icon = ResourceLoader.Load<Texture2D>($"res://assets/sprites/paddles/paddle_{refs.SelectedPaddleIndex}.png");
			}
		}

		private void UpdateSelectedDifficulty()
		{
			if (refs.SelectedDifficulty.DifficultyName.Contains("DIFF_DEFAULT_"))
			{
				_difficultyButton.Text = Tr(refs.SelectedDifficulty.DifficultyName);
			}
			else
			{
				_difficultyButton.Text = refs.SelectedDifficulty.DifficultyName;
			}
		}

		private void UpdateSelectedSkill()
		{
			_skillButton.Text = Tr($"SKILL_{refs.SelectedSkill.ToString().ToUpper()}_NAME");
		}

		private void DisplayPaddlePanel()
		{
			_lastActivePanel = 0;
			uiController.TogglePanel("PaddlePanel");
		}

		private void DisplaySkillPanel()
		{
			_lastActivePanel = 1;
			uiController.TogglePanel("SkillPanel");
		}

		private void DisplayDifficultyPanel()
		{
			_lastActivePanel = 2;
			uiController.TogglePanel("DifficultyPanel");
		}
	}
}