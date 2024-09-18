using UnityEngine;
using UnityEditor;

public class ArmorPlateEditor<T> : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ArmorPlate armorPlate = (ArmorPlate)target;

		if (GUILayout.Button("Find Mesh Colliders"))
		{
			// begin record
			Undo.RecordObject(armorPlate, "Find Mesh Colliders");

			var meshes = armorPlate.GetComponentsInChildren<MeshCollider>();
			// remove all named "Mount" from the list
			armorPlate.SetArmorMeshes(System.Array.FindAll(meshes, mesh => !mesh.name.Contains("Mount")));

			// end record
			EditorUtility.SetDirty(armorPlate);
		}
	}
}