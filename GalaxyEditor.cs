using UnityEngine;
using UnityEditor;
using Galaxia;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace GalaxyGeneratorEditor
{
    [CustomEditor(typeof(Galaxy))]
    [CanEditMultipleObjects]
    public sealed class GalaxyEditor : Editor
    {
	    private const string FRUSTUM_CULLING = "m_frustumCulling";
	    private const string GENERATION_TYPE = "m_generationType";

		void OnEnable()
        {
            SceneView.onSceneGUIDelegate += Draw;
            if(SceneView.currentDrawingSceneView != null)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                SceneView.currentDrawingSceneView.camera.Render();
                DestroyImmediate(g, true);
            }

			Galaxy galaxy = target as Galaxy;
			if (!EditorApplication.isPlayingOrWillChangePlaymode && galaxy.GenerationType == Galaxy.GalaxyGenerationType.Automatic)
            {
                galaxy.SmartParticleInitialization();
            }
        }

        public override void OnInspectorGUI()
        {
            Galaxy galaxy = target as Galaxy;
	        if (galaxy == null) return;
	        if (galaxy.LastPreRenderEvent.Used)
	        {
		        EditorGUILayout.HelpBox("Galaxy Rendering controlled elsewhere",MessageType.Info);
	        }
	        GUI.enabled = !galaxy.LastPreRenderEvent.Used;
            galaxy.RenderGalaxy = EditorGUILayout.Toggle(new GUIContent("Render Galaxy"), galaxy.RenderGalaxy);
	        GUI.enabled = true;
            galaxy.GPU = EditorGUILayout.Toggle(new GUIContent("GPU"), galaxy.GPU);
			galaxy.SaveMeshes = EditorGUILayout.Toggle(new GUIContent("Save Meshes","Should generated meshes by GPU particles be saved in scene or prefab?"), galaxy.SaveMeshes);
			if (!Application.isPlaying && GUI.changed) MarkAllResourcesDirty();
			galaxy.SaveParticles = EditorGUILayout.Toggle(new GUIContent("Save Particles", "Should generated particles by distributor be saved in scene or prefab? This represents the particle data."), galaxy.SaveParticles);
			if (!Application.isPlaying && GUI.changed) MarkAllResourcesDirty();

			serializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.PropertyField(serializedObject.FindProperty(FRUSTUM_CULLING), new GUIContent("Frustum Culling", "Enable Frustum Culling"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(GENERATION_TYPE), new GUIContent("Generation Type", "Should changes to galaxy prefab parameters be automatically applied to the particles"));
			serializedObject.ApplyModifiedProperties();

			if (galaxy.GenerationType == Galaxy.GalaxyGenerationType.Manual)
			{
				GUILayout.Space(16);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Generate", (GUIStyle)"ButtonLeft"))
				{
					galaxy.GenerateParticles();
				}
				if (GUILayout.Button("Refresh", (GUIStyle)"ButtonMid"))
				{
					galaxy.UpdateParticles();
				}
				if (GUILayout.Button("Destory", (GUIStyle)"ButtonRight"))
				{
					galaxy.DestroyParticles();
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}

			bool changed = DrawDefaultInspector();

            if(galaxy.GenerationType == Galaxy.GalaxyGenerationType.Automatic)
            {
                if(changed)
                {
                    galaxy.GenerateParticles();
                }
            }

			if (GUI.changed)
			{
				EditorUtility.SetDirty(galaxy);
			}
        }

        internal void Draw(SceneView view)
        {
            if(Event.current.type == EventType.repaint)
            {
                Draw(target as Galaxy);
            }
        }

        internal static void Draw(Galaxy galaxy)
        {
            if (galaxy.GalaxyPrefab != null)
            {
                Handles.color = new Color(0.6f, 0.8f, 1);
                Handles.DrawLine(galaxy.transform.TransformPoint(new Vector3(galaxy.GalaxyPrefab.Size, 0, 0) + new Vector3(0, 0, galaxy.GalaxyPrefab.Size + galaxy.GalaxyPrefab.Size * 0.1f)), galaxy.transform.TransformPoint(new Vector3(-galaxy.GalaxyPrefab.Size, 0, 0) + new Vector3(0, 0, galaxy.GalaxyPrefab.Size + galaxy.GalaxyPrefab.Size * 0.1f)));
                Handles.Label(galaxy.transform.TransformPoint(new Vector3(0, 0, galaxy.GalaxyPrefab.Size + galaxy.GalaxyPrefab.Size * 0.1f)), galaxy.GalaxyPrefab.Size.ToString(), EditorStyles.boldLabel);
            }
        }

	    private void MarkAllResourcesDirty()
	    {
			Galaxy galaxy = target as Galaxy;
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			EditorUtility.SetDirty(target);
			if (galaxy == null || galaxy.Particles == null) return;
		    foreach (var particle in galaxy.Particles)
		    {
			    EditorUtility.SetDirty(particle);
			    if (particle.Meshes == null) continue;
			    foreach (var mesh in particle.Meshes)
			    {
				    EditorUtility.SetDirty(mesh);
			    }
		    }
		}

        void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= Draw;
        }
    }
}
