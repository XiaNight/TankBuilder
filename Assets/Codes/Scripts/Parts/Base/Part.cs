using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
	[SerializeField] private List<Mount> mounts = new();
	[SerializeField] private List<Collider> physicColliders = new();

	public Contraption ParentContraption { get; private set; }

	public Collider[] GetColliders()
	{
		return physicColliders.ToArray();
	}

	public bool ContainsMount(Mount mount)
	{
		return mounts.Contains(mount);
	}

	public virtual void SetPlayingState(bool state)
	{
		SetMountState(state ? Mount.State.Disabled : Mount.State.Enabled);
	}

	public virtual void SetMountState(Mount.State state)
	{
		foreach (Mount mount in mounts)
		{
			mount.SetState(state);
		}
	}

	public virtual Contraption GetContraption()
	{
		return ParentContraption;
	}

	public void SetContraption(Contraption contraption)
	{
		if (ParentContraption != null)
		{
			Contraption previousContraption = ParentContraption;
			ParentContraption = null;
			previousContraption.RemovePart(this);
		}
		ParentContraption = contraption;
	}

	public Mount[] FindMatchingMounts(Mount mount)
	{
		Vector3 mountDir = mount.transform.forward;
		List<Mount> matchingMounts = new();
		foreach (Mount m in mounts)
		{
			Vector3 mDir = m.transform.forward;
			if (Vector3.Dot(mDir, mountDir) <= -0.99f)
			{
				matchingMounts.Add(m);
			}
		}
		return matchingMounts.ToArray();
	}

	public virtual void RestoreOriginal()
	{

	}
}