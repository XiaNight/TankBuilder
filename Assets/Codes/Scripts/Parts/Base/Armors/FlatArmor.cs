using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class FlatArmor : ArmorPlate
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
		Vector3[] rawVerts = new Vector3[8]
		{
			new(-Constants.HALF_GRID, av[0].OffsetMeters - av[0].ThicknessMeters / 2, -Constants.HALF_GRID), 				//- ( --- ) SW
			new(-Constants.HALF_GRID, av[1].OffsetMeters - av[1].ThicknessMeters / 2, Constants.HALF_GRID), 				//- ( --+ ) NW
			new(Constants.HALF_GRID, av[2].OffsetMeters - av[2].ThicknessMeters / 2, Constants.HALF_GRID), 					//- ( +-+ ) NE
			new(Constants.HALF_GRID, av[3].OffsetMeters - av[3].ThicknessMeters / 2, -Constants.HALF_GRID), 				//- ( +-- ) SE
			new(-Constants.HALF_GRID, av[0].OffsetMeters + av[0].ThicknessMeters / 2, -Constants.HALF_GRID), //- ( -+- ) SW
			new(-Constants.HALF_GRID, av[1].OffsetMeters + av[1].ThicknessMeters / 2, Constants.HALF_GRID),	//- ( -++ ) NW
			new(Constants.HALF_GRID, av[2].OffsetMeters + av[2].ThicknessMeters / 2, Constants.HALF_GRID), 	//- ( +++ ) NE
			new(Constants.HALF_GRID, av[3].OffsetMeters + av[3].ThicknessMeters / 2, -Constants.HALF_GRID),   //- ( ++- ) SE
		};

		BuildPolyhedron(mesh, rawVerts);
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

		thicknessField.SetValue(data[thicknessField.Key].Value<float>());
		offsetField.SetValue(data[offsetField.Key].Value<float>());
	}

	#endregion
}