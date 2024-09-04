using UnityEngine;

public class SaveVehicle : MonoBehaviour
{
	public Vehicle vehicle;

	private void Start()
	{
		Load();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			Save();
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			Load();
		}
	}

	private void Save()
	{
		// Save vehicle data
		string vehicleData = vehicle.GetSerializedData();
		PlayerPrefs.SetString("Vehicle", vehicleData);

		Debug.Log("Vehicle saved, data: " + vehicleData);
	}

	private void Load()
	{
		if (!PlayerPrefs.HasKey("Vehicle"))
		{
			Debug.Log("No vehicle data found");
			return;
		}

		// Load vehicle data
		string vehicleData = PlayerPrefs.GetString("Vehicle");
		vehicle.SetSerializedData(vehicleData);

		Debug.Log("Vehicle loaded, data: " + vehicleData);
	}
}