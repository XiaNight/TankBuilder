using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPlate : Part
{
	[SerializeField] protected MeshFilter meshFilter;
	protected Mesh mesh;

	public override void OnSpawned()
	{
		RebuildMesh();
	}

	public virtual void RebuildMesh()
	{

	}

	public struct ArmorValue
	{
		public Thickness thickness;
		public float offset;

		public readonly float ThicknessMeters => thickness.Meter;
		public readonly float OffsetMeters => offset / 1000;

		public ArmorValue(Thickness thickness, float offset)
		{
			this.thickness = thickness;
			this.offset = offset;
		}
	}

	protected virtual void ConstructMesh(Mesh mesh, in ArmorValue[] armorValues)
	{
		Vector3[] rawVerts = new Vector3[8]
		{
			new(-Constants.HALF_GRID, armorValues[0].OffsetMeters, -Constants.HALF_GRID), 				//- ( --- ) SW
			new(-Constants.HALF_GRID, armorValues[1].OffsetMeters, Constants.HALF_GRID), 				//- ( --+ ) NW
			new(Constants.HALF_GRID, armorValues[2].OffsetMeters, Constants.HALF_GRID), 					//- ( +-+ ) NE
			new(Constants.HALF_GRID, armorValues[3].OffsetMeters, -Constants.HALF_GRID), 				//- ( +-- ) SE
			new(-Constants.HALF_GRID, armorValues[0].ThicknessMeters + armorValues[0].OffsetMeters, -Constants.HALF_GRID), //- ( -+- ) SW
			new(-Constants.HALF_GRID, armorValues[1].ThicknessMeters + armorValues[1].OffsetMeters, Constants.HALF_GRID),	//- ( -++ ) NW
			new(Constants.HALF_GRID, armorValues[2].ThicknessMeters + armorValues[2].OffsetMeters, Constants.HALF_GRID), 	//- ( +++ ) NE
			new(Constants.HALF_GRID, armorValues[3].ThicknessMeters + armorValues[3].OffsetMeters, -Constants.HALF_GRID),   //- ( ++- ) SE
		};

		BuildPolyhedron(mesh, rawVerts);
	}

	///	5--------6
	///	|\       |\
	///	| \      | \
	///	|  4--------7
	///	|  |     |  |
	///	1--|-----2  |
	///	 \ |      \ |
	///	  \|       \|
	///	   0--------3
	protected static void BuildPolyhedron(Mesh mesh, in Vector3[] rawVerts)
	{
		if (rawVerts.Length != 8)
		{
			Debug.LogError("Invalid number of vertices for polyhedron");
			return;
		}
		mesh.Clear();

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

	protected static void MakeFace(in Vector3[] rawVerts, in int[] poses, ref int indexOffset, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs)
	{
		if (poses.Length > rawVerts.Length)
		{
			Debug.LogError("Invalid number of vertices for face");
			return;
		}
		switch (poses.Length)
		{
			case 3:
				MakeTriangle(rawVerts, poses, ref indexOffset, verticies, triangles, uvs);
				break;
			case 4:
			case > 4:
				MakeQuad(rawVerts, poses, ref indexOffset, verticies, triangles, uvs);
				break;
		}
	}

	private static void MakeTriangle(in Vector3[] rawVerts, in int[] poses, ref int indexOffset, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs)
	{
		verticies.AddRange(new Vector3[]
				{
			rawVerts[poses[0]],
			rawVerts[poses[1]],
			rawVerts[poses[2]],
				});

		triangles.AddRange(new int[]
		{
			0 + indexOffset, 2 + indexOffset, 1 + indexOffset,
		});

		uvs.AddRange(new Vector2[]
		{
			new(0, 0),
			new(0, 1),
			new(1, 1),
		});

		indexOffset += 3;
	}

	private static void MakeQuad(in Vector3[] rawVerts, in int[] poses, ref int indexOffset, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs)
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

	[Serializable]
	public struct Thickness
	{
		/// <summary>
		/// Thickness in millimeters
		/// </summary>
		public float value;
		public readonly float Meter => value / 1000;

		public Thickness(float value)
		{
			this.value = value;
		}
	}
}