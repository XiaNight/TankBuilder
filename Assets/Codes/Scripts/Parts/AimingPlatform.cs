using UnityEngine;

public class AimingPlatform : FreeHinge
{
	[SerializeField] private float P;
	[SerializeField] private float I;
	[SerializeField] private float D;
	[SerializeField] private float preAmplitude;
	[SerializeField] private float postAmplitude;
	private Vector3 localTargetDirection;

	private PIDController pidController;

	private void OnValidate()
	{
		if (pidController != null)
		{
			pidController.pFactor = P;
			pidController.iFactor = I;
			pidController.dFactor = D;
		}
	}

	private void Awake()
	{
		pidController = new PIDController(P, I, D);
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			Vector3 target = hit.point;
			localTargetDirection = content.InverseTransformDirection(target - transform.position);
			localTargetDirection.y = 0;
			localTargetDirection.Normalize();
		}
	}

	private void FixedUpdate()
	{
		float forward = Vector3.Dot(localTargetDirection, Vector3.forward);
		float right = Vector3.Dot(localTargetDirection, Vector3.right);

		if (forward < 0) right = right < 0 ? -1 : 1;
		right = Mathf.Clamp(right * preAmplitude, -1, 1);

		JointMotor motor = hingeJoint.motor;
		motor.targetVelocity = pidController.Update(0, -right, Time.fixedDeltaTime) * postAmplitude;
		hingeJoint.motor = motor;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(content.position, content.position + content.TransformDirection(localTargetDirection));

		Gizmos.color = Color.green;
		Gizmos.DrawLine(content.position, content.position + content.forward * 5);
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
