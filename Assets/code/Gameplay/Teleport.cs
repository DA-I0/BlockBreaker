using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField] private bool _isActive;
	[SerializeField] private GameObject _door;
	[SerializeField] private Transform _linkedDoor;
	[SerializeField] private float _ballDisplacement = 0.6f;
	[SerializeField] private float _reactivationDelay = 3f;
	private BoxCollider2D _trigger;

	public void UpdateDoorState(bool setActive, bool updateLinked = true)
	{
		_isActive = setActive;
		_door.SetActive(!_isActive);
		_trigger.enabled = _isActive;

		if (updateLinked)
		{
			_linkedDoor.GetComponent<Teleport>().UpdateDoorState(_isActive, false);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (_isActive && collider.gameObject.tag == "ball")
		{
			Transform ball = collider.transform;

			float angleChange = -(_linkedDoor.eulerAngles.z - transform.eulerAngles.z);
			ball.GetComponent<BallController>().RotateBall(angleChange);

			_linkedDoor.GetComponent<Teleport>().UpdateDoorState(false, false);
			ball.position = _linkedDoor.position - (_linkedDoor.up * _ballDisplacement);

			UpdateDoorState(false, false);
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (!_isActive && collider.gameObject.tag == "ball")
		{
			Invoke("DelayActivation", _reactivationDelay);
		}
	}

	private void DelayActivation()
	{
		UpdateDoorState(true);
	}

	private void Awake()
	{
		_trigger = gameObject.GetComponent<BoxCollider2D>();

		if (_linkedDoor == null)
		{
			_linkedDoor = transform;
		}
	}

	private void Start()
	{
		UpdateDoorState(true);
	}
}
