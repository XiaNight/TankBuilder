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

		if (lastState is TestDriveState)
		{
			Transform defaultTransform = hangar.hangarCameraDefaultPosition;
			Camera.main.transform.SetPositionAndRotation(defaultTransform.position, defaultTransform.rotation);
		}

		hangar.SetModeText("Building Mode");

		//- Reset vehicle
		hangar.playerVehicle.SetPlayingMode(false);
		hangar.freeCam.enabled = true;
		hangar.vehicleCamera.enabled = false;
		hangar.partList.enabled = true;

		//- Setup Builder
		Builder.Instance.Enable();
		Builder.Instance.SetSelectedPartData(hangar.partList.GetSelectedPartData());
		Builder.Instance.OnPartMousePressed += OnMousePressed;
		Builder.Instance.SetBuildingState(true);

		//- Part selection
		hangar.partList.gameObject.SetActive(true);
		hangar.partList.OnPartSelectedEvent += OnPartSelected;

		UpdateState();
	}

	public override void OnDisable()
	{
		base.OnDisable();

		hangar.partList.gameObject.SetActive(false);
		hangar.partList.OnPartSelectedEvent -= OnPartSelected;

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
			// switch (state)
			// {
			// 	case State.Building:
			// 		Builder.Instance.ClearSelectedPartData();
			// 		state = State.UIFocused;
			// 		break;
			// 	case State.UIFocused:
			// 		Builder.Instance.SetSelectedPartData(hangar.partList.GetSelectedPartData());
			// 		state = State.Building;
			// 		break;
			// }

			UpdateState();
		}
	}

	private void OnMousePressed(Part part, int mouseBtn)
	{
		switch (mouseBtn)
		{

		}
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