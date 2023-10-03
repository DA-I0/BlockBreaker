using Godot;

public delegate void LifeNotification(int changeValue);

public partial class Health : Node
{
	private int _lives;

	private SessionController refs;

	public event Notification GameOver;
	public event Notification ResetElements;
	public event LifeNotification LifeChanged;

	public override void _Ready()
	{
		refs = GetParent().GetChild(0) as SessionController;
		refs.GameSetup += SetupInitialValues;
	}

	public void ChangeLives(int amount)
	{
		_lives += amount;

		if (_lives > refs.SelectedDifficulty.MaxLives)
		{
			_lives = refs.SelectedDifficulty.MaxLives;
		}

		LifeChanged?.Invoke(_lives);

		if (amount < 1)
		{
			refs.audioController.PlayAudio(2);
			ResetElements?.Invoke();
		}

		CheckForGameOver();
	}

	private void SetupInitialValues()
	{
		_lives = refs.SelectedDifficulty.StartingLives;
		LifeChanged?.Invoke(_lives);
	}

	private void CheckForGameOver()
	{
		if (_lives < 0)
		{
			GameOver?.Invoke();
		}
	}
}
