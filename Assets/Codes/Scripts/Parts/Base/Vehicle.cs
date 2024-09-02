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
		rootContraption.SetPlayingState(isPlaying);
		rb.isKinematic = !isPlaying;

		if (isPlaying)
		{
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
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
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