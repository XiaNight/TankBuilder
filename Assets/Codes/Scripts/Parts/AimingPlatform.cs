using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AimingPlatform : FreeHinge
{
	[Header("Motor")]
	[SerializeField] private FloatSettingField forceField;

	[Tooltip("Max Speed in degrees per second")]
	[SerializeField] private float maxSpeed;

	[Header("PId Control")]
	[SerializeField] private float angleToStop;
	[SerializeField] private float postAmplitude;

	private Transform aimingTarget;
	private Vector3 localTargetPosition;

	private new void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (!isPlaying) return;

		UpdateAimingDirection();
	}

	private void FixedUpdate()
	{
		if (!isPlaying) return;

		//- Angle Calculation
		float angle = Vector3.SignedAngle(Vector3.forward, localTargetPosition, Vector3.up);

		//- Speed Calculation
		float calculatedSpeed = Mathf.LerpUnclamped(0, maxSpeed, angle / angleToStop);
		calculatedSpeed = Mathf.Clamp(calculatedSpeed, -maxSpeed, maxSpeed);

		//- Set Motor Speed
		JointMotor motor = hingeJoint.motor;
		motor.targetVelocity = calculatedSpeed;
		hingeJoint.motor = motor;
	}

	public static float CalculateStoppingRotation(float motorForce, float inertiaTensor, float initialAngularVelocity)
	{
		inertiaTensor = Mathf.Max(inertiaTensor, 1f);
		// Convert initial angular velocity from degrees per second to radians per second
		float initialAngularVelocityRad = initialAngularVelocity * Mathf.Deg2Rad;

		// Calculate angular deceleration (alpha)
		float angularDeceleration = motorForce / inertiaTensor;

		// Calculate time to stop (t)
		float timeToStop = initialAngularVelocityRad / angularDeceleration;

		// Calculate the stopping rotation (theta) in radians
		float stoppingRotationRad = (initialAngularVelocityRad * timeToStop) - (0.5f * angularDeceleration * timeToStop * timeToStop);

		// Convert stopping rotation from radians to degrees
		float stoppingRotationDeg = stoppingRotationRad * Mathf.Rad2Deg;

		return stoppingRotationDeg;
	}

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);
		if (isPlaying)
		{
			SetAimingTarget(AttachedVehicle.focusPoint);

			//- Set Motor
			JointMotor motor = hingeJoint.motor;
			motor.force = forceField.Value;
			hingeJoint.motor = motor;

			//- Calculate Stopping Rotation
			Vector3 horizontalInertiaTensor = rb.inertiaTensor;
			horizontalInertiaTensor.y = 0;

			float inertia = horizontalInertiaTensor.magnitude;
			float angleToStop = CalculateStoppingRotation(motor.force, inertia, maxSpeed);
			this.angleToStop = angleToStop * 1.1f;
			Debug.Log(angleToStop);
		}
	}

	public void SetAimingTarget(Transform target) => aimingTarget = target;

	private void UpdateAimingDirection()
	{
		if (aimingTarget == null) return;
		localTargetPosition = content.InverseTransformPoint(aimingTarget.position);
		localTargetPosition.y = 0;
		localTargetPosition.Normalize();
	}

	public override List<ISettingField> OpenSettings()
	{
		List<ISettingField> baseFields = base.OpenSettings();

		SettingField<float> forceSetting = new(forceField.Key, forceField.Value);
		forceSetting.OnValueChangedEvent += forceField.SetValue;

		baseFields.Add(forceField);

		return baseFields;
	}

	#region Serialization

	public override JObject Serialize()
	{
		JObject data = base.Serialize();
		data[forceField.Key] = forceField.Value;
		return data;
	}

	public override void Deserialize(JObject data)
	{
		base.Deserialize(data);
		forceField.SetValue(TryParseData<float>(data, forceField.Key));
	}

	#endregion

	private const float GIZMO_SIZE = 3f;
	private const int GIZMO_CIRCLE_RESOLUTION = 15;
	private void OnDrawGizmos()
	{
		if (!isPlaying) return;

		//- Draw wired circle
		Gizmos.color = Color.white;
		for (int i = 0; i < 360; i += GIZMO_CIRCLE_RESOLUTION)
		{
			Vector3 pointA = CalcPoint(i);
			Vector3 pointB = CalcPoint(i + GIZMO_CIRCLE_RESOLUTION);
			Gizmos.DrawLine(pointA, pointB);
		}

		Vector3 CalcPoint(float angle) => content.position + new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * GIZMO_SIZE, 0, Mathf.Cos(angle * Mathf.Deg2Rad) * GIZMO_SIZE);

		//- Local Target Direction
		Gizmos.color = Color.red;
		Vector3 targetLocal = content.InverseTransformPoint(aimingTarget.position);
		targetLocal.y = 0;
		targetLocal = targetLocal.normalized * GIZMO_SIZE;
		Gizmos.DrawLine(content.position, content.TransformPoint(targetLocal));

		//- Forward
		Vector3 forwardPoint = content.position + content.forward * GIZMO_SIZE;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(content.position, forwardPoint);

		//- Force Direction
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(forwardPoint, forwardPoint + content.right * hingeJoint.motor.targetVelocity / 10);
	}

	public class PIDController
	{
		public float pFactor, iFactor, dFactor;
		private float integral;
		private float lastError;

		public PIDController(float pFactor, float iFactor, float dFactor)
		{
			this.pFactor = pFactor;
			this.iFactor = iFactor;
			this.dFactor = dFactor;
		}

		public float Update(float setpoint, float actual, float timeFrame)
		{
			float present = setpoint - actual;
			integral += present * timeFrame;
			float deriv = (present - lastError) / timeFrame;
			lastError = present;
			return present * pFactor + integral * iFactor + deriv * dFactor;
		}
	}
}
