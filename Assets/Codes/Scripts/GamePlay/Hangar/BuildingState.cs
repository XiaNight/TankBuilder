using UnityEngine;

public class BuildingState : HangarManager.State
{
	private State state = State.Building;

	public BuildingState()
	{
		HangarManager.Instance.playerVehicle.SetPlayingMode(false);
		Builder.Instance.Enable();
		UpdateState();
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.T))
		{
			HangarManager.Instance.gameState = new TestDriveState();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			state = state switch
			{
				State.Building => State.UIFocused,
				State.UIFocused => State.Building,
				_ => state
			};

			UpdateState();
		}
	}

	private void UpdateState()
	{
		switch (state)
		{
			case State.Building:
				HangarManager.Instance.freeCam.enabled = true;
				Builder.Instance.enabled = true;
				Cursor.lockState = CursorLockMode.Locked;
				break;
			case State.UIFocused:
				HangarManager.Instance.freeCam.enabled = false;
				Builder.Instance.enabled = false;
				Cursor.lockState = CursorLockMode.None;
				break;
		}
	}

	public enum State
	{
		Building,
		UIFocused,
	}
}