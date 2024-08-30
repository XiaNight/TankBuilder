using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FloatField : PartSettingField
{
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_InputField inputField;

	public override Type ValueType => typeof(float);

	public override bool TryInstantiateField(string name, object value)
	{
		Debug.Log($"FloatField.TryInstantiateField({name}, {value})");
		//- Setup
		titleText.SetText(name);
		inputField.text = value.ToString();

		inputField.onEndEdit.AddListener((string value) =>
		{
			if (float.TryParse(value, out float floatValue))
			{
				SetValueInternal(floatValue);
			}
		});

		return true;
	}

	public override void SetValue(object value)
	{
		base.SetValue(value);
		inputField.text = value.ToString();
	}
}