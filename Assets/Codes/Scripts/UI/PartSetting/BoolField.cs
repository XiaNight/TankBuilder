using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoolField : PartSettingField
{
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private Toggle toggle;

	public override Type ValueType => typeof(bool);

	public override bool TryInstantiateField(string name, object value)
	{
		titleText.SetText(name);
		toggle.isOn = (bool)value;

		toggle.onValueChanged.AddListener((bool value) =>
		{
			SetValueInternal(value);
		});

		return true;
	}

	public override void SetValue(object value)
	{
		base.SetValue(value);
		toggle.isOn = (bool)value;
	}
}