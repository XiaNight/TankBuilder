using System;
using UnityEngine;

public class Builder : MonoBehaviour
{
	#region Singleton
	public static Builder Instance { get; private set; }
	#endregion

	public Vehicle vehicle;
	public PartData selectedPartData;

	public LayerMask mountLayer;
	public LayerMask buildFloorLayer;
	public LayerMask partLayer;
	public LayerMask interactionLayer;

	private Vector3 centerScreen;

	//- Building
	private Part previewInstance;
	private int mountSelector = 0;
	private Quaternion currentRotation = Quaternion.identity;
	private bool isBuilding = true;

	public event Action<Part> OnPartInteractionEnter;
	public event Action<Part> OnPartInteractionExit;
	public event Action<Part> OnPartInteractionHover;
	public event Action<Part, KeyCode> OnPartOverKeyPressed;
	public event Action<Part, int> OnPartMousePressed;

	//- Debug
	private Vector3 rayCastPos = new();
	private Color color = Color.red;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void Enable()
	{
		SpawnNewPreview();
		gameObject.SetActive(true);
	}

	public void Disable()
	{
		RemovePreview();
		gameObject.SetActive(false);
	}

	private void Start()
	{
		// SetMountState
		vehicle.rootContraption.SetMountState(Mount.State.Enabled);

		if (selectedPartData != null)
		{
			SetSelectedPartData(selectedPartData);
		}

		centerScreen = new Vector3(Screen.width / 2, Screen.height / 2);
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(centerScreen);

		RayCastInteraction(ray);
		if (isBuilding)
		{
			RayCastMounting(ray);
			CheckRotate();
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				mountSelector++;
			}
		}
	}

	private void CheckRotate()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			previewInstance.transform.Rotate(90, 0, 0, Space.World);
			currentRotation = previewInstance.transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			previewInstance.transform.Rotate(0, 90, 0, Space.World);
			currentRotation = previewInstance.transform.rotation;
		}
		else if (Input.GetKeyDown(KeyCode.R))
		{
			previewInstance.transform.Rotate(0, 0, 90, Space.World);
			currentRotation = previewInstance.transform.rotation;
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
	private void RayCastMounting(in Ray ray)
	{
		if (previewInstance == null) return;
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mountLayer + buildFloorLayer))
		{
			// because RayCastHit redirects hit.transform to the it's parent rigidbody,
			// so using hit.collider to get the actual ray hit collider.
			if (hit.collider.TryGetComponent(out Mount mountee))
			{
				// If clicked, and mounting is successful, build the instance
				if (Input.GetMouseButtonDown(0) && TryMounting(mountee))
				{
					BuildInstanceOn(mountee);
				}

				// Something changed, update the mounting position
				bool success = TryMounting(mountee);
				if (success) color = Color.green;
				else color = Color.red;
			}
			else //- Not hitting any mounting point.
			{
				//- Place on floor
				// if hit the build floor, move the selected instance to the hit point, and grid it to 0.25f
				Vector3 pos = hit.point;
				pos.x = Mathf.Round(pos.x * 4) * 0.25f;
				pos.z = Mathf.Round(pos.z * 4) * 0.25f;
				previewInstance.transform.position = pos;

				if (Input.GetMouseButtonDown(0)) Buildinstance(previewInstance, vehicle.rootContraption);
				if (Input.GetMouseButtonDown(1)) RemovePreview();

				color = Color.blue;
			}

			// Debug hit point
			rayCastPos = hit.point;
		}
		else //- Hit nothing.
		{
			//- Hide the selected instance outside view.
			previewInstance.transform.position = new Vector3(0, -100, 0);
			rayCastPos = ray.GetPoint(10);
			color = Color.blue;
		}
	}

	private void BuildInstanceOn(Mount mountOn)
	{
		Contraption contraption = mountOn.GetParent().GetContraption();

		if (contraption == null) contraption = vehicle.rootContraption;

		Buildinstance(previewInstance, contraption);
	}

	private void Buildinstance(Part part, Contraption contraption)
	{
		contraption.AddPart(part);
		part.SetColliderState(true);
		part.SetInteractionCollidersState(true);
		previewInstance = null;
		SpawnNewPreview();
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
		Mount[] matchingMounts = previewInstance.FindMatchingMounts(mountee);
		if (matchingMounts.Length == 0)
		{
			color = Color.cyan;
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

		Vector3 mountingLocalPos = mounter.transform.position - previewInstance.transform.position;
		mountingPos -= mountingLocalPos;

		previewInstance.transform.position = mountingPos;
		color = Color.green;
	}

	#endregion

	#region Interaction

	private Part lastInteractedPart;

	private void RayCastInteraction(in Ray ray)
	{
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactionLayer))
		{
			if (hit.collider.GetComponentInParent<Part>() is Part part)
			{
				//- Mouse Enter/Over/Exit
				if (lastInteractedPart != part)
				{
					if (lastInteractedPart != null)
					{
						OnPartInteractionExit?.Invoke(lastInteractedPart);
						lastInteractedPart.OnMouseExit();
					}
					OnPartInteractionEnter?.Invoke(part);
					part.OnMouseEnter();
					lastInteractedPart = part;
				}
				part.OnMouseOver();

				//- Mouse Over Key Pressed
				if (Input.anyKeyDown) OnPartOverKeyPressed?.Invoke(part, Event.current.keyCode);
				if (Input.GetMouseButtonDown(0)) OnPartMousePressed?.Invoke(part, 0);
				if (Input.GetMouseButtonDown(1)) OnPartMousePressed?.Invoke(part, 1);
				if (Input.GetMouseButtonDown(2)) OnPartMousePressed?.Invoke(part, 2);

				//- Remove Part
				if (isBuilding && Input.GetMouseButtonDown(1))
				{
					Contraption contraption = part.GetContraption();
					contraption.RemovePart(part);
					Destroy(part.gameObject);
				}

				return;
			}
		}
		if (lastInteractedPart != null)
		{
			OnPartInteractionExit?.Invoke(lastInteractedPart);
			lastInteractedPart.OnMouseExit();
			lastInteractedPart = null;
		}
	}

	#endregion

	public void SetBuildingState(bool isBuilding)
	{
		this.isBuilding = isBuilding;
		if (!isBuilding)
		{
			RemovePreview();
		}
		else
		{
			SpawnNewPreview();
		}
	}

	public void SetSelectedPartData(PartData partData)
	{
		selectedPartData = partData;
		SpawnNewPreview();
	}

	public void RemovePreview()
	{
		if (previewInstance != null) Destroy(previewInstance.gameObject);
	}

	private void SpawnNewPreview()
	{
		RemovePreview();

		previewInstance = Instantiate(selectedPartData.prefab);
		previewInstance.SetMetaData(selectedPartData);
		previewInstance.SetMountState(Mount.State.ShowOnly);
		previewInstance.SetColliderState(false);
		previewInstance.SetInteractionCollidersState(false);
		previewInstance.transform.rotation = currentRotation;
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