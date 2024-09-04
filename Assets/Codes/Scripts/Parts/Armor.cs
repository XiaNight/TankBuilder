using UnityEngine;

public class Armor : MonoBehaviour, IArmor
{
	[SerializeField] private float armorDensity = 1;

	public float ArmorDensity => armorDensity;



	public enum Material
	{
		RHA = 100,
		Cast = 85,
		Aluminium = 34,
		DepletedUranium = 230,
	}
}