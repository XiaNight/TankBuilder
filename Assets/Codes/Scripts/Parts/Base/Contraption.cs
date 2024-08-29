using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);
		foreach (Part part in parts)
		{
			part.SetPlayingState(isPlaying);
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
		part.SetAttachedContraption(this);
		part.SetMountState(currentMountState);
		part.SetAttachedVehicle(AttachedVehicle);
	}

	public void RemovePart(Part part)
	{
		if (parts.Contains(part))
			parts.Remove(part);
		part.transform.SetParent(null);
		part.SetAttachedContraption(null);
	}

	public void ClearParts()
	{
		for (int i = parts.Count - 1; i >= 0; i--)
		{
			Destroy(parts[i].gameObject);
		}
		parts.Clear();
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
		float mass = base.CalculateMass();
		foreach (Part part in parts)
		{
			mass += part.CalculateMass();
		}
		return mass;
	}

	#region Serialization

	public override JObject Serialize()
	{
		JObject data = base.Serialize();
		JArray partsData = new();
		foreach (Part part in parts)
		{
			partsData.Add(part.Serialize());
		}
		data.Add(nameof(parts), partsData);
		return data;
	}

	public override void Deserialize(JObject data)
	{
		base.Deserialize(data);

		JArray partsData = (JArray)data[nameof(parts)];
		foreach (JObject jData in partsData.Cast<JObject>())
		{
			string partType = jData["part_type"].Value<string>();
			PartData partData = PartManager.Instance.parts[partType];
			Part part = Instantiate(partData.prefab, content);
			part.SetMetaData(partData);
			AddPart(part);
			part.Deserialize(jData);
		}
	}

	#endregion
}