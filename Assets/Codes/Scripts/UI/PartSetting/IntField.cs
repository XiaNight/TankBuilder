using UnityEngine;
using TMPro;

public class IntField : PartSettingField
{
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_InputField inputField;

	public override System.Type ValueType => typeof(int);

	public override bool TryInstantiateField(string name, object initialValue)
	{
		//- Setup
		titleText.SetText(name);
		inputField.text = initialValue.ToString();

		inputField.onEndEdit.AddListener((string value) =>
		{
			if (int.TryParse(value, out int intValue))
			{
				SetValueInternal(intValue);
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