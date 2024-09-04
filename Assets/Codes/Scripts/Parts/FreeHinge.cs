using UnityEngine;

public class FreeHinge : Contraption
{
	[SerializeField] protected Rigidbody rb;
	[SerializeField] protected new HingeJoint hingeJoint;

	public override void OnPlay()
	{
		base.OnPlay();

		hingeJoint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();

		rb.isKinematic = false;
		rb.mass = CalculateMass();
	}

	public override void OnEndPlay()
	{
		base.OnEndPlay();
		rb.isKinematic = true;
		content.transform.localRotation = Quaternion.identity;
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