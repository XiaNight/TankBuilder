using UnityEngine;

/// <summary>
/// A state that the player is test driving the vehicle.
/// </summary>
public class TestDriveState : HangarManager.State
{
	public override void OnEnable(HangarManager hangarManager, HangarManager.State lastState)
	{
		base.OnEnable(hangarManager, lastState);

		hangar.SetModeText("Test Drive Mode");

		Vehicle vehicle = hangar.playerVehicle;
		vehicle.SetPlayingMode(true, true);
		vehicle.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
		vehicle.RestoreOriginal();

		Builder.Instance.Disable();
		hangar.freeCam.enabled = false;
		hangar.vehicleCamera.enabled = true;
		hangar.partList.enabled = false;

		Bot[] bots = Object.FindObjectsOfType<Bot>();
		foreach (Bot bot in bots)
		{
			bot.Vehicle.SetPlayingMode(true, true);
		}

		UserUpdate.Instance.AddInterface(hangar.playerVehicle);
	}

	public override void OnDisable()
	{
		base.OnDisable();

		UserUpdate.Instance.RemoveInterface(hangar.playerVehicle);
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			hangar.SetGameState(new BuildingState());
		}
	}
}