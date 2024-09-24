using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HangarManager : MonoBehaviour
{
	public static HangarManager Instance { get; private set; }
	public HangarFreeCam freeCam;
	public Vehicle playerVehicle;
	public VehicleCamera vehicleCamera;
	public Transform hangarCameraDefaultPosition;

	public Toggleable buildModeToggleGroup;
	public Toggleable editModeToggleGroup;
	public Toggleable testDriveModeToggleGroup;

	[Header("UI")]
	public PartList partList;

	[SerializeField] private TMP_Text modeText;
	private State gameState;

	public void SetModeText(string text) => modeText.text = text;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		SetGameState(new BuildingState());
		freeCam = FindObjectOfType<HangarFreeCam>();
	}

	private void Update()
	{
		gameState.Update();
	}

	public void SetGameState(State state)
	{
		gameState?.OnDisable();
		State lastState = gameState;
		gameState = state;
		gameState.OnEnable(this, lastState);
	}

	public void Ready()
	{
		SceneManager.LoadScene("GamePlay");
	}

	public void Leave()
	{
		SceneManager.LoadScene("Hangar");
	}

	public class State
	{
		protected HangarManager hangar;
		public virtual void Update() { }
		public virtual void OnEnable(HangarManager hangarManager, State lastState)
		{
			hangar = hangarManager;
		}
		public virtual void OnDisable() { }
	}
}
