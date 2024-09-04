using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Plate : Part
{
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private SettingField<float> thicknessField;
	[SerializeField] private SettingField<float> offsetField;
	private Mesh mesh;
	private Thickness thickness;

	private void RebuildMesh()
	{
		if (mesh != null) Destroy(mesh);
		mesh = new();
		BuildMesh(mesh);
		meshFilter.mesh = mesh;
	}

	private void BuildMesh(Mesh mesh)
	{
		mesh.Clear();

		float offset = this.offsetField.Value / 1000;

		Vector3[] rawVerts = new Vector3[]
		{
			new(-Constants.HALF_GRID, offset, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, offset, Constants.HALF_GRID),
			new(Constants.HALF_GRID, offset, Constants.HALF_GRID),
			new(Constants.HALF_GRID, offset, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, thickness.MM + offset, -Constants.HALF_GRID),
			new(-Constants.HALF_GRID, thickness.MM + offset, Constants.HALF_GRID),
			new(Constants.HALF_GRID, thickness.MM + offset, Constants.HALF_GRID),
			new(Constants.HALF_GRID, thickness.MM + offset, -Constants.HALF_GRID),
		};

		List<Vector3> verticies = new();
		List<int> triangles = new();
		List<Vector2> uvs = new();


		int indexOffset = 0;
		MakeFace(rawVerts, new int[] { 0, 1, 2, 3 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 1, 5, 6, 2 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 5, 4, 7, 6 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 4, 0, 3, 7 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 3, 2, 6, 7 }, ref indexOffset, verticies, triangles, uvs);
		MakeFace(rawVerts, new int[] { 0, 4, 5, 1 }, ref indexOffset, verticies, triangles, uvs);


		mesh.vertices = verticies.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
	}

	private void MakeFace(Vector3[] rawVerts, int[] poses, ref int indexOffset, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs)
	{
		verticies.AddRange(new Vector3[]
		{
			rawVerts[poses[0]],
			rawVerts[poses[1]],
			rawVerts[poses[2]],
			rawVerts[poses[3]],
		});

		triangles.AddRange(new int[]
		{
			0 + indexOffset, 2 + indexOffset, 1 + indexOffset,
			0 + indexOffset, 3 + indexOffset, 2 + indexOffset,
		});

		uvs.AddRange(new Vector2[]
		{
			new(0, 0),
			new(0, 1),
			new(1, 1),
			new(1, 0),
		});

		indexOffset += 4;
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

		RebuildMesh();
	}

	#endregion

	[Serializable]
	public struct Thickness
	{
		public float value;
		public readonly float MM => value / 1000;

		public Thickness(float value)
		{
			this.value = value;
		}
	}
}