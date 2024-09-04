using System;
using UnityEngine;

public interface ISettingField
{
	string Key { get; }
	string DisplayName { get; }
	object Value { get; set; }
	Type ValueType { get; }
	void SetValue(object value);
	bool VerifyValue(object value, out object fixedValue);
}

[Serializable]
public class SettingField<T> : ISettingField
{
	[SerializeField] protected string key;
	[SerializeField] protected string displayName;
	public string Key => key;
	public string DisplayName => displayName;

	[SerializeField] protected T value;
	public T Value => value;

	public event Action<T> OnValueChangedEvent;

	object ISettingField.Value { get => value; set => this.value = (T)value; }
	Type ISettingField.ValueType => typeof(T);

	public SettingField(string name, T value)
	{
		this.key = name;
		this.value = value;
	}

	public virtual void SetValue(T value)
	{
		VerifyValue(value, out object fixedValue);
		this.value = (T)fixedValue;
		OnValueChangedEvent?.Invoke((T)fixedValue);
	}

	public void SetValue(object value)
	{
		SetValue((T)value);
	}

	public virtual bool VerifyValue(object value, out object fixedValue)
	{
		fixedValue = value;
		return true;
	}
}

[Serializable]
public class IntSettingField : SettingField<int>
{
	public bool minimumClamping;
	public int minValue;
	public bool maximumClamping;
	public int maxValue;
	public IntSettingField(string name, int value) : base(name, value) { }

	public override bool VerifyValue(object value, out object fixedValue)
	{
		bool isValid = true;
		if (value is int intValue)
		{
			if (minimumClamping && intValue < minValue)
			{
				intValue = minValue;
				isValid = false;
			}
			if (maximumClamping && intValue > maxValue)
			{
				intValue = maxValue;
				isValid = false;
			}
			fixedValue = intValue;
			return isValid;
		}
		fixedValue = this.value;
		return false;
	}
}

[Serializable]
public class FloatSettingField : SettingField<float>
{
	public bool minimumClamping;
	public float minValue;
	public bool maximumClamping;
	public float maxValue;
	public FloatSettingField(string name, float value) : base(name, value) { }

	public override bool VerifyValue(object value, out object fixedValue)
	{
		bool isValid = true;
		if (value is float floatValue)
		{
			if (minimumClamping && floatValue < minValue)
			{
				floatValue = minValue;
				isValid = false;
			}
			if (maximumClamping && floatValue > maxValue)
			{
				floatValue = maxValue;
				isValid = false;
			}
			fixedValue = floatValue;
			return isValid;
		}
		fixedValue = this.value;
		return false;
	}
}

[Serializable]
public class BoolSettingField : SettingField<bool>
{
	public BoolSettingField(string name, bool value) : base(name, value) { }
}