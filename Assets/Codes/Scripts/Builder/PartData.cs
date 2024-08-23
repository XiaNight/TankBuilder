using UnityEngine;

[CreateAssetMenu(fileName = "PartData", menuName = "PartData", order = 0)]
public class PartData : ScriptableObject
{
	public string Name;
	public Sprite Icon;
	public Part Prefab;
	public float mass = 1;

	[TextArea]
	public string description;
}