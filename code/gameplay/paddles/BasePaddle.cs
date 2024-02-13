using Godot;

public delegate void PaddleStateNotification(PaddleState state);
public enum PaddleMode { basic, bouncy, sticky };
public enum PaddleState { idle, frozen, confused, locked, collisionLocked };

public partial class BasePaddle : CharacterBody2D
{
	public virtual void ChangeSize(int value) { }
	public virtual void SetPaddleMode(PaddleMode mode) { }
	public virtual void SetPaddleState(PaddleState state, float duration = -1) { }
	public virtual void ApplyPaddleEffect(Ball targetBall) { }
}