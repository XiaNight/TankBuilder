using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public bool IsOccupied => bind != null;
	private GameObject bind;

	public void Bind(GameObject bind)
	{
		this.bind = bind;
	}
}