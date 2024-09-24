using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class NetworkPlayer : NetworkBehaviour
{
	[SerializeField] private NetworkVehicle vehiclePrefab;
	private NetworkVehicle vehicle;
	public void SetVehicle(NetworkVehicle vehicle) => this.vehicle = vehicle;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		Debug.Log("Client started");

		if (LoadPlayerSavedVehicle(out string json))
		{
			// Print before and after compression in bytes
			byte[] compressed = StringCompression.Compress(json);
			Debug.Log($"Compression Before: {json.Length}, After: {compressed.Length}, Ratio: {compressed.Length / (float)json.Length}:.P2");
			CmdSpawnVehicle(compressed);
		}
	}

	[Command]
	public void CmdSpawnVehicle(byte[] compressedJson)
	{
		vehicle = Instantiate(vehiclePrefab, transform);
		vehicle.transform.position = transform.position;

		//TODO: Verify if the vehicle is valid
		NetworkServer.Spawn(vehicle.gameObject, connectionToClient);

		vehicle.SetCustomizationJson(compressedJson);
		vehicle.Vehicle.SetPlayingMode(true, true);
	}

	private bool LoadPlayerSavedVehicle(out string json)
	{
		if (!PlayerPrefs.HasKey("Vehicle"))
		{
			Debug.Log("No vehicle data found");
			json = null;
			return false;
		}

		json = PlayerPrefs.GetString("Vehicle");
		return true;
	}
}
