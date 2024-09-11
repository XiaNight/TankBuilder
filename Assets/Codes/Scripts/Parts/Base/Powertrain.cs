using System.Collections.Generic;
using UnityEngine;

public class Powertrain : MonoBehaviour
{
	private readonly List<PowerUnit> powerUnits = new();
	private readonly List<IMovement> mobility = new();

	[SerializeField] private AnimationCurve torqueRPMCurve;

	private float movementForward;
	private int mobilityCount;
	private float power;
	private float steering;
	private float brake;

	private float engineRPM;

	public void MobilityUpdate()
	{
		movementForward = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
		steering = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
		brake = Input.GetKey(KeyCode.Space) ? 1 : 0;
	}

	public void FixedUpdate()
	{
		RecalculatePower();
		float maxUnitRPM = 0;
		foreach (IMovement movement in mobility)
		{
			movement.SetBrake(brake);

			float torque = movementForward * torqueRPMCurve.Evaluate(Mathf.Abs(movement.UnitRPM) * 300) * power / mobilityCount * 50;
			if (movement.UnitRPM * movementForward < 0) movement.SetBrake(1);

			movement.SetTorque(torque);
			movement.SetSteer(steering);

			maxUnitRPM = Mathf.Max(movement.UnitRPM, maxUnitRPM);
		}
		engineRPM = maxUnitRPM;
	}

	public void SetBrake(float amount)
	{
		brake = amount;
	}

	public void SetSteer(float amount)
	{
		steering = amount;
	}

	public void SetThrottle(float movement)
	{
		movementForward = movement;
	}

	public void Clear()
	{
		movementForward = 0;
		steering = 0;
		brake = 0;
		engineRPM = 0;

		powerUnits.Clear();
		mobility.Clear();
	}

	public float GetPower()
	{
		float power = 0;
		foreach (PowerUnit powerUnit in powerUnits)
		{
			power += powerUnit.GetTorque(1000);
		}
		return power;
	}

	public void AddPowerUnit(PowerUnit powerUnit)
	{
		powerUnits.Add(powerUnit);
		RecalculatePower();
	}

	public void RemovePowerUnit(PowerUnit powerUnit)
	{
		if (powerUnits.Contains(powerUnit))
			powerUnits.Remove(powerUnit);
		RecalculatePower();
	}

	private void RecalculatePower()
	{
		power = GetPower();
	}

	public void Add(IMovement movement)
	{
		mobility.Add(movement);
		mobilityCount = mobility.Count;
	}

	public void Remove(IMovement movement)
	{
		if (mobility.Contains(movement))
			mobility.Remove(movement);
		mobilityCount = mobility.Count;
	}
}