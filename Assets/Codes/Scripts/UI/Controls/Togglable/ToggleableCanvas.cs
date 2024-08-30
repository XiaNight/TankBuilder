using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleableCanvas : Toggleable
{
	public CanvasGroup canvas;
	public float enabledAlpha = 1;
	public float disabledAlpha = 0;
	public float fadeTime;
	public bool updateInteractable = true;
	public bool updateCanvasState = false;

	private void Reset()
	{
		canvas = GetComponent<CanvasGroup>();
	}

	public override void Disable()
	{
		if (updateInteractable)
		{
			canvas.interactable = false;
			canvas.blocksRaycasts = false;
		}

		// if not active in hierarchy, don't use fade
		if (gameObject.activeInHierarchy)
		{
			if (Application.isPlaying)
			{
				StartCoroutine(FadeCanvasGroup(disabledAlpha, fadeTime, () =>
				{
					if (updateCanvasState)
					{
						gameObject.SetActive(false);
					}
				}));
			}
			else
			{
				canvas.alpha = disabledAlpha;
				if (updateCanvasState)
				{
					gameObject.SetActive(false);
				}
			}
		}
		else
		{
			canvas.alpha = disabledAlpha;
			return;
		}
	}

	public override void Enable()
	{
		if (updateCanvasState)
		{
			gameObject.SetActive(true);
		}
		if (updateInteractable)
		{
			canvas.interactable = true;
			canvas.blocksRaycasts = true;
		}

		// if not active in hierarchy, don't use fade
		if (gameObject.activeInHierarchy)
		{
			if (Application.isPlaying)
			{
				StartCoroutine(FadeCanvasGroup(enabledAlpha, fadeTime));
			}
			else
			{
				canvas.alpha = enabledAlpha;
			}
		}
		else
		{
			canvas.alpha = enabledAlpha;
			return;
		}
	}

	private IEnumerator FadeCanvasGroup(float targetAlpha, float duration, UnityAction callback = null)
	{
		float currentTime = 0;
		float startAlpha = canvas.alpha;
		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
			yield return null;
		}
		canvas.alpha = targetAlpha;
		callback?.Invoke();
	}
}