using UnityEngine;

public interface IWeapon
{
	void Fire();
	bool IsLoaded { get; }
	public bool IsAimed(Vector3 aimPos);
}