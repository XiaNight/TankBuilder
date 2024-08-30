using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TabsControl : MonoBehaviour
{
	[SerializeField] private List<Tab> tabs;
	[SerializeField] private int initialTab = -1;

	public delegate void OnTabSelectedHandler(int tabIndex);
	public event OnTabSelectedHandler OnTabSelected;

	public int CurrentTab { get; private set; } = -1;

	// If true, the tab will be activated on click even if it is already activated.
	public bool AllowUpdateOnSame { get; set; } = true;

	private void Awake()
	{
		for (int i = 0; i < tabs.Count; i++)
		{
			int iCopy = i;
			tabs[i].tabButton.onClick.AddListener(() =>
			{
				ActivateTab(iCopy);
			});
		}
	}

	private void Start()
	{
		if (initialTab >= 0)
		{
			ActivateTab(initialTab);
		}
	}

	public void ActivateTab(int index)
	{
		if (!AllowUpdateOnSame && CurrentTab == index) return;

		for (int i = 0; i < tabs.Count; i++)
		{
			if (i == index) continue;

			tabs[i].toggleable.Disable();
		}
		tabs[index].toggleable.Enable();
		CurrentTab = index;
		OnTabSelected?.Invoke(index);
	}

	public void AssignTab(Tab tab)
	{
		tabs.Add(tab);
	}

	public void ClearTabs()
	{
		tabs.Clear();
	}

	public bool GetTab(int index, out Tab tab)
	{
		if (index < 0 || index >= tabs.Count)
		{
			tab = null;
			return false;
		}
		tab = tabs[index];
		return true;
	}
}
