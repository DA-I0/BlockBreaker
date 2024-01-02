using Godot;

namespace BoGK.UI
{
	public partial class UISkillPanel : UIPaginatorPanel
	{
		[Export] private Label _name;
		[Export] private Label _description;

		private int _currentSkill = 0;

		public override void _Ready()
		{
			SetupBaseReferences();
			CreateItemIndicators(refs.gameData.Skills.Length);
			UpdateDisplayedValues();
		}

		private void UpdateDisplayedValues()
		{
			Skill currentSkill = refs.gameData.Skills[_currentSkill];
			_name.Text = $"SKILL_{currentSkill.ToString().ToUpper()}_NAME";
			_description.Text = $"{Tr("SKILL_ACTIVATION_COST")}: {currentSkill.ActivationCost}\n{Tr($"SKILL_{currentSkill.ToString().ToUpper()}_DESC")}";
			UpdatePaginatorStatus(_currentSkill);
		}

		private void ChangeSkill(bool next)
		{
			_currentSkill += next ? 1 : -1;
			CheckSkillRange();
			UpdateDisplayedValues();
		}

		private void CheckSkillRange()
		{
			if (_currentSkill > refs.gameData.Skills.Length - 1)
			{
				_currentSkill = 0;
			}

			if (_currentSkill < 0)
			{
				_currentSkill = refs.gameData.Skills.Length - 1;
			}
		}

		private void SelectSkill()
		{
			refs.SetSkill(_currentSkill);
			uiController.TogglePanel("GameSetupPanel");
		}
	}
}