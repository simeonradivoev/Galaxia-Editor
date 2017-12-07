using UnityEditor;
using UnityEngine;
using Galaxia;

namespace GalaxyGeneratorEditor
{
    [CustomPropertyDrawer(typeof(CurveRangeAttribute))]
    public class CurveRangeProperyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
			CurveRangeAttribute range = attribute as CurveRangeAttribute;
	        if (range != null)
	        {
				EditorGUI.CurveField(position, property, Color.green, range.range);
	        }
        }
    }
}
