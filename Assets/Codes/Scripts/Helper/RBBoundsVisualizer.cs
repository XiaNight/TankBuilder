using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RBBoundsVisualizer : MonoBehaviour
{
	public Rigidbody rb;

	private void Reset()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
}