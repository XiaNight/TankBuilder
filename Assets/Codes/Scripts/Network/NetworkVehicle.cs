using UnityEngine;
using Mirror;

public class NetworkVehicle : NetworkBehaviour
{
	[SerializeField] Vehicle vehicle;
	public Vehicle Vehicle => vehicle;

	// SyncVar with a hook to detect changes
	[SyncVar(hook = nameof(OnJsonChanged))]
	private byte[] customizationJson;
	public void SetCustomizationJson(byte[] json)
	{
		vehicle.SetSerializedData(StringCompression.Decompress(json));
		customizationJson = json;
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();

		Debug.Log("OnStartAuthority");

		// Assign this vehicle to the local player's vehicle variable
		NetworkIdentity localPlayerIdentity = NetworkClient.connection.identity;
		if (localPlayerIdentity != null)
		{
			if (localPlayerIdentity.TryGetComponent<NetworkPlayer>(out var player))
			{
				player.SetVehicle(this);
				transform.SetParent(player.transform);
				transform.position = player.transform.position;
			}
		}

		// Enable the camera and any other client-specific components
		if (vehicle.VehicleCamera != null)
		{
			vehicle.VehicleCamera.enabled = true;
		}

		// Update the user interface
		UserUpdate.Instance.AddInterface(vehicle);
		vehicle.SetPlayingMode(true, true);
	}


	// Hook function called on clients when customizationJson changes
	[Client]
	private void OnJsonChanged(byte[] oldJson, byte[] newJson)
	{
		vehicle.SetSerializedData(StringCompression.Decompress(newJson));
	}

	public void SetPlayingMode(bool isPlaying)
	{
		if (isPlaying)
		{
			if (isServer)
			{
				// Enable physics on the server
				Rigidbody rb = vehicle.rb;
				if (rb != null)
				{
					rb.isKinematic = false;
					rb.useGravity = true;
				}
				// Other server-side setup if needed
			}

			if (isClient)
			{
				if (!isOwned)
				{
					// On non-authority clients, make Rigidbody kinematic to prevent physics simulation
					Rigidbody rb = vehicle.rb;
					if (rb != null)
					{
						rb.isKinematic = true;
					}
				}
				else
				{
					// On the owning client, enable input handling
					// Initialize input listeners or controls here
				}
				// Other client-side setup if needed
			}
		}
		else
		{
			// Disable physics or other teardown logic
		}
	}
}