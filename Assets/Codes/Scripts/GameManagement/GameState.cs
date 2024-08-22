using UnityEngine;

public class GameState : GameManager.State
{
	public GameState()
	{
		Vehicle vehicle = GameManager.Instance.playerVehicle;
		vehicle.SetPlayingMode(true);
		vehicle.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
		vehicle.RestoreOriginal();
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GameManager.Instance.gameState = new BuildingState();
		}
	}
}