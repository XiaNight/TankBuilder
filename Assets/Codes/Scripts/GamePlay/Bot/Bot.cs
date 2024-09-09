using UnityEngine;

public class Bot : MonoBehaviour
{
	public Vehicle vehicle;
	public string autoLoadJson;

	private void Start()
	{
		if (autoLoadJson != null)
		{
			vehicle.SetSerializedData(autoLoadJson);
			vehicle.SetPlayingMode(true);
		}
	}
}