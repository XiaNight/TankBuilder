using UnityEngine;

public class Barrel : Part
{
	[SerializeField] private Animator animator;
	[SerializeField] private float fireForce = 1000;
	[SerializeField] private Transform barrelEnd;
	[SerializeField] private Projectile projectile;
	[SerializeField] private GameObject firingEffect;
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

	public override void OnPlay()
	{
		base.OnPlay();
		animator.enabled = true;
		rb = GetComponentInParent<Rigidbody>();
	}

	public override void OnEndPlay()
	{
		base.OnEndPlay();
		animator.enabled = false;
	}

	public void Fire()
	{
		animator.SetTrigger("Fire");
		rb.AddForceAtPosition(-transform.forward * fireForce, transform.position, ForceMode.Impulse);
		Projectile newProjectile = Instantiate(projectile, barrelEnd.position, barrelEnd.rotation);
		newProjectile.Launch();

		if (firingEffect != null)
		{
			var effect = Instantiate(firingEffect, barrelEnd.position, barrelEnd.rotation);
			Destroy(effect, 2);
		}

		Debug.Log("Fire!");
	}
}
