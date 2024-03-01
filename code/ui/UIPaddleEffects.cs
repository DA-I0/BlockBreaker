using Godot;

public partial class UIPaddleEffects : Node2D
{
	private AnimationPlayer _effectAnimator;

	public override void _Ready()
	{
		_effectAnimator = GetNode("EffectAnimator") as AnimationPlayer;
		((Paddle)GetNode("..")).StateChanged += PlayEffect;
	}

	private void PlayEffect(PaddleState _state)
	{
		switch (_state)
		{
			case PaddleState.frozen:
				_effectAnimator.Play("effect_frozen");
				break;

			case PaddleState.confused:
				_effectAnimator.Play("effect_confused");
				break;

			default:
				_effectAnimator.Play("effect_idle");
				break;
		}
	}
}