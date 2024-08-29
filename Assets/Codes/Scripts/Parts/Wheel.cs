using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Part
{
	public float power = 10;
	public float steer = 10;
	public float brake = 10;
	public WheelCollider wheelCollider;
	public Transform wheelTransform;

	private Part part;
	private int directionMultiplier = 1;

	private new void Awake()
	{
		base.Awake();
		part = GetComponent<Part>();
	}

	public void SetPower(float power)
	{
		this.power = power;
	}

	public void SetSteer(float steer)
	{
		this.steer = steer;
	}

	public void SetBrake(float brake)
	{
		this.brake = brake;
	}

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);
		wheelCollider.rotationSpeed = 0;
	}

	private void Update()
	{
		if (!part.isPlaying) return;

		float forward = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
		wheelCollider.motorTorque = forward * power * directionMultiplier;
		wheelCollider.wheelDampingRate = Input.GetKey(KeyCode.Space) ? brake / wheelCollider.rpm : 0;
		wheelCollider.steerAngle = Input.GetKey(KeyCode.A) ? -steer : Input.GetKey(KeyCode.D) ? steer : 0;

		wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
		wheelTransform.SetPositionAndRotation(position, rotation);
	}

	public override void OnMouseOver()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			directionMultiplier *= -1;
			print("Direction Multiplier: " + directionMultiplier);
		}

		Debug.Log("OnMouseOver");
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(wheelCollider.transform.position, wheelCollider.transform.position + wheelCollider.transform.forward * 5 * wheelCollider.motorTorque);
	}
}
