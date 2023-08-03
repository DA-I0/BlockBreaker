using UnityEngine;

public class GameScore : MonoBehaviour
{
	#region Variables
	private static int MaxMultiplier = 10;
	private static int MaxChainMultiplier = 5;
	private static int BonusTime = 60;

	private int _score = 0;

	private int _comboChain = 0;
	private int _scoreMultiplier = 1;

	private int _timeToExit = -99;

	// References
	private AudioController _audio;
	private Gameplay _gameState;
	private PaddleControls _paddle;
	#endregion

	#region Properties
	public int Score
	{
		get { return _score; }
	}

	public int Multiplier
	{
		get { return _scoreMultiplier; }
	}

	public int TimeToExit
	{
		get { return _timeToExit; }
	}


	#endregion

	#region Methods (public)
	public void Cleanup()
	{
		_score = 0;
		ResetMultiplier();
		ResetTimer();
	}

	public void ChangeScore(int pointValue, bool increaseCombo = true)
	{
		if (increaseCombo)
		{
			_comboChain++;
		}

		UpdateMultiplier();
		_score += pointValue * _scoreMultiplier;

		_audio.PlaySound(2);
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
		_timeToExit = BonusTime;
		BroadcastMessage("UpdateTimer");
		Invoke("DecreaseBonusTimer", 1f);
	}

	public void AddBonusScore()
	{
		if (_timeToExit > 0)
		{
			ChangeScore(_timeToExit, false);
		}

		ResetTimer();
	}
	#endregion

	#region Methods (private)
	private void Awake()
	{
		_paddle = GameObject.Find("paddle").GetComponent<PaddleControls>();
	}

	private void Start()
	{
		_audio = gameObject.GetComponent<AudioController>();
		_gameState = gameObject.GetComponent<Gameplay>();
		Cleanup();
	}

	private void UpdateMultiplier()
	{
		int chainMultiplier = (_comboChain / 10) + 1;
		_scoreMultiplier = (chainMultiplier > MaxChainMultiplier) ? MaxChainMultiplier : chainMultiplier;

		if (_paddle.SizeMultiplier < 0)
		{
			_scoreMultiplier -= _paddle.SizeMultiplier;
		}

		BallController ball = GameObject.FindGameObjectWithTag("ball").GetComponent<BallController>();

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

		if (_scoreMultiplier > MaxMultiplier)
		{
			_scoreMultiplier = MaxMultiplier;
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
	}

	private void ResetTimer()
	{
		_timeToExit = -99;
		BroadcastMessage("UpdateTimer");
	}
	#endregion
}
