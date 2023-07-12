using UnityEngine;

public class GameScore : MonoBehaviour
{
	#region Variables
	private static int _maxMultiplier = 10;
	private static int _maxChainMultiplier = 5;
	private static int _bonusTime = 60;

	private int _score = 0;

	private int _comboChain = 0;
	private int _scoreMultiplier = 1;

	private int _timeToExit = -1;

	private Gameplay _gameState;
	private PaddleControls _paddle;
	#endregion

	#region Properties
	public int Score
	{
		get { return _score; }
		set { _score = value; }
	}

	public int Multiplier
	{
		get { return _scoreMultiplier; }
		set { _scoreMultiplier = value; }
	}

	public int TimeToExit { get => _timeToExit; set => _timeToExit = value; }


	#endregion

	#region Methods (public)
	public void Cleanup()
	{
		_score = 0;
		ResetMultiplier();
	}

	public void ChangeScore(int amount, bool increaseCombo = true)
	{
		if (increaseCombo)
		{
			_comboChain++;
		}

		UpdateMultiplier();
		_score += amount * _scoreMultiplier;

		_gameState.PlaySound(2);
		BroadcastMessage("UpdateScore");
	}

	public void ResetMultiplier()
	{
		_comboChain = 0;
		_scoreMultiplier = 1;
		BroadcastMessage("UpdateScore");
	}

	public void InvokeExitTimer()
	{
		_timeToExit = _bonusTime;
		BroadcastMessage("UpdateTimer");
		Invoke("DecreaseBonusTimer", 1f);
	}

	public void AddBonusScore()
	{
		ChangeScore(_timeToExit, false);
		_timeToExit = -1;
		BroadcastMessage("UpdateTimer");
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		_paddle = GameObject.Find("paddle").GetComponent<PaddleControls>();
	}

	private void Start()
	{
		_gameState = gameObject.GetComponent<Gameplay>();
		Cleanup();
	}

	private void UpdateMultiplier()
	{
		int chainMultiplier = (_comboChain / 10) + 1;
		_scoreMultiplier = (chainMultiplier > _maxChainMultiplier) ? _maxChainMultiplier : chainMultiplier;

		if (_paddle.SizeMultiplier < 0)
		{
			_scoreMultiplier -= _paddle.SizeMultiplier;
		}

		BallControler ball = GameObject.FindGameObjectWithTag("ball").GetComponent<BallControler>();

		if (ball.GetBallSize() < 1)
		{
			_scoreMultiplier++;
		}

		if (ball.GetBallSpeed() > 1)
		{
			_scoreMultiplier++;
		}

		CheckMultiplierRange();
	}

	private void CheckMultiplierRange()
	{
		if (_scoreMultiplier < 1)
		{
			_scoreMultiplier = 1;
			return;
		}

		if (_scoreMultiplier > _maxMultiplier)
		{
			_scoreMultiplier = _maxMultiplier;
		}
	}

	private void DecreaseBonusTimer()
	{
		_timeToExit--;
		BroadcastMessage("UpdateTimer");

		if (_timeToExit > 0)
		{
			Invoke("DecreaseBonusTimer", 1f);
		}

		Debug.Log("exit timer: " + _timeToExit);
	}
	#endregion
}
