using UnityEditor;
using UnityEngine;

namespace GalaxyGeneratorEditor
{
    internal class PopupWindows
    {
        internal class CreateNewParticlesWindow : PopupWindowContent
        {
            public string NewName = "";
            public Galaxia.ParticlesPrefab.Preset Preset; 
            public GalaxyPrefabEditor PrefabEditor;

            public CreateNewParticlesWindow(GalaxyPrefabEditor PrefabEditor)
            {
                this.PrefabEditor = PrefabEditor;
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.BeginArea(rect);
                EditorGUILayout.Space();
                EditorGUILayout.PrefixLabel("Name");
                NewName = GUILayout.TextField(NewName);
                EditorGUILayout.PrefixLabel("Preset");
                Preset = (Galaxia.ParticlesPrefab.Preset)EditorGUILayout.EnumPopup(Preset);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create") && PrefabEditor != null)
                {
                    PrefabEditor.CreateParticlePrefab(NewName, Preset);
                    PrefabEditor.RefreshParticlesPrefabEditors();
                    editorWindow.Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    editorWindow.Close();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }
}
