using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AimingPlatform : FreeHinge
{
	[Header("Motor")]
	[SerializeField] private FloatSettingField forceField;
	[SerializeField] private BoolSettingField limitRotation;
	[SerializeField] private FloatSettingField negetiveRotationLimit;
	[SerializeField] private FloatSettingField positiveRotationLimit;

	[Tooltip("Max Speed in degrees per second")]
	[SerializeField] private FloatSettingField maxSpeedField;

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
		float calculatedSpeed = Mathf.LerpUnclamped(0, maxSpeedField.Value, angle / angleToStop);
		calculatedSpeed = Mathf.Clamp(calculatedSpeed, -maxSpeedField.Value, maxSpeedField.Value);

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

	public override void OnPlay()
	{
		base.OnPlay();
		SetAimingTarget(AttachedVehicle.focusPoint);

		//- Set Motor
		JointMotor motor = hingeJoint.motor;
		motor.force = forceField.Value;
		hingeJoint.motor = motor;

		//- Set Limits
		hingeJoint.useLimits = limitRotation.Value;
		hingeJoint.limits = new JointLimits { min = negetiveRotationLimit.Value, max = positiveRotationLimit.Value };

		//- Calculate Stopping Rotation
		Vector3 horizontalInertiaTensor = rb.inertiaTensor;
		horizontalInertiaTensor.y = 0;

		float inertia = horizontalInertiaTensor.magnitude;
		float angleToStop = CalculateStoppingRotation(motor.force, inertia, maxSpeedField.Value);
		angleToStop = Mathf.Max(angleToStop, 1);
		this.angleToStop = angleToStop * 1.1f;
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

		SettingField<bool> limitRotationSetting = new(limitRotation.Key, limitRotation.Value);
		limitRotationSetting.OnValueChangedEvent += limitRotation.SetValue;

		SettingField<float> negetiveRotationLimitSetting = new(negetiveRotationLimit.Key, negetiveRotationLimit.Value);
		negetiveRotationLimitSetting.OnValueChangedEvent += negetiveRotationLimit.SetValue;

		SettingField<float> positiveRotationLimitSetting = new(positiveRotationLimit.Key, positiveRotationLimit.Value);
		positiveRotationLimitSetting.OnValueChangedEvent += positiveRotationLimit.SetValue;

		SettingField<float> maxSpeedSetting = new(maxSpeedField.Key, maxSpeedField.Value);
		maxSpeedSetting.OnValueChangedEvent += maxSpeedField.SetValue;


		baseFields.Add(forceField);
		baseFields.Add(limitRotation);
		baseFields.Add(negetiveRotationLimit);
		baseFields.Add(positiveRotationLimit);
		baseFields.Add(maxSpeedField);

		return baseFields;
	}

	#region Serialization

	public override JObject Serialize()
	{
		JObject data = base.Serialize();
		data[forceField.Key] = forceField.Value;
		data[limitRotation.Key] = limitRotation.Value;
		data[negetiveRotationLimit.Key] = negetiveRotationLimit.Value;
		data[positiveRotationLimit.Key] = positiveRotationLimit.Value;
		data[maxSpeedField.Key] = maxSpeedField.Value;
		return data;
	}

	public override void Deserialize(JObject data)
	{
		base.Deserialize(data);
		forceField.SetValue(TryParseData<float>(data, forceField.Key));
		negetiveRotationLimit.SetValue(TryParseData<float>(data, negetiveRotationLimit.Key));
		positiveRotationLimit.SetValue(TryParseData<float>(data, positiveRotationLimit.Key));
		maxSpeedField.SetValue(TryParseData<float>(data, maxSpeedField.Key));
	}

	#endregion

	private const float GIZMO_SIZE = 3f;
	private const int GIZMO_CIRCLE_RESOLUTION = 15;
	private void OnDrawGizmos()
	{
		if (!isPlaying) return;

		//- Draw wired circle
		Gizmos.color = Color.white;
		for (float i = negetiveRotationLimit.Value; i < positiveRotationLimit.Value; i += GIZMO_CIRCLE_RESOLUTION)
		{
			float angle = i;
			Vector3 pointA = CalcPoint(angle);
			Vector3 pointB = CalcPoint(angle + GIZMO_CIRCLE_RESOLUTION);
			Gizmos.DrawLine(pointA, pointB);
		}

		Vector3 CalcPoint(float angle) => content.TransformPoint(new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * GIZMO_SIZE, 0, Mathf.Cos(angle * Mathf.Deg2Rad) * GIZMO_SIZE));

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
