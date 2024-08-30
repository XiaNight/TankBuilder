using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TextToggleable : Toggleable
{
	public Text label;
	public string enabledText;
	public string disabledText;

	public override void Disable()
	{
		label.text = disabledText;
	}

	public override void Enable()
	{
		label.text = enabledText;
	}
}
