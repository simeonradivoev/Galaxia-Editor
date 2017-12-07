using UnityEditor;

namespace GalaxyGeneratorEditor
{
    public static class GUIUtils
    {
        public delegate void DoElementGUI(SerializedProperty element);

        public static void DoList(SerializedProperty array, SerializedObject obj)
        {
            DoList(array, obj, DoDefaultElementGUI);
        }
        public static void DoList(SerializedProperty array, SerializedObject obj, DoElementGUI function)
        {
            for (int i = 0; i < array.arraySize;i++ )
            {
                function(array.GetArrayElementAtIndex(i));
            }

            obj.ApplyModifiedProperties();
        }

        private static void DoDefaultElementGUI(SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop,true);
        }
    }
}
