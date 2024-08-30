using System;
using System.Collections.Generic;
using UnityEngine;

public class PartFieldManager : MonoBehaviour
{
	#region Singleton
	public static PartFieldManager Instance { get; private set; }
	#endregion

	public List<PartSettingField> partSettingFieldPrefabs;
	public Transform content;

	private readonly Dictionary<Type, PartSettingField> partSettingFieldPrefabMap = new();
	private readonly List<PartSettingField> spawnedFields = new();

	private void Awake()
	{
		//- Singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		foreach (PartSettingField partSettingField in partSettingFieldPrefabs)
		{
			partSettingFieldPrefabMap.Add(partSettingField.ValueType, partSettingField);
		}
	}

	public void SetupFields(List<ISettingField> fields)
	{
		ClearFields();
		foreach (ISettingField field in fields)
		{
			if (partSettingFieldPrefabMap.TryGetValue(field.ValueType, out PartSettingField fieldPrefab))
			{
				PartSettingField spawnedField = Instantiate(fieldPrefab, content);
				spawnedField.OnValueChanged += value =>
				{
					bool verified = field.VerifyValue(value, out value);
					field.SetValue(value);
					if (!verified)
					{
						spawnedField.SetValue(field.Value);
					}
				};

				//- Try to instantiate the field
				if (spawnedField.TryInstantiateField(field.Key, field.Value))
				{
					spawnedFields.Add(spawnedField);
				}
				else
				{
					Destroy(spawnedField.gameObject);
				}
			}
		}
	}

	private void ClearFields()
	{
		for (int i = spawnedFields.Count - 1; i >= 0; i--)
		{
			Destroy(spawnedFields[i].gameObject);
		}
		spawnedFields.Clear();
	}
}