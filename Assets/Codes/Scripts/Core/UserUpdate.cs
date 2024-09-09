using UnityEngine;
using System.Collections.Generic;

public class UserUpdate : MonoBehaviour
{
	#region Singleton
	public static UserUpdate Instance { get; private set; }
	#endregion

	[SerializeField] private List<IUserUpdate> interfaces = new();

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		foreach (IUserUpdate i in interfaces)
		{
			i.UserLoop();
		}
	}

	private void FixedUpdate()
	{
		foreach (IUserUpdate i in interfaces)
		{
			i.UserFixedUpdate();
		}
	}

	public void AddInterface(IUserUpdate i)
	{
		interfaces.Add(i);
	}

	public void RemoveInterface(IUserUpdate i)
	{
		interfaces.Remove(i);
	}

	public void Clear()
	{
		interfaces.Clear();
	}
}


public interface IUserUpdate
{
	public void UserLoop() { }
	void UserFixedUpdate() { }
}