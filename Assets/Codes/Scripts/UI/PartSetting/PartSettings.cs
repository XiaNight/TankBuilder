using System;
using UnityEngine;

public abstract class PartSettingField : MonoBehaviour
{
	public abstract bool TryInstantiateField(string name, object value);
	public abstract Type ValueType { get; }
	public event Action<object> OnValueChanged;
	protected object value;

	public virtual void SetValue(object value)
	{
		this.value = value;
	}

	protected void SetValueInternal(object value)
	{
		SetValue(value);
		OnValueChanged?.Invoke(value);
	}
}