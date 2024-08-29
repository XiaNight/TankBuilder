using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Vehicle : MonoBehaviour
{
	public Rigidbody rb;
	public Contraption rootContraption;
	public Transform focusPoint;
	public UnityEvent<Bounds> OnCalculateBounds;

	private void Awake()
	{
		rootContraption.SetAttachedVehicle(this);
	}

	public void RestoreOriginal()
	{
		rootContraption.RestoreOriginal();
	}

	public void SetPlayingMode(bool isPlaying)
	{
		rootContraption.SetPlayingState(isPlaying);
		rb.isKinematic = !isPlaying;

		if (isPlaying)
		{
			rb.mass = rootContraption.CalculateMass();
			var jObject = rootContraption.Serialize();

			Debug.Log(jObject.ToString());

			// Calculate bounds center
			Bounds bounds = new();
			Collider[] colliders = GetComponentsInChildren<Collider>();
			foreach (Collider collider in colliders)
			{
				bounds.Encapsulate(collider.bounds);
			}
			OnCalculateBounds.Invoke(bounds);
		}
		else
		{
			transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	public string GetSerializedData()
	{
		return rootContraption.Serialize().ToString();
	}

	public void SetSerializedData(string data)
	{
		JObject jObject = JObject.Parse(data);

		rootContraption.ClearParts();

		rootContraption.Deserialize(jObject);
	}
}