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


	public PartData PartData => part;
	public Part Part => part.prefab;

	public event System.Action OnClick;

	private void OnValidate()
	{
		if (part == null) return;
		UpdateData();
	}

	private void Awake()
	{
		button.onClick.AddListener(Select);
	}

	public void Start()
	{
		UpdateData();
	}

	public void SetPart(PartData partData)
	{
		part = partData;
		UpdateData();
	}

	public void UpdateData()
	{
		image.sprite = part.icon;
		title.text = part.Name;
		description.text = part.description;
	}

	public void Select()
	{
		OnClick?.Invoke();
	}

	public void SetHighlight(bool isHighlighted)
	{

	}
}