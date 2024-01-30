using Godot;

public delegate void PaddleStateNotification(PaddleState state);
public enum PaddleMode { basic, bouncy, sticky };
public enum PaddleState { idle, frozen, confused, locked, collisionLocked };

public partial class Paddle : CharacterBody2D
{
	private const float SizeTransitionTime = 0.25f;

	[Export] private int _baseMoveSpeed = 100;
	[Export] protected float _bouncyBoost = 1.5f;
	[Export] private int _positionY = 90;
	[Export] private Texture2D[] _sprites;

	protected PaddleMode _paddleMode = PaddleMode.basic;
	protected PaddleState _state = PaddleState.idle;

	private int _size = 1;

	private int _movementDirection = 1;
	private float inputHorizontal;

	Vector2 _inputDirection = Vector2.Zero;

	private NinePatchRect _sprite;
	private AnimationPlayer _animator;
	private Timer _timer;
	private SessionController refs;

	public event PaddleStateNotification StateChanged;

	public PaddleState PaddleState
	{
		get { return _state; }
	}

	public int Size
	{
		get { return _size; }
	}

	public int MovementDirection
	{
		get { return _movementDirection; }
		set { _movementDirection = value; }
	}

	public override void _Ready()
	{
		SetupReferences();
		SetupInitialValues();
	}

	public override void _PhysicsProcess(double delta)
	{
		GetMovement();
		Movement(delta);

		if (_state == PaddleState.collisionLocked)
		{
			SetPaddleState(PaddleState.idle);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (refs.CurrentGameState != GameState.gameplay)
		{
			return;
		}

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			inputHorizontal = eventMouseMotion.Relative.X * refs.settings.SpeedMouse;
		}
		else
		{
			switch (refs.settings.ActiveInputType)
			{
				case InputType.Joypad:
					if (HelperMethods.IsActiveInputDevice(refs, @event))
					{
						inputHorizontal = (Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left")) * refs.settings.SpeedController;
					}
					else
					{
						inputHorizontal = 0;
					}
					break;

				case InputType.Keyboard:
					inputHorizontal = (Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left")) * refs.settings.SpeedKeyboard;
					break;

				default:
					inputHorizontal = 0;
					return;
			}
		}
	}

	private void GetMovement()
	{
		if (refs.CurrentGameState != GameState.gameplay)
		{
			return;
		}

		_inputDirection = new Vector2(inputHorizontal, 0);

		if (refs.settings.ActiveInputType == InputType.Mouse)
		{
			inputHorizontal = 0;
		}
	}

	public void ChangeSize(int value)
	{
		_size += value;

		if (_size > refs.SelectedDifficulty.MaxPaddleSize)
		{
			_size = refs.SelectedDifficulty.MaxPaddleSize;
		}

		if (_size < refs.SelectedDifficulty.MinPaddleSize)
		{
			_size = refs.SelectedDifficulty.MinPaddleSize;
		}

		Resize();
	}

	public void SetPaddleMode(PaddleMode mode)
	{
		_paddleMode = mode;
		AdjustSprite();
	}

	public void SetPaddleState(PaddleState state, float length = -1)
	{
		_state = state;

		if (length > 0)
		{
			_timer.Start(length);
		}

		StateChanged?.Invoke(_state);
	}

	public virtual void ApplyPaddleEffect(Ball targetBall)
	{
		switch (_paddleMode)
		{
			case PaddleMode.bouncy:
				targetBall.SetTempSpeedMultiplier(_bouncyBoost);
				break;

			case PaddleMode.sticky:
				targetBall.StateReset();
				break;

			default:
				break;
		}

		VibrateController(0.5f, 0, 0.1f);
	}

	private void SetupReferences()
	{
		_sprite = GetNode("PaddleSprite") as NinePatchRect;
		_animator = GetNode("PaddleAnimator") as AnimationPlayer;
		_timer = GetNode("Timer") as Timer;

		refs = GetNode("/root/GameController") as SessionController;
		refs.paddle = this;
		refs.health.ResetElements += SetupInitialValues;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += Recenter;
	}

	private void SetupInitialValues()
	{
		_movementDirection = 1;
		_size = refs.SelectedDifficulty.StartPaddleSize;
		_timer.Stop();
		Resize();
		Recenter();
		SetPaddleMode(PaddleMode.basic);
		SetPaddleState(PaddleState.idle);
	}

	private void Recenter()
	{
		Position = new Vector2(0, _positionY);
		_state = PaddleState.idle;
	}

	private void Movement(double delta)
	{
		if (_state != PaddleState.idle)
		{
			return;
		}

		CalculateMoveVelocity(_inputDirection, _baseMoveSpeed);
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null)
		{
			VibrateController(0.05f, 0, 0.01f);
		}

		Position = new Vector2(Position.X, _positionY);
		_inputDirection = (refs.settings.ActiveInputType == InputType.Mouse) ? Vector2.Zero : _inputDirection;
	}

	private void CalculateMoveVelocity(Vector2 direction, int speed)
	{
		var newVelocity = Velocity;

		newVelocity.X = speed * direction.X * _movementDirection;
		Velocity = newVelocity;
	}

	private void Resize()
	{
		_animator.Play($"size_{_size}");//, SizeTransitionTime);
	}

	private void AdjustSprite()
	{
		if (_sprites.Length < (int)_paddleMode)
		{
			return;
		}

		_sprite.Texture = _sprites[(int)_paddleMode];
	}

	private void VibrateController(float strengthWeak, float strengthStrong, float time)
	{
		if (refs.settings.ControllerVibrations)
		{
			if (refs.settings.ActiveControllerID < 0)
			{
				Input.StartJoyVibration(0, strengthWeak, strengthStrong, time);
			}
			else
			{
				Input.StartJoyVibration(refs.settings.ActiveControllerID, strengthWeak, strengthStrong, time);
			}
		}
	}

	private void Destroy()
	{
		refs.paddle = null;
		refs.health.ResetElements -= SetupInitialValues;
		refs.levelManager.ResetSession -= Destroy;
		refs.levelManager.SceneChanged -= Recenter;
		QueueFree();
	}
}
