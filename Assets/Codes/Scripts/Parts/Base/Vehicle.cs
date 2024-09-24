using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Vehicle : MonoBehaviour, IUserUpdate
{
	public Rigidbody rb;
	public Contraption rootContraption;
	public Transform focusPoint;
	public UnityEvent<Bounds> OnCalculateBounds;

	[SerializeField] private VehicleCamera vehicleCamera;
	public VehicleCamera VehicleCamera => vehicleCamera;
	[SerializeField] private Powertrain powertrain;
	public Powertrain Powertrain => powertrain;

	public List<IWeapon> Weapons { get; private set; } = new();
	public bool IsPlaying { get; private set; } = false;
	public bool IsUser { get; private set; } = false;
	public Vector3 CenterOfMass { get; private set; }
	public Vector3 CenterOfMassWorld => transform.TransformPoint(CenterOfMass);

	private void Awake()
	{
		rootContraption.SetAttachedVehicle(this);
		rootContraption.OnPartAdded += OnPartAdded;
		rootContraption.OnPartRemoved += OnPartRemoved;
	}

	private void Update()
	{
		if (IsPlaying) rootContraption.PlayUpdate();
	}

	private void FixedUpdate()
	{
		if (IsPlaying) rootContraption.PlayFixedUpdate();
	}

	void IUserUpdate.UserLoop()
	{
		if (!IsPlaying) return;
		powertrain.MobilityUpdate();
	}

	void IUserUpdate.SetFocusState(bool state)
	{
		IsUser = state;
	}

	private void OnDestroy()
	{
		if (rootContraption != null)
		{
			rootContraption.OnPartAdded -= OnPartAdded;
			rootContraption.OnPartRemoved -= OnPartRemoved;
		}
	}

	public void RestoreOriginal()
	{
		rootContraption.RestoreOriginal();
	}

	public void SetPlayingMode(bool isPlaying, bool physicsEnabled)
	{
		if (this.IsPlaying == isPlaying) return;
		this.IsPlaying = isPlaying;
		if (isPlaying)
		{
			rootContraption.OnPlay();
			rb.isKinematic = !physicsEnabled;
			rb.mass = rootContraption.CalculateMass();

			// Calculate bounds center
			Collider[] colliders = GetComponentsInChildren<Collider>();
			if (colliders.Length > 0)
			{
				Bounds bounds = colliders[0].bounds;
				foreach (Collider collider in colliders)
				{
					bounds.Encapsulate(collider.bounds);
				}
				OnCalculateBounds.Invoke(bounds);
			}
			RecalculateCenterOfMass();
		}
		else
		{
			rb.isKinematic = true;
			rootContraption.OnEndPlay();
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	/// <summary>
	/// lower center of mass by 70%
	/// </summary>
	private void RecalculateCenterOfMass()
	{
		rb.ResetCenterOfMass();
		CenterOfMass = rb.centerOfMass;
		rb.centerOfMass -= 0.5f * rb.centerOfMass.y * Vector3.up;
	}

	private void OnPartAdded(Part part)
	{
		rootContraption.OnOtherPartAttached(part);
		if (part is PowerUnit powerUnit)
			powertrain.AddPowerUnit(powerUnit);
		if (part is IMovement movement)
			powertrain.Add(movement);
		if (part is IWeapon weapon)
		{
			Weapons.Add(weapon);
		}
		RecalculateCenterOfMass();
	}

	private void OnPartRemoved(Part part)
	{
		rootContraption.OnOtherPartRemoved(part);
		if (part is PowerUnit powerUnit) powertrain.RemovePowerUnit(powerUnit);
		if (part is IMovement movement) powertrain.Remove(movement);
		if (part is IWeapon weapon) Weapons.Remove(weapon);
		RecalculateCenterOfMass();
	}

	public string GetSerializedData()
	{
		return rootContraption.Serialize().ToString();
	}

	public void SetSerializedData(string data)
	{
		JObject jObject = JObject.Parse(data);

		powertrain.Clear();

		rootContraption.ClearParts();

		rootContraption.Deserialize(jObject);

		rootContraption.OnSpawned();

		rb.mass = rootContraption.CalculateMass();
	}
}