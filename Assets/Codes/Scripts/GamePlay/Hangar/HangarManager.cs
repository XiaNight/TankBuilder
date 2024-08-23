using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarManager : MonoBehaviour
{
	public static HangarManager Instance { get; private set; }
	public HangarFreeCam freeCam;
	public Vehicle playerVehicle;
	public State gameState;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		gameState = new BuildingState();
		freeCam = FindObjectOfType<HangarFreeCam>();
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
