using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using OutlineFx;

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
	[SerializeField] private Outline[] highlightOutlines;

	[SerializeField]
	private RotationRestriction rotationRestriction = RotationRestriction.None;

	public Contraption AttachedContraption { get; private set; }
	public Vehicle AttachedVehicle { get; private set; }

	protected List<Collider> physicColliders = new();
	protected List<Mount> mounts = new();

	public Mount[] Mounts => mounts.ToArray();

	private void Reset()
	{
		highlightOutlines = GetComponentsInChildren<Outline>();
	}

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

	/// <summary>
	/// On part is spawned (instantiated and called after deserialize).
	/// </summary>
	public virtual void OnSpawned()
	{

	}

	/// <summary>
	/// When the placement of the part changed.
	/// </summary>
	public virtual void OnPlacementChanged()
	{

	}

	/// <summary>
	/// When the part is first placed down or spawned.
	/// </summary>
	public virtual void OnAttached()
	{

	}

	/// <summary>
	/// When the part is removed from it's vehicle.
	/// </summary>
	public virtual void OnRemoved()
	{

	}

	/// <summary>
	/// On other part attached to this vehicle.
	/// </summary>
	/// <param name="part"> The part that has been attached.</param>
	public virtual void OnOtherPartAttached(Part part)
	{

	}

	/// <summary>
	/// On other part removed from this vehicle.
	/// </summary>
	/// <param name="part"> The part that has been removed.</param>
	public virtual void OnOtherPartRemoved(Part part)
	{

	}

	public virtual void OnPlay()
	{
		SetMountState(Mount.State.Disabled);
		SetHighlight(false);
		SetInteractionCollidersState(false);
	}

	public virtual void OnEndPlay()
	{
		SetMountState(Mount.State.Enabled);
		SetInteractionCollidersState(true);
	}

	public virtual void PlayUpdate()
	{

	}

	public virtual void PlayFixedUpdate()
	{

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

	public Mount[] FindMatchingMounts(Vector3 mountDir)
	{
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

	public virtual void SetHighlight(bool isHighlighted, Color color = default)
	{
		if (highlightVisual == null) return;
		highlightVisual.SetActive(isHighlighted);

		foreach (Outline outline in highlightOutlines)
		{
			outline.enabled = isHighlighted;
			outline.Color = color;
		}
	}

	public virtual List<ISettingField> OpenSettings()
	{
		return new List<ISettingField>();
	}

	#region Interaction

	public event Action OnMouseEnterEvent;
	public event Action OnMouseExitEvent;
	public event Action OnMouseOverEvent;

	public virtual void OnMouseEnter()
	{
		OnMouseEnterEvent?.Invoke();
		if (!AttachedVehicle.IsPlaying)
		{
			AttachedContraption.SetHighlight(true, Color.green);
			SetHighlight(true, Color.yellow);
		}
	}

	public virtual void OnMouseExit()
	{
		OnMouseExitEvent?.Invoke();
		if (!AttachedVehicle.IsPlaying)
		{
			AttachedContraption.SetHighlight(false);
			SetHighlight(false);
		}
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
		Vector3 localPosition = AttachedVehicle.transform.InverseTransformPoint(transform.position);
		Quaternion localRotation = Quaternion.Inverse(AttachedVehicle.transform.rotation) * transform.rotation;

		JObject data = new()
		{
			["part_type"] = type,
			["position"] = new JArray(new float[] { localPosition.x, localPosition.y, localPosition.z }),
			["rotation"] = new JArray(new float[] { localRotation.x, localRotation.y, localRotation.z, localRotation.w }),
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
		position = AttachedVehicle.transform.TransformPoint(position);

		Quaternion rotation = new(
			data["rotation"][0].Value<float>(),
			data["rotation"][1].Value<float>(),
			data["rotation"][2].Value<float>(),
			data["rotation"][3].Value<float>()
		);
		rotation = AttachedVehicle.transform.rotation * rotation;

		transform.SetPositionAndRotation(position, rotation);
		transform.localScale = new Vector3(
			data["scale"][0].Value<float>(),
			data["scale"][1].Value<float>(),
			data["scale"][2].Value<float>()
		);
	}

	protected static T TryParseData<T>(JObject data, string key)
	{
		if (data.TryGetValue(key, out JToken value))
		{
			return value.Value<T>();
		}
		return default;
	}

	#endregion

	[Flags]
	public enum RotationRestriction
	{
		None = 0,
		X = 1,
		Y = 2,
		XY = 3,
		Z = 4,
		XZ = 5,
		YZ = 6,
		XYZ = 7
	}
}