using UnityEngine;

public class FreeHinge : Contraption
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private HingeJoint hingeJoint;

	public override void SetPlayingState(bool isPlaying)
	{
		base.SetPlayingState(isPlaying);

		rb.isKinematic = !isPlaying;

		hingeJoint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();
		if (!isPlaying)
		{
			content.transform.localRotation = Quaternion.identity;
		}
	}
}