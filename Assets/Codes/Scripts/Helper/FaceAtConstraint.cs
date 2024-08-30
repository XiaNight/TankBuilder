using UnityEngine;

public class FaceAtConstraint : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private bool flip = false;
	[SerializeField] private bool flat = false; // 是否為平面
	[SerializeField] private bool constantSize = false; // 大小呈現於螢幕上固定

	public float sizeMultiplier = 1f;

	private void LateUpdate()
	{
		transform.rotation = GetLookRotation();
		if (constantSize && target != null)
		{
			transform.localScale = Vector3.Distance(transform.position, target.position) * sizeMultiplier * Vector3.one;
		}
	}

	private void OnEnable()
	{
		transform.rotation = GetLookRotation();
	}

	private Quaternion GetLookRotation()
	{
		if (target != null)
		{
			if (flat)
			{
				Quaternion q = Quaternion.identity;
				q.SetLookRotation(target.forward, target.up);
				return q;
			}
			else
			{
				Vector3 lookPos = target.position - transform.position;
				lookPos.y = 0;
				return Quaternion.LookRotation(lookPos) * Quaternion.Euler(0, flip ? 180 : 0, 0);
			}
		}
		else
		{
			return Quaternion.identity;
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
		transform.rotation = GetLookRotation();
	}
}
