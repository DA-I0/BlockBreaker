using Godot;

public partial class StaticPaddle : Paddle
{
	[Export] private CollisionPolygon2D _topCollider;
	[Export] private float _maxBounceAngle = 45f;

	public override void ApplyPaddleEffect(Ball targetBall)
	{
		ApplyBallRotation(targetBall);

		switch (_paddleMode)
		{
			case PaddleMode.bouncy:
				targetBall.ChangeTempSpeedMultiplier(_bouncyBoost);
				break;

			case PaddleMode.sticky:
				targetBall.StateReset();
				break;

			default:
				targetBall.AddVelocity(Velocity);
				break;
		}
	}

	private void ApplyBallRotation(Ball targetBall)
	{
		float positionDifference = Position.X - targetBall.Position.X;
		float paddleLength = Mathf.Abs(_topCollider.Polygon[0].X - _topCollider.Polygon[1].X);
		float newRotation = -90 - ((positionDifference / (paddleLength * 0.5f)) * _maxBounceAngle);
		targetBall.ChangeRotationTo(newRotation);
	}
}