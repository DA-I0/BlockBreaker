using Godot;

public delegate void ScoreNotification(int score, int scoreMultiplier, int combo);
public delegate void StageClearScoreNotification(int score, int scoreMultiplier, int timeLeft, int perfectClearBonus);

namespace BoGK.Gameplay
{
	public partial class Score : Node
	{
		public readonly int MaxScoreMultiplier = 9;
		public readonly int ComboMultiplierStep = 10;
		public readonly int MaxComboMultiplier = 5;
		public readonly int PaddleSizeForMultiplier = 3;
		[Export] public int PerfectClearBonus = 50;
		[Export] public int GameWinBonus = 100;

		private int _currentScore = 0;

		private int _currentScoreMultiplier = 1;
		private int _comboChain = 0;

		private bool _isPerfect = true;

		private Timer _exitTimer;
		private GameSystem.SessionController refs;

		public event ScoreNotification ScoreChanged;
		public event StageClearScoreNotification StageCleared;
		public event Notification TimerStart;
		public event Notification TimerEnd;

		public int CurrentScore
		{
			get { return _currentScore; }
		}

		public int Multiplier
		{
			get { return _currentScoreMultiplier; }
		}

		public int TimeLeft
		{
			get { return (int)_exitTimer.TimeLeft; }
		}

		public void Cleanup()
		{
			_currentScore = 0;
			ResetMultiplier();
		}

		public void ChangeScore(int pointValue, bool increaseCombo = true)
		{
			if (increaseCombo)
			{
				_comboChain++;
			}

			UpdateMultiplier();
			_currentScore += pointValue * _currentScoreMultiplier;
			ScoreChanged?.Invoke(_currentScore, _currentScoreMultiplier, _comboChain);
			refs.audioController.PlayAudio(1);
		}

		public void ResetMultiplier()
		{
			_isPerfect = false;
			_comboChain = 0;
			_currentScoreMultiplier = 1;
			ScoreChanged?.Invoke(_currentScore, _currentScoreMultiplier, _comboChain);
		}

		public void InvokeExitTimer()
		{
			_exitTimer.Start();
			TimerStart?.Invoke();
		}

		public void AddBonusScore()
		{
			int timeLeft = TimeLeft;
			ChangeScore(TimeLeft, false);
			_exitTimer.Stop();
			TimerEnd?.Invoke();

			if (_isPerfect)
			{
				ChangeScore(PerfectClearBonus, false);
				StageCleared?.Invoke(_currentScore, _currentScoreMultiplier, timeLeft, PerfectClearBonus);
			}
			else
			{
				StageCleared?.Invoke(_currentScore, _currentScoreMultiplier, timeLeft, 0);
			}
		}

		public override void _Ready()
		{
			_exitTimer = GetNode<Timer>("ScoreTimer");
			refs = GetParent<GameSystem.SessionController>();
			refs.levelManager.ResetSession += SessionSetup;
			refs.levelManager.SceneChanged += ResetPerfectState;
			refs.health.ResetElements += ResetMultiplier;
			refs.GameStateChanged += AddEndScore;
		}

		private void SessionSetup()
		{
			Cleanup();
		}

		private void ResetPerfectState()
		{
			_isPerfect = true;
		}

		private void UpdateMultiplier()
		{
			int chainMultiplier = (_comboChain / ComboMultiplierStep) + 1;
			_currentScoreMultiplier = (chainMultiplier > MaxComboMultiplier) ? MaxComboMultiplier : chainMultiplier;

			_currentScoreMultiplier += (refs.paddle.Size < PaddleSizeForMultiplier) ? PaddleSizeForMultiplier - refs.paddle.Size : 0;
			_currentScoreMultiplier += (refs.paddle.MovementDirection < 0) ? 1 : 0;

			Ball ball = refs.Balls[0] as Ball;

			_currentScoreMultiplier += (ball.Size < 1) ? 1 : 0;
			_currentScoreMultiplier += (ball.SpeedMultiplier > 1) ? 1 : 0;

			CheckMultiplierRange();
		}

		private void CheckMultiplierRange()
		{
			if (_currentScoreMultiplier < 1)
			{
				_currentScoreMultiplier = 1;
				return;
			}

			if (_currentScoreMultiplier > MaxScoreMultiplier)
			{
				_currentScoreMultiplier = MaxScoreMultiplier;
			}
		}

		private void AddEndScore(GameState newState)
		{
			if (newState == GameState.gameWin)
			{
				ChangeScore(GameWinBonus, false);
			}
		}
	}
}