using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A state that players can edit their parts on the vehicle, tweak settings, modify parts, etc.
/// </summary>
public class EditState : HangarManager.State
{
	private Part selectedPart;
	public override void OnEnable(HangarManager hangarManager, HangarManager.State lastState)
	{
		base.OnEnable(hangarManager, lastState);

		hangar.SetModeText("Edit Mode");

		hangar.playerVehicle.SetPlayingMode(false);
		hangar.freeCam.enabled = true;
		hangar.vehicleCamera.enabled = false;
		hangar.partList.enabled = true;

		Builder.Instance.SetBuildingState(false);
		Builder.Instance.OnPartMousePressed += OnMousePressed;
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			hangar.SetGameState(new BuildingState());
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (selectedPart != null)
			{
				ClosePartSettingsUI();
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Builder.Instance.OnPartMousePressed -= OnMousePressed;
	}

	private void OnMousePressed(Part part, int mouseButton)
	{
		if (mouseButton == 0)
		{
			OpenPartSettingsUI(part);
		}
	}

	#region Part Settings UI

	private void OpenPartSettingsUI(Part part)
	{
		selectedPart = part;
		List<ISettingField> fieldSettings = part.OpenSettings();
		GenerateSettingsUI(fieldSettings);
		hangar.freeCam.enabled = false;
		Builder.Instance.enabled = false;
		Cursor.lockState = CursorLockMode.None;
	}

	private void ClosePartSettingsUI()
	{
		selectedPart = null;
		hangar.freeCam.enabled = true;
		Builder.Instance.enabled = true;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void GenerateSettingsUI(List<ISettingField> settings)
	{
		// Generate settings UI for the selected part
		PartFieldManager.Instance.SetupFields(settings);
	}

	#endregion
}