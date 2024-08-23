using UnityEngine;

public class TestDriveState : HangarManager.State
{
	public TestDriveState()
	{
		Vehicle vehicle = HangarManager.Instance.playerVehicle;
		vehicle.SetPlayingMode(true);
		vehicle.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
		vehicle.RestoreOriginal();
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			HangarManager.Instance.gameState = new BuildingState();
		}
	}
}