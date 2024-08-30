using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Toggleable : MonoBehaviour
{
	public abstract void Enable();
	public abstract void Disable();
	public void SetState(bool state)
	{
		if (state)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}
}
