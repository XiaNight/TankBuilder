using System.Collections.Generic;
using UnityEngine;

public class ToggleableCollection : Toggleable
{
	[SerializeField] private List<Toggleable> toggleables = new List<Toggleable>();

	public void Clear()
	{
		toggleables.Clear();
	}

	public void AssignToggleable(params Toggleable[] toggleable)
	{
		toggleables.AddRange(toggleable);
	}

	public override void Disable()
	{
		foreach (Toggleable toggleable in toggleables)
		{
			toggleable.Disable();
		}
	}

	public override void Enable()
	{
		foreach (Toggleable togglable in toggleables)
		{
			togglable.Enable();
		}
	}
}
