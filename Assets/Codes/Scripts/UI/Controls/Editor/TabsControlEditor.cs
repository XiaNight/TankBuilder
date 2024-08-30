using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabsControl))]
public class TabsControlEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Find Tabs in children"))
		{
			TabsControl tabsControl = (TabsControl)target;
			tabsControl.ClearTabs();
			Tab[] tabs = tabsControl.GetComponentsInChildren<Tab>(true);
			for (int i = 0; i < tabs.Length; i++)
			{
				tabsControl.AssignTab(tabs[i]);
			}

			EditorUtility.SetDirty(tabsControl);
		}
	}
}