using UnityEngine;

public class Wheel : Part, IMovement
{
	public float steeringAngle = 40;
	public float brake = 10;
	public float smoothSteerFactor = 5;
	public WheelCollider wheelCollider;
	public Transform wheelTransform;

	private int visualRotationFix = 1;

	private float targetSteeringAngle;
	private float currentSteeringAngle;

	public float Circumference => wheelCollider.radius * 2 * Mathf.PI;
	public float UnitRPM => wheelCollider.rpm / Circumference;

	private new void Awake()
	{
		base.Awake();
	}

	public override void OnSpawned()
	{
		base.OnSpawned();
		visualRotationFix = transform.forward.z > 0 ? 1 : -1;
		UpdateWheelVisual();
	}

	public override void OnPlay()
	{
		base.OnPlay();
		wheelCollider.rotationSpeed = 0;
	}

	public override void OnEndPlay()
	{
		base.OnEndPlay();
		wheelCollider.motorTorque = 0;
		wheelCollider.wheelDampingRate = 0;
		wheelCollider.steerAngle = 0;
		wheelCollider.brakeTorque = 0;
		wheelCollider.rotationSpeed = 0;

		UpdateWheelVisual();
	}

	public override void OnPlacementChanged()
	{
		base.OnPlacementChanged();
		visualRotationFix = transform.forward.z > 0 ? 1 : -1;
		UpdateWheelVisual();
	}

	public override void PlayUpdate()
	{
		UpdateWheelVisual();
	}

	//- Update wheel visual
	private void UpdateWheelVisual()
	{
		wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
		if (visualRotationFix == -1)
		{
			rotation *= Quaternion.Euler(0, 0, 180);
		}
		wheelTransform.SetPositionAndRotation(position, rotation);
	}

	public override void PlayFixedUpdate()
	{
		currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, Time.fixedDeltaTime * smoothSteerFactor);
		wheelCollider.steerAngle = currentSteeringAngle;
	}


	/// <summary>
	/// Set the power level of the motors, from -1 to 1
	/// </summary>
	/// <param name="torque"> The power level of the motors, from -1 to 1 </param>
	public void SetTorque(float torque)
	{
		wheelCollider.motorTorque = torque;
	}

	public void SetBrake(float brake)
	{
		wheelCollider.brakeTorque = this.brake * brake;
	}

	public void SetSteer(float steer)
	{
		targetSteeringAngle = Mathf.Lerp(-steeringAngle, steeringAngle, (steer + 1) / 2);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(wheelCollider.transform.position, wheelCollider.transform.position + wheelCollider.transform.forward * 5 * wheelCollider.motorTorque);
	}
}
