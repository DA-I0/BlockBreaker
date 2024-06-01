using Godot;

namespace BoGK.Gameplay
{
	public partial class DoublePaddle : Paddle
	{
		[Export] private CharacterBody2D _secondaryPaddle;
		[Export] private NinePatchRect _secondarySprite;
		[Export] private Texture2D[] _secondarySprites;

		protected override void Recenter()
		{
			Position = new Vector2(0, _positionY);
			_secondaryPaddle.Position = Vector2.Zero;
			_state = PaddleState.idle;
		}

		protected override void Movement(double delta)
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

			float horizontalPosition = (Position.X > 0) ? 0 : Position.X;

			Position = new Vector2(horizontalPosition, _positionY);
			_secondaryPaddle.Position = new Vector2(-horizontalPosition * 2, 0);
			_inputDirection = (refs.settings.ActiveInputType == InputType.Mouse) ? Vector2.Zero : _inputDirection;
		}

		protected override void AdjustSprite()
		{
			if (_sprites.Length < (int)_paddleMode)
			{
				return;
			}

			_sprite.Texture = _sprites[(int)_paddleMode];
			_secondarySprite.Texture = _secondarySprites[(int)_paddleMode];
		}
	}
}