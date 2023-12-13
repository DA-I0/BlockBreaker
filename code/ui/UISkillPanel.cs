using Godot;

public partial class UISkillPanel : UIPanel
{
	[Export] private Label _name;
	[Export] private Label _description;
	[Export] private Button _returnButton;

	private int _currentSkill = 0;

	public override void _Ready()
	{
		SetupReferences();
		UpdateDisplayedValues();
	}

	protected override void SetupReferences()
	{
		refs = GetNode("/root/GameController") as SessionController;
		uiController = (UIController)GetNode("../..");
		uiController.RefreshUI += Focus;
		_returnButton.Pressed += () => ((UIController)GetNode("../..")).TogglePanel("GameSetupPanel");
	}

	private void UpdateDisplayedValues()
	{
		_name.Text = $"SKILL_{refs.Skills[_currentSkill].ToString().ToUpper()}_NAME";
		_description.Text = $"{Tr("SKILL_ACTIVATION_COST")}: {refs.Skills[_currentSkill].ActivationCost}\n{Tr($"SKILL_{refs.Skills[_currentSkill].ToString().ToUpper()}_DESC")}";
	}

	private void ChangeSkill(bool next)
	{
		_currentSkill += next ? 1 : -1;
		CheckSkillRange();
		UpdateDisplayedValues();
	}

	private void CheckSkillRange()
	{
		if (_currentSkill > refs.Skills.Length - 1)
		{
			_currentSkill = 0;
		}

		if (_currentSkill < 0)
		{
			_currentSkill = refs.Skills.Length - 1;
		}
	}

	private void SelectSkill()
	{
		refs.SetSkill(_currentSkill);
		uiController.TogglePanel("GameSetupPanel");
	}
}
