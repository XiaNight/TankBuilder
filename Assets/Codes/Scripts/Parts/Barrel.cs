using UnityEngine;

public class Barrel : Part
{
	[SerializeField] private Animator animator;
	[SerializeField] private float fireForce = 1000;
	private Rigidbody rb;

	private new void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (!isPlaying) return;
		if (Input.GetMouseButtonDown(0))
		{
			Fire();
		}
	}

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);
		animator.enabled = isPlaying;
		rb = GetComponentInParent<Rigidbody>();
	}

	public void Fire()
	{
		animator.SetTrigger("Fire");
		rb.AddForceAtPosition(-transform.forward * fireForce, transform.position, ForceMode.Impulse);
		Debug.Log("Fire!");
	}
}
