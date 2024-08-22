using UnityEngine;

[CreateAssetMenu(fileName = "PartData", menuName = "PartData", order = 0)]
public class PartData : ScriptableObject
{
	public string Name;
	public Sprite Icon;
	public Part Prefab;

	[TextArea]
	public string description;
}