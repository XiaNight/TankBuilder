using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TriangleArmor : ArmorPlate
{
	[SerializeField] private SettingField<float> thicknessField;
	[SerializeField] private SettingField<float> offsetField;
	private Thickness thickness;

	public override void OnSpawned()
	{
		base.OnSpawned();
		thicknessField.OnValueChangedEvent += _ => RebuildMesh();
		offsetField.OnValueChangedEvent += _ => RebuildMesh();
	}

	public override void RebuildMesh()
	{
		if (mesh != null) Destroy(mesh);
		thickness = new(thicknessField.Value);

		mesh = new();
		ConstructMesh(mesh, new ArmorValue[]
		{
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
			new(thickness, offsetField.Value),
		});
		ApplyMesh();
	}

	protected override void ConstructMesh(Mesh mesh, in ArmorValue[] av)
	{
		Vector3[] rawVerts = new Vector3[6]
		{
			new(-Constants.HALF_GRID, av[0].OffsetMeters, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, av[1].OffsetMeters, Constants.HALF_GRID),
			new(Constants.HALF_GRID, av[2].OffsetMeters, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, av[0].ThicknessMeters + av[0].OffsetMeters, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, av[1].ThicknessMeters + av[1].OffsetMeters, Constants.HALF_GRID),
			new(Constants.HALF_GRID, av[2].ThicknessMeters + av[2].OffsetMeters, -Constants.HALF_GRID)
		};

		List<Vector3> verticies = new();
		List<int> triangles = new();
		List<Vector2> uvs = new();

		int indexOffset = 0;
		MakeFace(rawVerts, new int[] { 0, 1, 2 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 3, 5, 4 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 0, 2, 5, 3 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 1, 0, 3, 4 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 2, 1, 4, 5 }, ref indexOffset, verticies, triangles, uvs);

		mesh.vertices = verticies.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
	}

	public override List<ISettingField> OpenSettings()
	{
		List<ISettingField> settings = base.OpenSettings();

		settings.Add(thicknessField);
		settings.Add(offsetField);

		return settings;
	}

	#region Serialization

	public override JObject Serialize()
	{
		JObject data = base.Serialize();

		data[thicknessField.Key] = thicknessField.Value;
		data[offsetField.Key] = offsetField.Value;

		return data;
	}

	public override void Deserialize(JObject data)
	{
		base.Deserialize(data);

		thicknessField.SetValue(new Thickness(data[thicknessField.Key].Value<float>()));
		offsetField.SetValue(data[offsetField.Key].Value<float>());
	}

	#endregion
}