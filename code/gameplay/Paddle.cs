using Godot;

public delegate void PaddleNotification(int size, int movementDirection);

public partial class Paddle : CharacterBody2D
{
	private const int MoveSpeed = 100;
	private const int PositionX = 128;
	private const int PositionY = 207;

	private int _size = 1;

	private bool _mouseControlls = false;
	public bool blockMovement = false;
	private int _movementDirection = 1;

	Vector2 _inputDirection = Vector2.Zero;

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
		Movement(delta);
	}

	public override void _Input(InputEvent @event)
	{
		float inputHorizontal;

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			inputHorizontal = eventMouseMotion.Relative.X * refs.settings.SpeedMouse;
			_mouseControlls = true;
		}
		else
		{
			inputHorizontal = (Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left")) * refs.settings.SpeedKeyboard;
			_mouseControlls = false;
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

	private void SetupReferences()
	{
		_animator = GetNode("Animator") as AnimationPlayer;
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		refs.paddle = this;
		refs.health.ResetElements += SetupInitialValues;
		refs.levelManager.ResetSession += Destroy;
		refs.levelManager.SceneChanged += Recenter;
	}

	private void SetupInitialValues()
	{
		Recenter();
		_size = refs.SelectedDifficulty.StartPaddleSize;
		Resize();
		_movementDirection = 1;
		// SetPaddleMode(PaddleMode.basic);
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
		Position = new Vector2(PositionX, PositionY);
		blockMovement = false;
	}

	private void Movement(double delta)
	{
		if (blockMovement)
		{
			return;
		}

		CalculateMoveVelocity(_inputDirection, MoveSpeed);
		var collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null)
		{
			Ball ball = collision.GetCollider() as Ball;
			ball?.AddVelocity(Velocity);
		}

		Position = new Vector2(Position.X, PositionY);
		_inputDirection = _mouseControlls ? Vector2.Zero : _inputDirection;
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

	private void Destroy()
	{
		refs.paddle = null;
		refs.health.ResetElements -= SetupInitialValues;
		refs.levelManager.ResetSession -= Destroy;
		refs.levelManager.SceneChanged -= Recenter;
		QueueFree();
	}
}
