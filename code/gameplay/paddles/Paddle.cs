using Godot;

public partial class Paddle : BasePaddle
{
	protected const float SizeTransitionTime = 0.25f;

	[Export] protected int _baseMoveSpeed = 100;
	[Export] protected float _bouncyBoost = 1.5f;
	[Export] protected int _positionY = 90;
	[Export] protected Texture2D[] _sprites;

	protected PaddleMode _paddleMode = PaddleMode.basic;
	protected PaddleState _state = PaddleState.idle;

	protected int _size = 1;

	protected int _movementDirection = 1;
	protected float inputHorizontal;

	protected Vector2 _inputDirection = Vector2.Zero;

	protected NinePatchRect _sprite;
	protected AnimationPlayer _animator;
	protected Timer _timer;
	protected SessionController refs;

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

	public override void ChangeSize(int value)
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

	public override void SetPaddleMode(PaddleMode mode)
	{
		_paddleMode = mode;
		AdjustSprite();
	}

	public override void SetPaddleState(PaddleState state, float duration = -1)
	{
		_state = state;

		if (duration > 0)
		{
			_timer.Start(duration);
		}

		StateChanged?.Invoke(_state);
	}

	public override void ApplyPaddleEffect(Ball targetBall)
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

	protected void SetupReferences()
	{
		_sprite = GetNode<NinePatchRect>("PaddleSprite");
		_animator = GetNode<AnimationPlayer>("PaddleAnimator");
		_timer = GetNode<Timer>("Timer");

		refs = GetNode<SessionController>("/root/GameController");
		refs.paddle = this;
		refs.health.ResetElements += SetupInitialValues;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += Recenter;
	}

	protected void SetupInitialValues()
	{
		_movementDirection = 1;
		_size = refs.SelectedDifficulty.StartPaddleSize;
		_timer.Stop();
		Resize();
		Recenter();
		SetPaddleMode(PaddleMode.basic);
		SetPaddleState(PaddleState.idle);
	}

	protected virtual void Recenter()
	{
		Position = new Vector2(0, _positionY);
		_state = PaddleState.idle;
	}

	protected void GetMovement()
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

	protected virtual void Movement(double delta)
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

	protected void CalculateMoveVelocity(Vector2 direction, int speed)
	{
		var newVelocity = Velocity;

		newVelocity.X = speed * direction.X * _movementDirection;
		Velocity = newVelocity;
	}

	protected void Resize()
	{
		_animator.Play($"size_{_size}", SizeTransitionTime);
	}

	protected virtual void AdjustSprite()
	{
		if (_sprites.Length < (int)_paddleMode)
		{
			return;
		}

		_sprite.Texture = _sprites[(int)_paddleMode];
	}

	public void VibrateController(float strengthWeak, float strengthStrong, float time) // TODO: move
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

	protected void Destroy()
	{
		refs.paddle = null;
		refs.health.ResetElements -= SetupInitialValues;
		refs.levelManager.ResetSession -= Destroy;
		refs.levelManager.SceneChanged -= Recenter;
		QueueFree();
	}
}
