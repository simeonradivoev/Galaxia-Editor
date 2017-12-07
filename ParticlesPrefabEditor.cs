using UnityEditor;
using UnityEngine;
using System.Collections;
using Galaxia;

namespace GalaxyGeneratorEditor
{
    [CustomEditor(typeof(Galaxia.ParticlesPrefab))]
    [CanEditMultipleObjects]
    public sealed class ParticlesPrefabEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            OnGUI();
        }

        internal bool OnGUI()
        {
            bool changed = false;
            serializedObject.UpdateIfRequiredOrScript();
            serializedObject.targetObject.name = EditorGUILayout.TextField("Name", serializedObject.targetObject.name);
            serializedObject.ApplyModifiedProperties();
	        GUI.enabled = false;
            EditorGUILayout.ObjectField(serializedObject.FindProperty("m_material").objectReferenceValue,typeof(Material),false);
	        GUI.enabled = serializedObject.FindProperty("m_active").boolValue;
            changed = DrawDefaultInspector();
            changed |= DoDistributorProperty("m_sizeDistributor", "Size");
            changed |= DoDistributorProperty("m_rotationDistributor", "Rotation");
            changed |= DoDistributorProperty("m_alphaDistributor", "Alpha");
            changed |= DoDistributorProperty("m_colorDistributor", "Color");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_colorOverlay"));
            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_color"));
            changed |= serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;

            return changed;
        }

        bool DoDistributorProperty(string Property, string Title)
        {
            SerializedProperty p = serializedObject.FindProperty(Property);
            IEnumerator e = p.GetEnumerator();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Title, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(p.FindPropertyRelative("m_type"));
            EditorGUILayout.PropertyField(p.FindPropertyRelative("m_distributionCurve"));
            EditorGUILayout.PropertyField(p.FindPropertyRelative("m_variation"));
            EditorGUILayout.PropertyField(p.FindPropertyRelative("m_multiplayer"));
            if (p.FindPropertyRelative("m_type").enumValueIndex == (int)DistrbuitionType.Perlin)
            {
                EditorGUILayout.PropertyField(p.FindPropertyRelative("m_frequncy"));
                EditorGUILayout.PropertyField(p.FindPropertyRelative("m_amplitude"));
            }
            EditorGUI.indentLevel--;
            return serializedObject.ApplyModifiedProperties();
        }

    }
}
