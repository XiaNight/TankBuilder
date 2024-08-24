using UnityEngine;

[CreateAssetMenu(fileName = "PartData", menuName = "PartData", order = 0)]
public class PartData : ScriptableObject
{
	public string partId;
	public string Name;
	public Sprite icon;
	public Part prefab;
	public float mass = 1;

	[TextArea]
	public string description;

	private void Reset()
	{
		partId = GetInstanceID().ToString();
	}
}