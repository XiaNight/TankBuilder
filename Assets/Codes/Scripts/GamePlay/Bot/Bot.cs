using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
	[SerializeField] private Vehicle vehicle;
	public Vehicle Vehicle => vehicle;
	[SerializeField] private string autoLoadJson;
	[SerializeField] private Transform focusPoint;
	[SerializeField] private float accuracy = 0.5f;
	[SerializeField] private float accuracyFreq = 0.1f;

	[Header("Player Detection")]
	[SerializeField] private float detectionRadius = 100;
	[SerializeField] private float detectionRate = 0.5f;
	[SerializeField] private LayerMask detectionRaycastLayer;
	private bool hasDirectSight = false;
	private Vector3 sightPosition;


	[Header("Navigation")]
	[SerializeField] private AnimationCurve angleToSteer;
	[SerializeField] private AnimationCurve angleToThrottle;
	[SerializeField] private AnimationCurve angleSpeedCurve;
	[SerializeField] private float throttleMultiplier = 1;
	[SerializeField] private float navigationUpdateRate = 0.1f;

	private Vehicle userVehicle = null;
	private Vector3[] navMeshTrace;

	private void Start()
	{
		if (autoLoadJson != null)
		{
			vehicle.SetSerializedData(autoLoadJson);
			// vehicle.SetPlayingMode(true);
		}

		Vehicle[] vehicles = FindObjectsOfType<Vehicle>();
		foreach (Vehicle vehicle in vehicles)
		{
			if (vehicle.CompareTag("Player"))
			{
				userVehicle = vehicle;
				break;
			}
		}

		if (userVehicle != null)
		{
			StartCoroutine(PathUpdater());
			StartCoroutine(AutoPilot());
			StartCoroutine(DetectPlayer());
		}
	}

	private void Update()
	{
		if (userVehicle != null)
			Debug.DrawLine(vehicle.transform.position, userVehicle.transform.position, Color.red);
	}

	private void FixedUpdate()
	{
		if (userVehicle == null) return;

		Vector3 inaccuracy = PerlinRandomSphere(accuracyFreq) * accuracy;
		Vector3 targetPosition = userVehicle.transform.TransformPoint(userVehicle.CenterOfMass);
		Vector3 targetDirection = (targetPosition - vehicle.transform.position).normalized;
		float distance = Vector3.Distance(vehicle.transform.position, targetPosition);

		focusPoint.position = (targetDirection + inaccuracy) * distance + vehicle.transform.position;

		// if (hasDirectSight)
		// {
		// 	foreach (IWeapon weapon in vehicle.Weapons)
		// 	{
		// 		if (weapon.IsAimed(focusPoint.position) && weapon.IsLoaded)
		// 		{
		// 			weapon.Fire();
		// 		}
		// 	}
		// }
	}

	private float CompareDistance(Vector3 A, Vector3 B)
	{
		return A.sqrMagnitude - B.sqrMagnitude;
	}

	private IEnumerator DetectPlayer()
	{
		while (true)
		{
			yield return new WaitForSeconds(detectionRate);

			hasDirectSight = false;
			// Ray cast to player vehicle
			Vector3 selfPos = vehicle.CenterOfMassWorld;
			Vector3 playerPos = userVehicle.CenterOfMassWorld;
			Ray ray = new(selfPos, playerPos - selfPos);
			if (Physics.Raycast(ray.GetPoint(5), ray.direction, out RaycastHit hit, detectionRadius, detectionRaycastLayer))
			{
				sightPosition = hit.point;
				if (hit.transform.CompareTag("Player"))
				{
					hasDirectSight = true;
				}
			}
			else
			{
				sightPosition = ray.GetPoint(detectionRadius);
			}

			if (CompareDistance(sightPosition - selfPos, playerPos - selfPos) >= 0)
			{
				hasDirectSight = true;
			}
		}
	}

	private IEnumerator PathUpdater()
	{
		if (userVehicle == null) yield break;
		while (true)
		{
			FindPathTo(userVehicle.transform.position);
			yield return new WaitForSeconds(navigationUpdateRate);
		}
	}

	private IEnumerator AutoPilot()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.05f);

			//Determin next point is on the right or left
			Vector3 forward = vehicle.transform.forward;
			Vector3 toTarget = GetNextPointOnPath(5) - vehicle.transform.position;
			float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);
			float unsignedAngle = Mathf.Abs(angle);
			float side = Mathf.Sign(angle);
			float steering = angleToSteer.Evaluate(unsignedAngle) * side;
			float throttle = angleToThrottle.Evaluate(unsignedAngle) * throttleMultiplier;
			float speed = vehicle.rb.velocity.magnitude * 3.6f; // m/s to km/h
			float speedLimit = angleSpeedCurve.Evaluate(unsignedAngle);

			// throttle = (speedLimit - speed + 10) / 10 * throttle;
			// Debug.Log($"Speed: {speed} Speed Limit: {speedLimit} Throttle: {throttle}");

			//Determin next point is in front or back
			if (Vector3.Dot(vehicle.transform.forward, toTarget) > 0)
			{
				vehicle.Powertrain.SetThrottle(throttle);
				vehicle.Powertrain.SetSteer(steering);
			}
			else
			{
				vehicle.Powertrain.SetThrottle(-throttle);
				vehicle.Powertrain.SetSteer(-steering);
			}

			if (speed > speedLimit)
			{
				vehicle.Powertrain.SetThrottle(0);
				vehicle.Powertrain.SetBrake(1);
			}
			else
			{
				vehicle.Powertrain.SetBrake(0);
			}
		}
	}

	private Vector3 GetNextPointOnPath(float onNext)
	{
		if (navMeshTrace == null || navMeshTrace.Length < 2) return vehicle.transform.position;
		Vector3 traceDiff = navMeshTrace[1] - navMeshTrace[0];
		Vector3 onLinePos = Vector3.Project(vehicle.transform.position - navMeshTrace[0], traceDiff);

		return TraceTo(0, onNext + onLinePos.magnitude);
	}

	private Vector3 TraceTo(int index, float onNext)
	{
		Vector3 from = navMeshTrace[index];
		int nextIndex = index + 1;

		//- Distance is less than next point and no more points
		if (navMeshTrace.Length <= nextIndex) return from;

		Vector3 to = navMeshTrace[nextIndex];
		Vector3 nextPosDiff = to - from;

		//- Distance is greater than next point
		float nextDistance = nextPosDiff.magnitude;
		if (nextDistance > onNext)
		{
			return from + nextPosDiff.normalized * onNext;
		}

		//- Distance is less than next point
		return TraceTo(nextIndex, onNext - nextDistance);
	}

	private void FindPathTo(Vector3 targetPosition)
	{
		NavMeshPath path = new();
		NavMesh.CalculatePath(vehicle.transform.position, targetPosition, NavMesh.AllAreas, path);
		navMeshTrace = path.corners;
	}

	private void OnDrawGizmos()
	{
		if (navMeshTrace == null) return;

		for (int i = 0; i < navMeshTrace.Length - 1; i++)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(navMeshTrace[i], navMeshTrace[i + 1]);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(GetNextPointOnPath(5), 0.2f);


		//- Detection
		if (userVehicle == null) return;
		Vector3 selfPos = vehicle.CenterOfMassWorld;
		Vector3 playerPos = userVehicle.CenterOfMassWorld;
		Gizmos.color = hasDirectSight ? Color.green : Color.red;
		Gizmos.DrawLine(selfPos, playerPos);

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(sightPosition, 0.2f);
	}

	private Vector3 PerlinRandomSphere(float scale = 1)
	{
		float x = Mathf.PerlinNoise(Time.time * scale, 0) * 2 - 1;
		float y = Mathf.PerlinNoise(0, Time.time * scale) * 2 - 1;
		float z = Mathf.PerlinNoise(Time.time * scale, Time.time * scale) * 2 - 1;
		return new Vector3(x, y, z).normalized;
	}
}