using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
	[SerializeField] private Collider penetrationCollider;
	[SerializeField] private GameObject hitEffect;
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

	private Rigidbody rb;
	private Vector3 contactPos = new(0, -100, 0);
	private Vector3 backHitPos = new(0, -100, 0);
	private Vector3 hitPos = new(0, -100, 0);

	private float remainingPenetration;

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
		if (hasPenetrated)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(hitPos, backHitPos);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(contactPos, 0.1f);

		Gizmos.color = Color.green;
		Gizmos.DrawSphere(hitPos, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(backHitPos, 0.1f);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isHit) return;
		isHit = true;

		contactPos = other.contacts[0].point;
		hitPos = transform.position;

		Debug.Log("Hit");

		// calculate reflection vector
		Vector3 hitVelocity = -other.relativeVelocity.normalized;
		Vector3 normal = other.contacts[0].normal;
		Vector3 outDirection = Vector3.Reflect(hitVelocity, normal);

		float density = 0.3f;
		if (other.collider.gameObject.layer == armorLayer)
		{
			// check penetration
			if (other.collider.TryGetComponent(out IArmor armor))
			{
				density = armor.ArmorDensity;
			}
		}

		if (other.collider.Raycast(new Ray(contactPos + hitVelocity * 10, -hitVelocity), out RaycastHit hit, penetration))
		{
			// penetration
			Debug.Log("Penetration");
			hasPenetrated = true;
			backHitPos = hit.point;
		}
		float thickness = Vector3.Distance(contactPos, backHitPos);
		remainingPenetration -= thickness * density;

		if (remainingPenetration <= 0)
		{
			return;
		}

		Instantiate(hitEffect, contactPos, Quaternion.LookRotation(outDirection));
	}
}