using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartList : MonoBehaviour
{
	public PartEntry partEntryPrefab;
	public Transform partListParent;

	private List<PartEntry> spawnedEntries = new();
	private int selectedEntryIndex = 0;

	private void Start()
	{
		foreach (KeyValuePair<string, PartData> part in PartManager.Instance.parts)
		{
			PartEntry partEntry = Instantiate(partEntryPrefab, partListParent);
			partEntry.SetPart(part.Value);
			spawnedEntries.Add(partEntry);

			int index = spawnedEntries.Count - 1;
			partEntry.OnClick += () =>
			{
				OnPartSelected(index);
			};
		}
	}

	private void Update()
	{
		// Scrollwheel selection
		if (Input.mouseScrollDelta.y > 0)
		{
			selectedEntryIndex--;
			if (selectedEntryIndex < 0)
				selectedEntryIndex = spawnedEntries.Count - 1;
			spawnedEntries[selectedEntryIndex].Select();
		}
		else if (Input.mouseScrollDelta.y < 0)
		{
			selectedEntryIndex++;
			if (selectedEntryIndex >= spawnedEntries.Count)
				selectedEntryIndex = 0;
			spawnedEntries[selectedEntryIndex].Select();
		}
	}

	private void OnPartSelected(int index)
	{
		selectedEntryIndex = index;
		Builder.Instance.SetSelectedPartData(spawnedEntries[selectedEntryIndex].PartData);
		UpdateHighlight();
	}

	private void UpdateHighlight()
	{
		for (int i = 0; i < spawnedEntries.Count; i++)
		{
			spawnedEntries[i].SetHighlight(i == selectedEntryIndex);
		}
	}
}
