using UnityEngine;

public class Hazard_Automatic : MonoBehaviour
{
	[SerializeField] private bool isActivated = false;
	[SerializeField] private float _stateChangeDelay = 1f;

	private Animator _animator;

	private void Awake()
	{
		_animator = gameObject.GetComponent<Animator>();
		SetStartingAnimation();
	}

	private void SetStartingAnimation()
	{
		if (isActivated)
		{
			_animator.Play("Base Layer.idle_active", 0, 0.001f);
		}
		else
		{
			_animator.Play("Base Layer.idle", 0, 0.001f);
		}

		Invoke("ToggleState", _stateChangeDelay);
	}

	private void ToggleState()
	{
		if (isActivated)
		{
			_animator.Play("Base Layer.deactivate", 0, 0);

		}
		else
		{
			_animator.Play("Base Layer.activate", 0, 0);
		}
	}
}
