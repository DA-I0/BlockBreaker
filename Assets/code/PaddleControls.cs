using UnityEngine;
using UnityEngine.UI;

public class PaddleControls : MonoBehaviour
{
	#region Variables
	private static float MinSizeMultiplier = 0.5f;
	private static float MaxSizeMultiplier = 2.0f;

	[SerializeField] private float _speedGeneral = 10f;
	[SerializeField] private float _speedMouse = 10f;
	[SerializeField] private float _sizeMultiplier = 1f;

	private SpriteRenderer _sprite;
	private Rigidbody2D _paddleRB;
	#endregion

	public void ResetPaddle()
	{
		_sizeMultiplier = 1f;
		ResizePaddle();
		transform.position = new Vector3(0f, transform.position.y, transform.position.z);
	}

	public void ChangePaddleSize(float multiplier)
	{
		_sizeMultiplier *= multiplier;

		if (_sizeMultiplier < MinSizeMultiplier)
		{
			_sizeMultiplier = MinSizeMultiplier;
		}

		if (_sizeMultiplier > MaxSizeMultiplier)
		{
			_sizeMultiplier = MaxSizeMultiplier;
		}

		ResizePaddle();
	}

	private void Awake()
	{
		// _speedMouse = _speed * 0.75f;
		_sprite = gameObject.GetComponent<SpriteRenderer>();
		_paddleRB = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void ResizePaddle()
	{
		_sprite.size = new Vector2(_sizeMultiplier, _sprite.size.y);
	}

	private void Move()
	{
		Vector2 paddleTransform;

		if (Input.GetAxis("Mouse X") != 0)
		{
			paddleTransform = Vector2.right * Input.GetAxis("Mouse X") * _speedMouse;
		}
		else
		{
			paddleTransform = Vector2.right * Input.GetAxis("Horizontal") * _speedGeneral;
		}

		_paddleRB.MovePosition(_paddleRB.position + paddleTransform * Time.deltaTime);
	}
}
