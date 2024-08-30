using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions.Must;

public class RectTransformToggleable : Toggleable
{
	public RectData enabledData;
	public RectData disabledData;

	public override void Disable()
	{
		JumpToData(enabledData);
		EaseToData(disabledData);
	}

	public override void Enable()
	{
		JumpToData(disabledData);
		EaseToData(enabledData);
	}

	private void JumpToData(RectData data)
	{
		RectTransform rect = transform as RectTransform;

		if (data.anchoredPosition) rect.anchoredPosition = data.anchoredPosition.value;
		if (data.anchorMin) rect.anchorMin = data.anchorMin.value;
		if (data.anchorMax) rect.anchorMax = data.anchorMax.value;
		if (data.sizeDelta) rect.sizeDelta = data.sizeDelta.value;
		if (data.pivot) rect.pivot = data.pivot.value;
		if (data.localPosition) rect.localPosition = data.localPosition;
		if (data.localScale) rect.localScale = data.localScale;
		if (data.localEulerAngles) rect.localEulerAngles = data.localEulerAngles;
	}

	private void EaseToData(RectData data)
	{
		RectTransform rect = transform as RectTransform;

		data.anchoredPosition.EaseTo(() => rect.anchoredPosition, x => rect.anchoredPosition = x, data.anchoredPosition, data.easeTime);
		data.anchorMin.EaseTo(() => rect.anchorMin, x => rect.anchorMin = x, data.anchorMin, data.easeTime);
		data.anchorMax.EaseTo(() => rect.anchorMax, x => rect.anchorMax = x, data.anchorMax, data.easeTime);
		data.sizeDelta.EaseTo(() => rect.sizeDelta, x => rect.sizeDelta = x, data.sizeDelta, data.easeTime);
		data.pivot.EaseTo(() => rect.pivot, x => rect.pivot = x, data.pivot, data.easeTime);
		data.localPosition.EaseTo(() => rect.localPosition, x => rect.localPosition = x, data.localPosition, data.easeTime);
		data.localScale.EaseTo(() => rect.localScale, x => rect.localScale = x, data.localScale, data.easeTime);
		data.localEulerAngles.EaseTo(() => rect.localEulerAngles, x => rect.localEulerAngles = x, data.localEulerAngles, data.easeTime);
	}

	[System.Serializable]
	public struct RectData
	{
		public ToggleDataVector2 anchoredPosition;
		public ToggleDataVector2 anchorMin;
		public ToggleDataVector2 anchorMax;
		public ToggleDataVector2 sizeDelta;
		public ToggleDataVector2 pivot;
		public ToggleDataVector3 localPosition;
		public ToggleDataVector3 localScale;
		public ToggleDataVector3 localEulerAngles;
		public float easeTime;

		public void CopyRectTransform(RectTransform rect)
		{
			anchoredPosition.value = rect.anchoredPosition;
			anchorMin.value = rect.anchorMin;
			anchorMax.value = rect.anchorMax;
			sizeDelta.value = rect.sizeDelta;
			pivot.value = rect.pivot;
			localPosition.value = rect.localPosition;
			localScale.value = rect.localScale;
			localEulerAngles.value = rect.localEulerAngles;
		}

		[System.Serializable]
		public abstract class ToggleData<T>
		{
			public bool enabled;
			public T value;
			protected Tween tween;

			public static implicit operator bool(ToggleData<T> toggleData)
			{
				return toggleData.enabled;
			}

			public static implicit operator T(ToggleData<T> toggleData)
			{
				return toggleData.value;
			}

			public delegate T Getter();
			public delegate void Setter(T value);

			public abstract void EaseTo(Getter getter, Setter setter, T value, float time);
		}

		[System.Serializable]
		public sealed class ToggleDataVector2 : ToggleData<Vector3>
		{
			public override void EaseTo(Getter getter, Setter setter, Vector3 value, float time)
			{
				if (Application.isPlaying)
				{
					if (enabled)
					{
						if (tween != null) tween.Kill();
						tween = DOTween.To(() => getter(), x => setter(x), value, time);
					}
				}
				else
				{
					setter(value);
				}
			}
		}

		[System.Serializable]
		public sealed class ToggleDataVector3 : ToggleData<Vector3>
		{
			public override void EaseTo(Getter getter, Setter setter, Vector3 value, float time)
			{
				if (Application.isPlaying)
				{
					if (enabled)
					{
						if (tween != null) tween.Kill();
						tween = DOTween.To(() => getter(), x => setter(x), value, time);
					}
				}
				else
				{
					setter(value);
				}
			}
		}
	}
}
