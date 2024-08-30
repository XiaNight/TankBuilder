using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Toggleable))]
public class ToggleableEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		// begin horizontal layout
		EditorGUILayout.BeginHorizontal();
		// make two buttons
		if (GUILayout.Button("Enable"))
		{
			Undo.RecordObject(target, "Enable");
			Toggleable toggleable = (Toggleable)target;
			toggleable.Enable();
			EditorUtility.SetDirty(toggleable);
		}
		if (GUILayout.Button("Disable"))
		{
			Undo.RecordObject(target, "Enable");
			Toggleable toggleable = (Toggleable)target;
			toggleable.Disable();
			EditorUtility.SetDirty(toggleable);
		}
		// end horizontal layout
		EditorGUILayout.EndHorizontal();
	}
}
[CustomEditor(typeof(ImageToggleable))]
public class ImageToggleableEditor : ToggleableEditor { }

[CustomEditor(typeof(ToggleableCanvas))]
public class ToggleableCanvasEditor : ToggleableEditor { }

[CustomEditor(typeof(RectTransformToggleable))]
public class RectTransformToggleableEditor : ToggleableEditor
{
	public override void OnInspectorGUI()
	{
		RectTransformToggleable target = (RectTransformToggleable)this.target;
		base.OnInspectorGUI();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Set to enabled data"))
		{
			Undo.RecordObject(target, "Set to enabled data");
			target.enabledData.CopyRectTransform(target.transform as RectTransform);
			EditorUtility.SetDirty(target);
		}
		if (GUILayout.Button("Set to disabled data"))
		{
			Undo.RecordObject(target, "Set to disabled data");
			target.disabledData.CopyRectTransform(target.transform as RectTransform);
			EditorUtility.SetDirty(target);
		}
		EditorGUILayout.EndHorizontal();
	}
}


[CustomEditor(typeof(ToggleableCollection))]
public class ToggleableCollectionEditor : ToggleableEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		// Find all Toggleables in children
		if (GUILayout.Button("Find Toggleables in children"))
		{
			ToggleableCollection toggleableCollection = (ToggleableCollection)target;
			toggleableCollection.Clear();

			// Find all Toggleables in children, except itself
			List<Toggleable> toggleables = new(toggleableCollection.GetComponentsInChildren<Toggleable>(true));
			if (toggleables.Contains(toggleableCollection))
			{
				toggleables.Remove(toggleableCollection);
			}

			toggleableCollection.AssignToggleable(toggleables.ToArray());

			EditorUtility.SetDirty(toggleableCollection);
		}
	}
}


