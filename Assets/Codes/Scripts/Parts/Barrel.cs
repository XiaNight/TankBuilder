using System.Collections;
using UnityEngine;

public class Barrel : Part, IWeapon
{
	[SerializeField] private Animator animator;
	[SerializeField] private float fireForce = 1000;
	[SerializeField] private Transform barrelEnd;
	[SerializeField] private Projectile projectile;
	[SerializeField] private GameObject firingEffect;
	[SerializeField] private float reloadTime = 1;
	[SerializeField] private bool isReloaded = true;

	[Header("Casing")]
	[SerializeField] private GameObject casingPrefab;
	[SerializeField] private Transform casingExit;
	[SerializeField] private float casingExitForce = 1;

	private Rigidbody rb;
	bool IWeapon.IsLoaded => isReloaded;

	private new void Awake()
	{
		base.Awake();
	}

	public override void PlayUpdate()
	{
		if (!AttachedVehicle.IsUser) return;

		if (Input.GetMouseButton(0))
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
		if (!isReloaded) return;
		isReloaded = false;
		animator.SetTrigger("Fire");
		rb.AddForceAtPosition(-transform.forward * fireForce, transform.position, ForceMode.Impulse);
		Projectile newProjectile = Instantiate(projectile, barrelEnd.position, barrelEnd.rotation);
		newProjectile.Launch();

		if (firingEffect != null)
		{
			var effect = Instantiate(firingEffect, barrelEnd.position, barrelEnd.rotation);
			Destroy(effect, 2);
		}

		EjectCasing();

		StartCoroutine(Reload());
	}

	private void EjectCasing()
	{
		if (casingPrefab == null) return;
		if (casingExit == null) return;
		GameObject casing = Instantiate(casingPrefab, casingExit.position, casingExit.rotation);
		if (casing.TryGetComponent(out Rigidbody casingRb))
		{
			casingRb.velocity = rb.velocity;
			casingRb.AddForce(casingExit.forward * casingExitForce, ForceMode.Impulse);

			// add random rotation
			casingRb.AddTorque(casing.transform.right * Random.Range(0.3f, 1.1f), ForceMode.Impulse);
		}
		Destroy(casing, 5);
	}

	bool IWeapon.IsAimed(Vector3 pos)
	{
		Vector3 localDir = transform.InverseTransformPoint(pos).normalized;
		return localDir.z > 0.999f;
	}

	public IEnumerator Reload()
	{
		yield return new WaitForSeconds(reloadTime);
		isReloaded = true;
	}
}
