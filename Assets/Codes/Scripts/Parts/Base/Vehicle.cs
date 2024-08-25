using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	public Rigidbody rb;
	public Contraption rootContraption;

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
		}
		else
		{
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
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