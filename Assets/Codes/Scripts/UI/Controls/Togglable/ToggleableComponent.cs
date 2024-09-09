using UnityEngine;

public class ComponentToggleable : Toggleable
{
	[SerializeField] private Behaviour component;
	[SerializeField] private bool invert;
	public override void Enable()
	{
		component.enabled = !invert;
	}

	public override void Disable()
	{
		component.enabled = invert;
	}
}