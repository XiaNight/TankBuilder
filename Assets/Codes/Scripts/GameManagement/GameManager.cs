using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	public Vehicle playerVehicle;
	public State gameState;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{

	}

	private void Update()
	{
		gameState.Update();
	}

	public class State
	{
		public virtual void Update() { }
	}
}