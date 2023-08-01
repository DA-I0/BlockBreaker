using UnityEngine;

// NOTE: River thingy, doesn't really work atm
public class LevelHazard : MonoBehaviour
{
	[SerializeField] private Vector2 _rotateForce = Vector2.zero;
	private AreaEffector2D _hazard;

	private void Start()
	{
		_hazard = gameObject.GetComponent<AreaEffector2D>();
	}

	private void OnTriggerStay2D(Collider2D collider)
	{
		Vector3 rotationForce = new Vector3(0f, 0f, _hazard.forceAngle * 0.1f);
		collider.transform.RotateAround(collider.transform.position, Vector3.forward, _hazard.forceAngle * 0.1f);
	}
}
