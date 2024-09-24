using UnityEngine;
using Mirror;

public class NetworkManagerVehicleBuilder : NetworkManager
{
	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

		foreach (SpawnPoint spawnPoint in spawnPoints)
		{
			if (!spawnPoint.IsOccupied)
			{
				GameObject player = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
				NetworkServer.AddPlayerForConnection(conn, player);
				spawnPoint.Bind(player);
				break;
			}
		}
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();
	}
}