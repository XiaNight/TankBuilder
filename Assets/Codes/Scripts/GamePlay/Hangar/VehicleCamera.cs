using UnityEngine;

public class VehicleCamera : MonoBehaviour
{
	public Transform anchor;
	public Vector2 angles;
	public Vector2 screenAimingPosition;
	public LayerMask aimingRayCastLayers;
	public Transform aimingTarget;
	public Vector3 lookPos;

	private void Update()
	{
		//- Camera Rotation
		angles.x += Input.GetAxis("Mouse X");
		angles.y -= Input.GetAxis("Mouse Y");

		angles.y = Mathf.Clamp(angles.y, -90, 90);

		Camera.main.transform.SetPositionAndRotation(anchor.position, Quaternion.Euler(angles.y, angles.x, 0));
		Camera.main.transform.Translate(Vector3.back * 5);

		//- Aiming Raycast
		Ray ray = Camera.main.ViewportPointToRay(screenAimingPosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimingRayCastLayers))
		{
			lookPos = hit.point;
		}
		else
		{
			lookPos = ray.GetPoint(100);
		}
		aimingTarget.position = lookPos;
	}

	public void SetAnchorPosition(Bounds bounds)
	{
		anchor.position = bounds.center;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, lookPos);
		Gizmos.DrawSphere(lookPos, 0.1f);
	}
}