using System.Collections.Generic;
using UnityEngine;

public class PartManager : MonoBehaviour
{
	public static PartManager Instance { get; private set; }

	[SerializeField] private List<PartData> partList = new();

	public Dictionary<string, PartData> parts = new();

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		foreach (PartData part in partList)
		{
			parts.Add(part.partId, part);
		}
	}

	public void ListParts()
	{
		foreach (KeyValuePair<string, PartData> part in parts)
		{
			Debug.Log(part.Key);
		}
	}
}