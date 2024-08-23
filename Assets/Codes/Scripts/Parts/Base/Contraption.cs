using System.Collections.Generic;
using UnityEngine;

public class Contraption : Part
{
	[SerializeField] protected Transform content;
	[SerializeField] protected List<Part> parts;

	private Mount.State currentMountState = Mount.State.Disabled;

	private void Reset()
	{
		content = transform;
	}

	private void Start()
	{
		foreach (Part part in parts)
		{
			part.SetContraption(this);
		}
	}

	public override void SetPlayingState(bool state)
	{
		base.SetPlayingState(state);
		foreach (Part part in parts)
		{
			part.SetPlayingState(state);
		}
	}

	public override void SetMountState(Mount.State state)
	{
		currentMountState = state;
		base.SetMountState(state);
		foreach (Part part in parts)
		{
			part.SetMountState(currentMountState);
		}
	}

	public void AddPart(Part part)
	{
		if (!parts.Contains(part))
			parts.Add(part);
		part.transform.SetParent(content);
		part.SetContraption(this);
		part.SetMountState(currentMountState);
	}

	public void RemovePart(Part part)
	{
		if (parts.Contains(part))
			parts.Remove(part);
		part.transform.SetParent(null);
		part.SetContraption(null);
	}

	public override void RestoreOriginal()
	{
		foreach (Part part in parts)
		{
			part.RestoreOriginal();
		}
	}

	public override Contraption GetContraption()
	{
		return this;
	}

	public override float CalculateMass()
	{
		if (isMassCalculated) return calculatedMass;
		float mass = 0;
		foreach (Part part in parts)
		{
			mass += part.CalculateMass();
		}
		isMassCalculated = true;
		calculatedMass = mass;
		return calculatedMass;
	}
}