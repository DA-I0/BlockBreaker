using Godot;

namespace BoGK.UI
{
	public partial class UIPaddleEffects : Node2D // should it be in UI?
	{
		private AnimationPlayer _effectAnimator;

		public override void _Ready()
		{
			_effectAnimator = GetNode<AnimationPlayer>("EffectAnimator");
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
}