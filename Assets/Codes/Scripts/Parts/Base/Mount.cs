using UnityEngine;

public class Mount : MonoBehaviour
{
	[SerializeField] private new Collider collider;
	[SerializeField] private MountingRule type = MountingRule.Normal;
	[SerializeField] private ContraptionInheritance inheritance = ContraptionInheritance.Self;
	[SerializeField] private new Renderer renderer;

	[SerializeField] private Material normalMaterial;
	[SerializeField] private Material buildOnlyMaterial;
	[SerializeField] private Material mountOnlyMaterial;

	public MountingRule Rule => type;
	public ContraptionInheritance Inheritance => inheritance;

	private void OnValidate()
	{
		if (renderer != null)
		{
			renderer.material = type switch
			{
				MountingRule.Normal => normalMaterial,
				MountingRule.BuildOnly => buildOnlyMaterial,
				MountingRule.MountOnly => mountOnlyMaterial,
				_ => renderer.material
			};
		}
	}

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

	public enum ContraptionInheritance
	{
		Parent,
		Self
	}

	public enum MountingRule
	{
		/// <summary>
		/// Connect both ways.
		/// </summary>
		Normal,

		/// <summary>
		/// Only for building parts on top of it.
		/// </summary>
		BuildOnly,

		/// <summary>
		/// Only for mounting. No building on top.
		/// </summary>
		MountOnly
	}
}