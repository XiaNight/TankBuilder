using UnityEngine;

public class Mount : MonoBehaviour
{
	[SerializeField] private new Collider collider;

	public Part GetParent()
	{
		return GetComponentInParent<Part>();
	}

	public void SetState(State state)
	{
		collider.enabled = state switch
		{
			State.Enabled => true,
			State.ShowOnly => false,
			State.ActiveButHide => true,
			State.Disabled => false,
			_ => collider.enabled
		};
		gameObject.SetActive(state != State.Disabled);
	}

	public enum State
	{
		Enabled,
		ShowOnly,
		ActiveButHide,
		Disabled
	}
}