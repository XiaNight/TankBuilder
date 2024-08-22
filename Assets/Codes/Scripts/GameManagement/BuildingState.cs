using UnityEngine;

public class BuildingState : GameManager.State
{
	public BuildingState()
	{
		GameManager.Instance.playerVehicle.SetPlayingMode(false);
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKeyDown(KeyCode.T))
		{
			GameManager.Instance.gameState = new GameState();
		}
	}
}