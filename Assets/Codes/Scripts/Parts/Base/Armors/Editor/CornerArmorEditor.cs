using UnityEditor;

[CustomEditor(typeof(CornerArmor))]
public class CornerArmorEditor : ArmorPlateEditor<CornerArmor> { }

[CustomEditor(typeof(FlatArmor))]
public class FlatArmorEditor : ArmorPlateEditor<FlatArmor> { }

[CustomEditor(typeof(SlopeArmor))]
public class SlopeArmorEditor : ArmorPlateEditor<SlopeArmor> { }

[CustomEditor(typeof(TriangleArmor))]
public class TriangleArmorEditor : ArmorPlateEditor<TriangleArmor> { }

[CustomEditor(typeof(OuterCornerArmor))]
public class OuterCornerArmorEditor : ArmorPlateEditor<OuterCornerArmor> { }