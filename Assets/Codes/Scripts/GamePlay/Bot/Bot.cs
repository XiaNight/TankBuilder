using UnityEngine;

public class Bot : MonoBehaviour
{
	[SerializeField] private Vehicle vehicle;
	[SerializeField] private string autoLoadJson;
	[SerializeField] private Transform focusPoint;

	private Vehicle userVehicle = null;

	private void Start()
	{
		if (autoLoadJson != null)
		{
			vehicle.SetSerializedData(autoLoadJson);
			vehicle.SetPlayingMode(true);
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
	}

	private void FixedUpdate()
	{
		if (userVehicle == null) return;

		vehicle.Powertrain.SetBrake(1);

		focusPoint.position = userVehicle.transform.TransformPoint(userVehicle.rb.centerOfMass);
	}
}