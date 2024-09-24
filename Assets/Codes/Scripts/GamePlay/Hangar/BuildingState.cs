using UnityEngine;

/// <summary>
/// A state that the player is activly adding and removing parts from the vehicle.
/// </summary>
public class BuildingState : HangarManager.State
{
	private State state = State.Building;

	public override void OnEnable(HangarManager hangarManager, HangarManager.State lastState)
	{
		base.OnEnable(hangarManager, lastState);

		if (lastState is TestDriveState or null)
		{
			Transform defaultTransform = hangar.hangarCameraDefaultPosition;
			Camera.main.transform.SetPositionAndRotation(defaultTransform.position, defaultTransform.rotation);
		}

		hangar.SetModeText("Building Mode");

		//- Reset vehicle
		hangar.playerVehicle.SetPlayingMode(false, false);
		hangar.freeCam.enabled = true;
		hangar.vehicleCamera.enabled = false;
		hangar.partList.enabled = true;

		//- Setup Builder
		Builder.Instance.Enable();
		Builder.Instance.SetSelectedPartData(hangar.partList.GetSelectedPartData());
		Builder.Instance.OnPartMousePressed += OnMousePressed;
		Builder.Instance.SetBuildingState(true);

		//- Part selection
		hangar.partList.OnPartSelectedEvent += OnPartSelected;

		hangar.buildModeToggleGroup.Enable();

		Bot[] bots = Object.FindObjectsOfType<Bot>();
		foreach (Bot bot in bots)
		{
			bot.Vehicle.SetPlayingMode(false, false);
		}

		UpdateState();
	}

	public override void OnDisable()
	{
		base.OnDisable();

		hangar.partList.OnPartSelectedEvent -= OnPartSelected;

		hangar.buildModeToggleGroup.Disable();

		Builder.Instance.OnPartMousePressed -= OnMousePressed;
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.T))
		{
			hangar.SetGameState(new TestDriveState());
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			hangar.SetGameState(new EditState());
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			string json = hangar.playerVehicle.GetSerializedData();
			Debug.Log(json);
			// copy to clipboard
			TextEditor te = new();
			te.text = json;
			te.SelectAll();
			te.Copy();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			switch (state)
			{
				case State.Building:
					Builder.Instance.RemovePreview();
					state = State.UIFocused;
					break;
				case State.UIFocused:
					Builder.Instance.SetSelectedPartData(hangar.partList.GetSelectedPartData());
					state = State.Building;
					break;
			}

			UpdateState();
		}
	}

	private void OnMousePressed(Part part, int mouseBtn)
	{

	}

	private void OnPartSelected(PartData partData)
	{
		Builder.Instance.SetSelectedPartData(partData);
	}

	private void UpdateState()
	{
		switch (state)
		{
			case State.Building:
				hangar.freeCam.enabled = true;
				Builder.Instance.enabled = true;
				Cursor.lockState = CursorLockMode.Locked;
				break;
			case State.UIFocused:
				hangar.freeCam.enabled = false;
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