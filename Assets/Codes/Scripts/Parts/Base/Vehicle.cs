using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Vehicle : MonoBehaviour
{
	public Rigidbody rb;
	public Contraption rootContraption;
	public Transform focusPoint;
	public UnityEvent<Bounds> OnCalculateBounds;

	[SerializeField] private Powertrain powertrain;

	private bool isPlaying = false;

	private void Awake()
	{
		rootContraption.SetAttachedVehicle(this);
		rootContraption.OnPartAdded += OnPartAdded;
		rootContraption.OnPartRemoved += OnPartRemoved;
	}

	private void Update()
	{
		if (!isPlaying) return;
		powertrain.MobilityUpdate();
	}

	private void OnDestroy()
	{
		if (rootContraption != null)
		{
			rootContraption.OnPartAdded -= OnPartAdded;
			rootContraption.OnPartRemoved -= OnPartRemoved;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		Debug.Log("Stay");
	}

	public void RestoreOriginal()
	{
		rootContraption.RestoreOriginal();
	}

	public void SetPlayingMode(bool isPlaying)
	{
		this.isPlaying = isPlaying;
		if (isPlaying)
		{
			rootContraption.OnPlay();
			rb.isKinematic = false;
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
		}
		else
		{
			rb.isKinematic = true;
			rootContraption.OnEndPlay();
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	private void OnPartAdded(Part part)
	{
		rootContraption.OnOtherPartAttached(part);
		if (part is PowerUnit powerUnit)
			powertrain.AddPowerUnit(powerUnit);
		if (part is IMovement movement)
			powertrain.Add(movement);
	}

	private void OnPartRemoved(Part part)
	{
		rootContraption.OnOtherPartRemoved(part);
		if (part is PowerUnit powerUnit) powertrain.RemovePowerUnit(powerUnit);
		if (part is IMovement movement) powertrain.Remove(movement);
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
	}
}