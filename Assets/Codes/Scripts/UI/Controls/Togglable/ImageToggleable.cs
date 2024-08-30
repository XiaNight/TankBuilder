using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageToggleable : Toggleable
{
	public Image image;
	public Sprite enabledSprite;
	public Sprite disabledSprite;
	public Color enabledColor = Color.white;
	public Color disabledColor = Color.white;
	public float crossfadeDurationForward = 0.0f;
	public float crossfadeDurationBackward = 0.0f;

	private void Reset()
	{
		image = GetComponent<Image>();
	}

	public override void Disable()
	{
		if (disabledSprite != null)
		{
			image.sprite = disabledSprite;
		}
		image.CrossFadeColor(disabledColor, crossfadeDurationBackward, true, true);
	}

	public override void Enable()
	{
		if (enabledSprite != null)
		{
			image.sprite = enabledSprite;
		}
		image.CrossFadeColor(enabledColor, crossfadeDurationForward, true, true);
	}
}
