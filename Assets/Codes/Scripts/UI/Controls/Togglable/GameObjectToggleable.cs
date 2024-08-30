using UnityEngine;

public class GameObjectToggleable : Toggleable
{
	[SerializeField] private bool invert;
	public override void Enable()
	{
		gameObject.SetActive(!invert);
	}

	public override void Disable()
	{
		gameObject.SetActive(invert);
	}
}