using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class Part : MonoBehaviour
{
	[Tooltip("Colliders for the main gameplay physics")]
	[SerializeField] private GameObject physicCollidersContainer;

	[Tooltip("Mounts for connecting parts")]
	[SerializeField] private GameObject mountsContainer;

	[Tooltip("Collider for user interaction")]
	[SerializeField] private GameObject interactionColliders;

	[Tooltip("Visual object for highlighting the part")]
	[SerializeField] private GameObject highlightVisual;

	public Contraption AttachedContraption { get; private set; }
	public Vehicle AttachedVehicle { get; private set; }

	public bool isPlaying { get; private set; } = false;
	protected List<Collider> physicColliders = new();
	protected List<Mount> mounts = new();

	protected void Awake()
	{
		if (mountsContainer != null) mounts = new List<Mount>(mountsContainer.GetComponentsInChildren<Mount>());
		if (physicCollidersContainer != null) physicColliders = new List<Collider>(physicCollidersContainer.GetComponentsInChildren<Collider>());
	}

	public Collider[] GetColliders()
	{
		return physicColliders.ToArray();
	}

	public bool ContainsMount(Mount mount)
	{
		return mounts.Contains(mount);
	}

	public virtual void SetPlayingState(bool isPlaying)
	{
		this.isPlaying = isPlaying;
		SetMountState(isPlaying ? Mount.State.Disabled : Mount.State.Enabled);
		if (isPlaying) SetHighlight(false);
		SetInteractionCollidersState(!isPlaying);
	}

	public void SetInteractionCollidersState(bool state)
	{
		if (interactionColliders == null) return;
		interactionColliders.SetActive(state);
	}

	public virtual void SetColliderState(bool isEnabled)
	{
		foreach (Collider collider in physicColliders)
		{
			collider.enabled = isEnabled;
		}
	}

	public virtual void SetMountState(Mount.State state)
	{
		foreach (Mount mount in mounts)
		{
			mount.SetState(state);
		}
	}

	/// <summary>
	/// This is not the same as AttachedContraption.
	/// </summary>
	public virtual Contraption GetContraption()
	{
		return AttachedContraption;
	}

	public void SetAttachedContraption(Contraption contraption)
	{
		if (AttachedContraption != null)
		{
			Contraption previousContraption = AttachedContraption;
			AttachedContraption = null;
			previousContraption.RemovePart(this);
		}
		AttachedContraption = contraption;
	}

	public void SetAttachedVehicle(Vehicle vehicle)
	{
		AttachedVehicle = vehicle;
	}

	public Mount[] FindMatchingMounts(Mount mount)
	{
		Vector3 mountDir = mount.transform.forward;
		List<Mount> matchingMounts = new();
		foreach (Mount m in mounts)
		{
			Vector3 mDir = m.transform.forward;
			if (Vector3.Dot(mDir, mountDir) <= -0.99f)
			{
				matchingMounts.Add(m);
			}
		}
		return matchingMounts.ToArray();
	}

	public virtual void RestoreOriginal() { }

	#region Metadata

	protected PartData MetaData { get; private set; }

	public event Action<PartData> OnMetaDataChangedEvent;

	public virtual void SetMetaData(PartData data)
	{
		MetaData = data;
		OnMetaDataChangedEvent?.Invoke(data);
	}

	#endregion

	public virtual float CalculateMass()
	{
		if (MetaData != null) return MetaData.mass;

		return 0;
	}

	public virtual void SetHighlight(bool isHighlighted)
	{
		if (highlightVisual == null) return;
		highlightVisual.SetActive(isHighlighted);
	}

	#region Interaction

	public event Action OnMouseEnterEvent;
	public event Action OnMouseExitEvent;
	public event Action OnMouseOverEvent;

	public virtual void OnMouseEnter()
	{
		OnMouseEnterEvent?.Invoke();
		if (!isPlaying) SetHighlight(true);
	}

	public virtual void OnMouseExit()
	{
		OnMouseExitEvent?.Invoke();
		if (!isPlaying) SetHighlight(false);
	}

	public virtual void OnMouseOver()
	{
		OnMouseOverEvent?.Invoke();
	}

	#endregion

	#region Serialization

	public virtual JObject Serialize()
	{
		string type = MetaData == null ? "root" : MetaData.partId;
		JObject data = new()
		{
			["part_type"] = type,
			["position"] = new JArray(new float[] { transform.position.x, transform.position.y, transform.position.z }),
			["rotation"] = new JArray(new float[] { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w }),
			["scale"] = new JArray(new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z })
		};

		return data;
	}

	public virtual void Deserialize(JObject data)
	{
		Vector3 position = new(
			data["position"][0].Value<float>(),
			data["position"][1].Value<float>(),
			data["position"][2].Value<float>()
		);
		Quaternion rotation = new(
			data["rotation"][0].Value<float>(),
			data["rotation"][1].Value<float>(),
			data["rotation"][2].Value<float>(),
			data["rotation"][3].Value<float>()
		);

		transform.SetPositionAndRotation(position, rotation);
		transform.localScale = new Vector3(
			data["scale"][0].Value<float>(),
			data["scale"][1].Value<float>(),
			data["scale"][2].Value<float>()
		);
	}

	#endregion
}