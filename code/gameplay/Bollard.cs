using Godot;
using System;

public partial class Bollard : Node
{
	[Export] private bool _isActived = false;
	[Export] private float _stateChangeCooldown = 1f;

	private AnimationPlayer _animator;
	private Timer _timer;

	public override void _Ready()
	{
		_animator = (AnimationPlayer)GetChild(2);
		_timer = (Timer)GetChild(3);
		SetInitialState();
	}

	private void SetInitialState()
	{
		if (_isActived)
		{
			_animator.Play("idle_enabled");
		}
		else
		{
			_animator.Play("idle_disabled");
		}
	}

	private void ToggleState()
	{
		if (_isActived)
		{
			_animator.Play("deactivate");
			_animator.Queue("idle_disabled");
		}
		else
		{
			_animator.Play("activate");
			_animator.Queue("idle_enabled");
		}
	}

	private void StartTimer(StringName animationName)
	{
		if (animationName.ToString().Contains("idle_"))
		{
			_timer.Start(_stateChangeCooldown);
		}
	}
}
