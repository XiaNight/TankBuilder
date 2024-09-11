using UnityEngine;

public class PowerUnit : Part
{
	[SerializeField] private float horsePower = 100;
	[SerializeField] private AnimationCurve torqueRPMCurve;


	public float HorsePower => horsePower;

	public float GetTorque(float rpm)
	{
		return horsePower * health / MaxHealth;
	}
}