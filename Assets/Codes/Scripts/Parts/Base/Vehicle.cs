using System.Collections.Generic;
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
		}
		else
		{
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}
	}
}