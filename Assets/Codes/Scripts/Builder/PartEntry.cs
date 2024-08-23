using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartEntry : MonoBehaviour
{
	[SerializeField] private PartData part;
	[SerializeField] private Image image;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text description;
	[SerializeField] private Button button;


	public Part Part => part.Prefab;

	private void OnValidate()
	{
		if (part == null) return;
		SetData();
	}

	private void Awake()
	{
		button.onClick.AddListener(OnClick);
	}

	public void Start()
	{
		SetData();
	}

	public void SetData()
	{
		image.sprite = part.Icon;
		title.text = part.Name;
		description.text = part.description;
	}

	public void OnClick()
	{
		Builder.Instance.SetSelectedPartData(part);
	}
}