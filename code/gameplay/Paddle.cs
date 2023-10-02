using Godot;

public delegate void PaddleNotification(int size, int movementDirection);
public enum PaddleMode { basic, bouncy, sticky };

public partial class Paddle : CharacterBody2D
{
	[Export] private int _baseMoveSpeed = 100;
	[Export] private float _bouncyBoost = 1.5f;
	[Export] private int _positionY = 90;
	[Export] private Texture2D[] _sprites;

	private PaddleMode _paddleMode = PaddleMode.basic;

	private int _size = 1;

	public bool blockMovement = false;
	private int _movementDirection = 1;

	Vector2 _inputDirection = Vector2.Zero;

	private NinePatchRect _sprite;
	private AnimationPlayer _animator;
	private SessionController refs;

	public event PaddleNotification PaddleChanged;

	public int Size
	{
		get { return _size; }
	}

	public int MovementDirection
	{
		get { return _movementDirection; }
	}

	public override void _Ready()
	{
		SetupReferences();
		SetupInitialValues();
		Resize();
	}

	public override void _PhysicsProcess(double delta)
	{
		GetMovement();
		Movement(delta);
	}

	public override void _Input(InputEvent @event)
	{
		float inputHorizontal = 0;

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			inputHorizontal = eventMouseMotion.Relative.X * refs.settings.SpeedMouse;
		}

		_inputDirection = new Vector2(inputHorizontal, 0);
	}

	private void GetMovement()
	{
		float inputHorizontal;

		switch (refs.settings.ActiveController)
		{
			case InputType.gamepad:
				inputHorizontal = Input.GetJoyAxis(0, JoyAxis.LeftX);
				break;

			case InputType.keyboard:
				inputHorizontal = (Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left")) * refs.settings.SpeedKeyboard;
				break;

			default:
				return;
		}

		_inputDirection = new Vector2(inputHorizontal, 0);
	}

	public void OnAreaEntered(Area2D area)
	{
		//if (area is Ball ball)
		//{
		// Assign new direction
		//	ball.direction = new Vector2(_ballDir, ((float)new Random().NextDouble()) * 2 - 1).Normalized();
		//}
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

	private void SetupReferences()
	{
		_sprite = GetNode("Sprite") as NinePatchRect;
		_animator = GetNode("Animator") as AnimationPlayer;

		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.paddle = this;
		refs.health.ResetElements += SetupInitialValues;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += Recenter;
	}

	private void SetupInitialValues()
	{
		_movementDirection = 1;
		_size = refs.SelectedDifficulty.StartPaddleSize;
		Resize();
		Recenter();
		SetPaddleMode(PaddleMode.basic);
	}

	// NOTE: Potential fix for ball getting stuck inside the paddle, gotta test more.
	private void OnBallEnterPaddle(Node2D body)
	{
		Ball ball = (Ball)body;
		ball?.SetCollisionMaskValue(3, false);
	}


	private void OnBallExitPaddle(Node2D body)
	{
		Ball ball = (Ball)body;
		ball?.SetCollisionMaskValue(3, true);
	}

	private void Recenter()
	{
		Position = new Vector2(0, _positionY);
		blockMovement = false;
	}

	private void Movement(double delta)
	{
		if (blockMovement)
		{
			return;
		}

		CalculateMoveVelocity(_inputDirection, _baseMoveSpeed);
		var collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null)
		{
			Ball ball = collision.GetCollider() as Ball;

			if (ball != null) // doesn't trigger every time (trigger manually from the ball?)
			{
				PaddleModeAction(ball);
			}
			// ball?.AddVelocity(Velocity);
		}

		Position = new Vector2(Position.X, _positionY);
		_inputDirection = (refs.settings.ActiveController/*_controller*/ == InputType.mouse) ? Vector2.Zero : _inputDirection;
	}

	private void PaddleModeAction(Ball targetBall)
	{
		switch (_paddleMode)
		{
			case PaddleMode.bouncy:
				targetBall.SetTempBoost(_bouncyBoost);
				break;

			case PaddleMode.sticky:
				targetBall.Reset();
				break;

			default:
				targetBall.AddVelocity(Velocity);
				break;
		}
	}

	private void CalculateMoveVelocity(Vector2 direction, int speed)
	{
		var newVelocity = Velocity;

		newVelocity.X = speed * direction.X;
		Velocity = newVelocity;
	}

	private void Resize()
	{
		_animator.Play($"size_{_size}");
		PaddleChanged?.Invoke(_size, _movementDirection);
	}

	private void AdjustSprite()
	{
		if (_sprites.Length < (int)_paddleMode)
		{
			return;
		}

		_sprite.Texture = _sprites[(int)_paddleMode];
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
