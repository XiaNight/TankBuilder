using UnityEngine;

public class FreeHinge : Contraption
{
	[SerializeField] protected Rigidbody rb;
	[SerializeField] protected new HingeJoint hingeJoint;

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);

		rb.isKinematic = !isPlaying;

		hingeJoint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();

		if (isPlaying)
		{
			rb.mass = CalculateMass();
		}
		else
		{
			content.transform.localRotation = Quaternion.identity;
		}
	}

	public override void SetMetaData(PartData data)
	{
		base.SetMetaData(data);
		rb.mass = data.mass;
	}

	public override float CalculateMass()
	{
		return base.CalculateMass();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Vector3 cog = content.TransformPoint(rb.centerOfMass);
		Gizmos.DrawSphere(cog, 0.05f);
		Gizmos.DrawLine(content.position, cog);
	}
}