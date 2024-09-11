using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private Collider penetrationCollider;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private LayerMask rayCastLayer;
	[SerializeField] private LayerMask armorLayer;

	private bool isHit;
	private bool hasPenetrated;

	/// <summary>
	/// muzzle velocity in m/s
	/// </summary>
	public float muzzleVelocity = 850;

	/// <summary>
	/// penetration in RHA mm
	/// </summary>
	public float penetration = 180;

	public float projectilMass = 20;

	public bool createShrapnel = false;
	public ShrapnelSetting shrapnelSetting;

	private Rigidbody rb;
	private float remainingPenetration;
	private List<RayCastProjectile> rayCastProjectiles = new();

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		remainingPenetration = penetration;
		Destroy(gameObject, 60);
	}

	public void Launch()
	{
		rb.velocity = transform.forward * muzzleVelocity;
	}

	private void OnDrawGizmos()
	{
		foreach (RayCastProjectile rayCastProjectile in rayCastProjectiles)
		{
			rayCastProjectile.OnDrawGizmos();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isHit) return;
		isHit = true;

		SpawnRayCastProjectile(other);

		Debug.Log(other.transform.name);

		//- calculate reflection vector
		Vector3 hitVelocity = -other.relativeVelocity;
		Vector3 normal = other.contacts[0].normal;
		Vector3 outDirection = Vector3.Reflect(hitVelocity, normal);

		if (remainingPenetration <= 0)
		{
			return;
		}

		Instantiate(hitEffect, other.contacts[0].point, Quaternion.LookRotation(outDirection));

		Destroy(gameObject, 5);
	}

	public void SpawnRayCastProjectile(Collision other)
	{
		Vector3 hitVelocity = -other.relativeVelocity;
		Vector3 hitDirection = hitVelocity.normalized;

		Ray ray = new(other.contacts[0].point, hitDirection);

		RayCastProjectile rayCastProjectile = new(ray, rayCastLayer, armorLayer, penetration, true);
		rayCastProjectile.OnHit(other.collider, other.contacts[0].point);
		rayCastProjectiles.Add(rayCastProjectile);
	}

	[Serializable]
	public struct ShrapnelSetting
	{
		public float shrapnelCount;
		public float mainShrapnelDamage;
	}
}