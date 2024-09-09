using System.Collections.Generic;
using UnityEngine;

class RayCastProjectile
{
	//- Settings
	private Ray ray;
	private LayerMask rayCastLayer;
	private LayerMask armorLayer;
	private readonly float penetration;
	private readonly bool canCreateShrapnel;

	//- Ray Cast Infos
	private float remainingPenetration;
	private bool isHit = false;
	private bool hasThickness;
	private bool hasPenetrated;
	private float penetrationThickness;
	private Vector3 backHitPos = new(0, -100, 0);
	private Vector3 hitPos = new(0, -100, 0);
	private readonly List<RayCastProjectile> rayCastProjectiles = new();

	public RayCastProjectile(Ray ray, LayerMask layerMask, LayerMask armorLayer, float penetration, bool canCreateShrapnel = false)
	{
		this.ray = ray;
		rayCastLayer = layerMask;
		this.armorLayer = armorLayer;
		this.penetration = penetration;
		remainingPenetration = penetration;
		this.canCreateShrapnel = canCreateShrapnel;
	}

	public void Initiate(float range)
	{
		if (Physics.Raycast(ray, out RaycastHit hit, range, rayCastLayer))
		{
			OnHit(hit.collider, hit.point);
		}
		else
		{

		}
	}

	public HitInfo OnHit(Collider collider, Vector3 hitPoint)
	{
		hitPos = hitPoint;
		isHit = true;

		//- Get Armor Density
		float density = 0.3f;
		Debug.Log("collider layer: " + (1 << collider.gameObject.layer) + " armor layer: " + armorLayer.value);
		if (((1 << collider.gameObject.layer) & armorLayer.value) > 0)
		{
			// check penetration
			if (collider.TryGetComponent(out IArmor armor))
			{
				density = armor.ArmorDensity;
			}
		}

		//- Get raw penetration thickness
		if (collider.Raycast(new Ray(hitPoint + ray.direction * 10, -ray.direction), out RaycastHit hit, 10))
		{
			// penetration
			backHitPos = hit.point;
			hasThickness = true;
		}
		float rawThickness = Vector3.Distance(hitPoint, backHitPos);
		float thickness = rawThickness * density;

		penetrationThickness = remainingPenetration / density;

		if (remainingPenetration / 1000 >= thickness) hasPenetrated = true;

		remainingPenetration -= thickness * 1000;

		if (hasPenetrated && canCreateShrapnel) CreateShrapnels(new Ray(hitPoint, ray.direction));

		return new HitInfo
		{
			hitPos = hitPos,
			backHitPos = backHitPos,
			direction = ray.direction,
			thickness = thickness
		};
	}

	public void CreateShrapnels(Ray ray)
	{
		for (int i = 0; i < 10; i++)
		{
			Vector3 randomDirection = Random.onUnitSphere;

			randomDirection = (randomDirection + ray.direction * 5).normalized; //- Controls the spread of the shrapnel

			CreateShrapnel(new Ray(backHitPos, randomDirection));
		}
	}

	public void CreateShrapnel(Ray ray)
	{
		RayCastProjectile rayCastProjectile = new(ray, rayCastLayer, armorLayer, penetration, false);
		rayCastProjectile.Initiate(100);
		rayCastProjectiles.Add(rayCastProjectile);
	}

	public struct HitInfo
	{
		public Vector3 hitPos;
		public Vector3 backHitPos;
		public Vector3 direction;
		public float thickness;
	}

	public void OnDrawGizmos()
	{
		if (hasThickness)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(hitPos, backHitPos);

			if (hasPenetrated)
				Gizmos.color = Color.blue;
			else
				Gizmos.color = Color.red;
			Gizmos.DrawSphere(backHitPos, 0.1f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(hitPos + ray.direction * penetrationThickness / 1000, 0.05f);

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(hitPos, 0.1f);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(hitPos, 0.1f);
		}

		Gizmos.color = canCreateShrapnel switch
		{
			true => Color.yellow,
			false => Color.red,
		};

		Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * penetrationThickness / 1000);

		foreach (RayCastProjectile rayCastProjectile in rayCastProjectiles)
		{
			rayCastProjectile.OnDrawGizmos();
		}
	}
}