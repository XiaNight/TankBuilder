using UnityEngine;
using UnityEditor;

public class PartsCreationEditorWindow : EditorWindow
{
	private Mount mount;

	private const string PART_PATH = "Assets/Prefab/Game/Mount.prefab";
	private const float GRID_SIZE = 0.25f;

	private Vector3 mountDimensions;
	private bool createTop = true;
	private bool createBottom = true;
	private bool createLeft = true;
	private bool createRight = true;
	private bool createFront = true;
	private bool createBack = true;

	private void OnEnable()
	{
		mount = AssetDatabase.LoadAssetAtPath<Mount>(PART_PATH);
	}

	[MenuItem("TankBuilder/PartsCreationWindow")]
	private static void ShowWindow()
	{
		var window = GetWindow<PartsCreationEditorWindow>();
		window.titleContent = new GUIContent("PartsCreationWindow");
		window.Show();
	}

	private void OnGUI()
	{
		if (mount == null)
		{
			EditorGUILayout.HelpBox("Cannot load Mount from path: " + PART_PATH, MessageType.Error);
			return;
		}
		else
		{
			EditorGUILayout.HelpBox("Mount loaded successfully", MessageType.Info);
		}

		EditorGUILayout.Space();

		//- Mount Dimensions
		EditorGUILayout.LabelField("Mount Dimensions", EditorStyles.boldLabel);
		mountDimensions = EditorGUILayout.Vector3Field("Dimensions", mountDimensions);

		EditorGUILayout.Space();

		//- Mount Sides
		EditorGUILayout.LabelField("Mount Sides", EditorStyles.boldLabel);
		createTop = EditorGUILayout.Toggle("Top", createTop);
		createBottom = EditorGUILayout.Toggle("Bottom", createBottom);
		createLeft = EditorGUILayout.Toggle("Left", createLeft);
		createRight = EditorGUILayout.Toggle("Right", createRight);
		createFront = EditorGUILayout.Toggle("Front", createFront);
		createBack = EditorGUILayout.Toggle("Back", createBack);

		EditorGUILayout.Space();

		//- Create Mount
		if (GUILayout.Button("Create Mount"))
		{
			//- Top/Bottom side
			for (int i = 0; i < mountDimensions.x; i++)
			{
				for (int j = 0; j < mountDimensions.z; j++)
				{
					if (createTop)
						Instantiate(mount, new Vector3(i, mountDimensions.y - 0.5f, j) * GRID_SIZE, Quaternion.LookRotation(Vector3.down));
					if (createBottom)
						Instantiate(mount, new Vector3(i, -0.5f, j) * GRID_SIZE, Quaternion.LookRotation(Vector3.up));
				}
			}

			//- Left/Right side
			for (int i = 0; i < mountDimensions.y; i++)
			{
				for (int j = 0; j < mountDimensions.z; j++)
				{
					if (createLeft)
						Instantiate(mount, new Vector3(-0.5f, i, j) * GRID_SIZE, Quaternion.LookRotation(Vector3.right));
					if (createRight)
						Instantiate(mount, new Vector3(mountDimensions.x - 0.5f, i, j) * GRID_SIZE, Quaternion.LookRotation(Vector3.left));
				}
			}

			//- Front/Back side
			for (int i = 0; i < mountDimensions.x; i++)
			{
				for (int j = 0; j < mountDimensions.y; j++)
				{
					if (createFront)
						Instantiate(mount, new Vector3(i, j, mountDimensions.z - 0.5f) * GRID_SIZE, Quaternion.LookRotation(Vector3.back));
					if (createBack)
						Instantiate(mount, new Vector3(i, j, -0.5f) * GRID_SIZE, Quaternion.LookRotation(Vector3.forward));
				}
			}
		}
	}
}