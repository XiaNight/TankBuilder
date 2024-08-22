using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
	#region Singleton
	public static Builder Instance { get; private set; }
	#endregion

	public Vehicle vehicle;
	public Part selectionPrefab;

	public LayerMask mountLayer;
	private Part selectedInstance;
	private int mountSelector = 0;

	//- Debug
	private Vector3 rayCastPos = new();
	private Color color = Color.red;
	private Quaternion currentRotation = Quaternion.identity;
	private Mount focusedMount = null;
	private bool isDirty = false;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		// SetMountState
		vehicle.rootContraption.SetMountState(Mount.State.Enabled);

		if (selectionPrefab != null)
		{
			SetSelectionPrefab(selectionPrefab);
		}
	}

	private void Update()
	{
		CheckRayCast();
		CheckRotate();
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			mountSelector++;
			isDirty = true;
		}
	}

	private void CheckRotate()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			selectedInstance.transform.Rotate(0, 90, 0, Space.World);
			currentRotation = selectedInstance.transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			selectedInstance.transform.Rotate(0, -90, 0, Space.World);
			currentRotation = selectedInstance.transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.R))
		{
			selectedInstance.transform.Rotate(0, 0, 90, Space.World);
			currentRotation = selectedInstance.transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			selectedInstance.transform.Rotate(0, 0, -90, Space.World);
			currentRotation = selectedInstance.transform.rotation;
		}
	}

	private void OnDrawGizmos()
	{
		// Draw RayCast
		Gizmos.color = color;
		Gizmos.DrawSphere(rayCastPos, 0.1f);
	}

	#region Building

	/// <summary>
	/// Check the raycast hit and try to mount the selected instance onto the mountee
	/// </summary>
	private void CheckRayCast()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mountLayer))
		{
			// because RayCastHit redirects hit.transform to the it's parent rigidbody,
			// so using hit.collider to get the actual ray hit collider.
			if (hit.collider.TryGetComponent(out Mount mountee))
			{
				// If clicked, and mounting is successful, build the instance
				if (Input.GetMouseButtonDown(0) && TryMounting(mountee)) BuildInstanceOn(mountee);

				// If nothing changed, return
				if (!isDirty && focusedMount == mountee) return;
				focusedMount = mountee;

				// Something changed, update the mounting position
				bool success = TryMounting(mountee);
				if (success) color = Color.green;
				else color = Color.red;
			}
			else
			{
				color = Color.yellow;
			}
			rayCastPos = hit.point;
		}
		else
		{
			selectedInstance.transform.position = new Vector3(0, -100, 0);
			rayCastPos = ray.GetPoint(10);
			color = Color.blue;
		}
	}

	private void BuildInstanceOn(Mount mountOn)
	{
		Part mountOnPart = mountOn.GetParent();
		Contraption parentContraption = mountOnPart.GetContraption();
		if (parentContraption != null)
		{
			parentContraption.AddPart(selectedInstance);
			selectedInstance = null;
		}
		else
		{
			Contraption root = mountOnPart.GetComponent<Contraption>();
			if (root != null)
			{
				root.AddPart(selectedInstance);
				selectedInstance = null;
			}
			else
			{
				Debug.LogError("Parent doesn't have Contraption component");
			}
		}

		SpawnNewInstance();
	}

	/// <summary>
	/// Try placing selected instance onto the mountee
	/// </summary>
	/// <returns> if the selected instance is able to place down. </returns>
	private bool TryMounting(Mount mountee)
	{
		Mount mount = FindMatchingMount(mountee);
		if (mount == null) return false;

		CalculateMounting(mountee, mount);
		return true;
	}

	private Mount FindMatchingMount(Mount mountee)
	{
		Mount[] matchingMounts = selectedInstance.FindMatchingMounts(mountee);
		if (matchingMounts.Length == 0)
		{
			color = Color.red;
			return null;
		}
		mountSelector %= matchingMounts.Length;
		return matchingMounts[mountSelector];
	}

	/// <summary>
	/// Calculate the mounting position of the selected instance where the mounter placed on the mountee
	/// </summary>
	private void CalculateMounting(Mount mountee, Mount mounter)
	{
		Vector3 mountingPos = mountee.transform.position;

		Vector3 mountingLocalPos = mounter.transform.position - selectedInstance.transform.position;
		mountingPos -= mountingLocalPos;

		selectedInstance.transform.position = mountingPos;
		color = Color.green;
	}

	#endregion

	public void SetSelectionPrefab(Part prefab)
	{
		selectionPrefab = prefab;
		SpawnNewInstance();
	}

	private void SpawnNewInstance()
	{
		if (selectedInstance != null) Destroy(selectedInstance.gameObject);

		selectedInstance = Instantiate(selectionPrefab);
		selectedInstance.SetMountState(Mount.State.ShowOnly);
		selectedInstance.transform.rotation = currentRotation;
	}

	public bool ComputeColliderPenetration(Collider[] collidersA, Vector3 posA, Quaternion rotA, Collider[] collidersB, Vector3 posB, Quaternion rotB, out Vector3 dir, out float dist)
	{
		foreach (Collider colliderA in collidersA)
		{
			foreach (Collider colliderB in collidersB)
			{
				if (Physics.ComputePenetration(colliderA, posA, rotA, colliderB, posB, rotB, out dir, out dist))
				{
					return true;
				}
			}
		}
		dir = Vector3.zero;
		dist = 0;
		return false;
	}
}