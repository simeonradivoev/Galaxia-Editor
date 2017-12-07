//#define Debug

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Galaxia;
using System.Reflection;

namespace GalaxyGeneratorEditor
{
    [CustomEditor(typeof(GalaxyPrefab))]
    [CanEditMultipleObjects]
    public sealed class GalaxyPrefabEditor : Editor
    {
	    private const string DISTRIBUTOR = "m_distributor";
	    private const string ACTIVE = "m_active";
	    private const string PARTICLE_PREFABS = "m_particlePrefabs";

	    private GUIStyle lightBgStyle;
	    private GUIStyle darkBackgroundStyle;
	    private GUIStyle moduleBgStyle;
	    private GUIStyle titleStyle;
	    private GUIStyle buttonLeftStyle;
	    private GUIStyle buttonRightStyle;
	    private GUIStyle buttonMidStyle;

		List<ParticlesPrefabEditor> Editors;
        Editor ParticlesDistributionEditor;

        void OnEnable()
        {
            RefreshParticlesPrefabEditors();
        }

        internal void RefreshParticlesPrefabEditors()
        {
            GalaxyPrefab prefab = target as GalaxyPrefab;
            Editors = new List<ParticlesPrefabEditor>();
            foreach (ParticlesPrefab p in prefab)
            {
                Editors.Add(CreateEditor(p) as ParticlesPrefabEditor);
            }

            //if (!EditorApplication.isPlayingOrWillChangePlaymode)
            //{
            //    RecreateAllGalaxies();
            //}
        }

        internal void RefreshParticlesDistributionEditor()
        {
            serializedObject.Update();
            if (serializedObject.FindProperty(DISTRIBUTOR).objectReferenceValue != null)
            {
                ParticlesDistributionEditor = CreateEditor(serializedObject.FindProperty(DISTRIBUTOR).objectReferenceValue);
            }
            else
            {
                ParticlesDistributionEditor = null;
            }
            RecreateAllGalaxies();
        }

	    internal void UpdateDistributorEditor()
	    {
		    if (ParticlesDistributionEditor == null)
		    {
			    RefreshParticlesDistributionEditor();
				return;
		    }

		    if (ParticlesDistributionEditor != null && ParticlesDistributionEditor.target != serializedObject.FindProperty(DISTRIBUTOR).objectReferenceValue)
		    {
				RefreshParticlesDistributionEditor();
			}
	    }

	    public override void OnInspectorGUI()
        {
            OnGUI();
        }

	    private void CheckStyles()
	    {
		    if (darkBackgroundStyle == null)
		    {
			    lightBgStyle = "PreferencesSectionBox";
				darkBackgroundStyle = "CurveEditorBackground";
			    moduleBgStyle = "ShurikenModuleBg";
			    titleStyle = "ShurikenEmitterTitle";
			    buttonLeftStyle = "ButtonLeft";
			    buttonMidStyle = "ButtonMid";
			    buttonRightStyle = "ButtonRight";
		    }
	    }

        internal bool OnGUI()
        {
	        CheckStyles();

			serializedObject.UpdateIfRequiredOrScript();

			SerializedProperty particlePrefabs = serializedObject.FindProperty(PARTICLE_PREFABS);
			SerializedProperty distributor = serializedObject.FindProperty(DISTRIBUTOR);

			serializedObject.targetObject.name = EditorGUILayout.TextField("Name",serializedObject.targetObject.name);
            if (DrawDefaultInspector())
            {
                UpdateAllGalaxies();
            }

            EditorGUILayout.BeginVertical(lightBgStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Type", distributor.tooltip));
            if (GUILayout.Button(distributor.objectReferenceValue != null ? distributor.objectReferenceValue.name : "Distribution", EditorStyles.popup))
            {
                NewDistributorMenu();
            }
            EditorGUILayout.EndHorizontal();
            if (distributor.objectReferenceValue != null)
            {

				/*SerializedObject Distributor = new SerializedObject(distributor.objectReferenceValue);
                SerializedProperty prop = Distributor.GetIterator();
                prop.NextVisible(true);

                while (prop.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(prop, true);
                }*/

				EditorGUI.BeginChangeCheck();
	            UpdateDistributorEditor();
                ParticlesDistributionEditor.OnInspectorGUI();
	            if (EditorGUI.EndChangeCheck())
	            {
					UpdateAllGalaxies();
				}

	            /*if (Distributor.ApplyModifiedProperties())
                {
                    UpdateAllGalaxies();
                }*/
            }
            #if Debug
            if (GUILayout.Button("Change Flags"))
            {
                foreach(ParticlesPrefab p in target as GalaxyPrefab)
                {
                    p.hideFlags = HideFlags.HideInHierarchy;

                }

                (target as GalaxyPrefab).Distributor.hideFlags = HideFlags.HideInHierarchy;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            #endif
            if(GUILayout.Button("Recreate curves"))
            {
                (distributor.objectReferenceValue as ParticleDistributor).RecreateCurves();
            }
            if (serializedObject.ApplyModifiedProperties())
            {
                UpdateAllGalaxies();

            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(darkBackgroundStyle);
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", buttonLeftStyle))
            {
                //PopupWindow.Show(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), popup);
                PopupWindow.Show(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), new PopupWindows.CreateNewParticlesWindow(this));
            }
            if (GUILayout.Button("Update", buttonMidStyle))
            {
                foreach(Galaxy g in GameObject.FindObjectsOfType<Galaxy>())
                {
                    if (g.GalaxyPrefab == target as GalaxyPrefab)
                    {
                        g.UpdateParticles();
                    }
                }
            }
            if (GUILayout.Button("ReCreate", buttonMidStyle))
            {
                RecreateAllGalaxies();
            }
            if (GUILayout.Button("Clear", buttonRightStyle))
            {
	            if (EditorUtility.DisplayDialog("Clear All Particle Prefabs", "Are you sure you want to delete all Particle Prefabs from the galaxy?", "Yes", "Cancel"))
	            {
		            foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target)))
		            {
			            if (asset is ParticlesPrefab)
			            {
				            DestoryParticlesPrefab(asset as ParticlesPrefab);
			            }
		            }
	            }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

			for (int i = 0; i < particlePrefabs.arraySize; i++)
            {
                if (particlePrefabs.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
					particlePrefabs.DeleteArrayElementAtIndex(i);
                    break;
                }
                else if (Editors.Count > i && Editors[i].target != null)
                {
                    Editors[i].serializedObject.Update();
                    //serializedObject.FindProperty("ParticlePrefabs").GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.InspectorTitlebar(serializedObject.FindProperty("ParticlePrefabs").GetArrayElementAtIndex(i).isExpanded, Editors[i].target);
                    GUIStyle s = new GUIStyle(titleStyle);
                    s.padding.left += 40;
                    s.padding.right = 0;
                    EditorGUILayout.BeginHorizontal(s, GUILayout.MinHeight(28));
					particlePrefabs.GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.Foldout(particlePrefabs.GetArrayElementAtIndex(i).isExpanded, particlePrefabs.GetArrayElementAtIndex(i).objectReferenceValue.name);
                    s = new GUIStyle((GUIStyle)"Button");
                    s.normal.background = null;
                    s.active.background = null;
                    s.margin.left = s.margin.right = 0;
                    s.padding = new RectOffset(0, 0, 0, 0);
                    s.fixedWidth = 24;
                    GUILayout.Button(EditorGUIUtility.IconContent("_Help"), s);
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup"), s))
                        ParticlesPrefabContextMenu(Editors[i].target as ParticlesPrefab).ShowAsContext();
                    s = new GUIStyle((GUIStyle)"OL Toggle");
                    s.focused.background = null;
                    Editors[i].serializedObject.FindProperty(ACTIVE).boolValue = EditorGUILayout.Toggle(Editors[i].serializedObject.FindProperty(ACTIVE).boolValue, s, GUILayout.Width(28));
                    if(Editors[i].serializedObject.ApplyModifiedProperties())
                    {
                        UpdateAllGalaxies(Editors[i].target as ParticlesPrefab);
                    }
                    EditorGUILayout.EndHorizontal();
                    if (particlePrefabs.GetArrayElementAtIndex(i).isExpanded)
                    {
                        GUI.enabled = Editors[i].serializedObject.FindProperty(ACTIVE).boolValue;
                        EditorGUILayout.BeginVertical(moduleBgStyle);
                        EditorGUILayout.Space();
                        if (Editors[i].OnGUI() || Event.current.commandName == "UndoRedoPerformed" && Event.current.type == EventType.ExecuteCommand)
                        {
                            UpdateAllGalaxies(Editors[i].target as ParticlesPrefab);
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                        GUI.enabled = true;
                    }

                    if(Editors[i].serializedObject.ApplyModifiedProperties())
                    {
                        UpdateAllGalaxies(Editors[i].target as ParticlesPrefab);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            if(serializedObject.ApplyModifiedProperties())
            {
                UpdateAllGalaxies();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            return false;
        }

        void NewDistributorMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach(Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(System.Type type in ass.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ParticleDistributor)))
                    {
                        menu.AddItem(new GUIContent(type.Name), false, ChangeDistribution, type);
                    }
                }
            }

            menu.ShowAsContext();
        }

        void ChangeDistribution(object typeObj)
        {
            System.Type type = typeObj as System.Type;
            GalaxyPrefab galaxy = target as GalaxyPrefab;

            if (galaxy.Distributor != null)
            {
                DestroyImmediate(galaxy.Distributor,true);
            }

            galaxy.Distributor = ScriptableObject.CreateInstance(type) as ParticleDistributor;
            galaxy.Distributor.name = type.Name;
            AssetDatabase.AddObjectToAsset(galaxy.Distributor,galaxy);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshParticlesDistributionEditor();
        }

        void UpdateAllGalaxies()
        {
            GalaxyPrefab galaxyPrefab = target as GalaxyPrefab;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                galaxyPrefab.UpdateAllGalaxies();
            }
            else
            {
                foreach (Galaxy galaxy in GameObject.FindObjectsOfType<Galaxy>())
                {
                    if (galaxy.GalaxyPrefab == target as GalaxyPrefab && galaxy.GenerationType == Galaxy.GalaxyGenerationType.Automatic)
                    {
						galaxy.UpdateParticlesImmediately();
					}
                }
            }
        }

        void UpdateAllGalaxies(ParticlesPrefab prefab)
        {
            GalaxyPrefab galaxyPrefab = target as GalaxyPrefab;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                galaxyPrefab.UpdateAllGalaxies(prefab);
            }
            else
            {
                foreach (Galaxy galaxy in GameObject.FindObjectsOfType<Galaxy>())
                {
                    if (galaxy.GalaxyPrefab == target as GalaxyPrefab && galaxy.GenerationType == Galaxy.GalaxyGenerationType.Automatic)
                    {
						galaxy.UpdateParticlesImmediately();
					}
                }
            }
        }

        void RecreateAllGalaxies()
        {
            GalaxyPrefab galaxyPrefab = target as GalaxyPrefab;

            foreach (Galaxy galaxy in GameObject.FindObjectsOfType<Galaxy>())
            {
                if (galaxy.GalaxyPrefab == target as GalaxyPrefab)
                {
					galaxy.UpdateParticlesImmediately();
				}
            }
        }

        [MenuItem("Assets/Create/Galaxy Prefab")]
        public static void CreateGalaxyPrefab()
        {
            GalaxyPrefab prefab = ScriptableObjectUtility.CreateAsset<GalaxyPrefab>();
            prefab.Distributor = ScriptableObject.CreateInstance<DensityWaveDistributor>();
            prefab.Distributor.name = "Density Wave Distribution";
            AssetDatabase.AddObjectToAsset(prefab.Distributor,prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal void CreateParticlePrefab(string name,ParticlesPrefab.Preset Preset)
        {
            GalaxyPrefab galaxyPrefab = target as GalaxyPrefab;
            ParticlesPrefab prefab = galaxyPrefab.Create(name, Preset);
            UnityEditor.AssetDatabase.AddObjectToAsset(prefab, galaxyPrefab);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            RefreshParticlesPrefabEditors();
        }

        GenericMenu ParticlesPrefabContextMenu(ParticlesPrefab prefab)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, DestoryParticlesPrefab, prefab);
            menu.AddItem(new GUIContent("Duplicate"), false, Dublicate, prefab);
            menu.AddItem(new GUIContent("Reset"), false, Reset, prefab);
            return menu;
        }

        void DestoryParticlesPrefab(object prefabObj)
        {
            GalaxyPrefab galaxy = target as GalaxyPrefab;
            ParticlesPrefab prefab = prefabObj as ParticlesPrefab;
            galaxy.Remove(prefab);
            prefab.DestoryPrefab();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            RefreshParticlesPrefabEditors();
        }

        void Dublicate(object prefab)
        {
            GalaxyPrefab galaxy = target as GalaxyPrefab;
            ParticlesPrefab newPrefab = Instantiate(prefab as ParticlesPrefab) as ParticlesPrefab;
            newPrefab.hideFlags = HideFlags.HideInHierarchy;
            galaxy.Insert(prefab as ParticlesPrefab, newPrefab);
            AssetDatabase.AddObjectToAsset(newPrefab, galaxy);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            RefreshParticlesPrefabEditors();
        }

        void Reset(object prefabObj)
        {
            ParticlesPrefab prefab = prefabObj as ParticlesPrefab;
            GalaxyPrefab galaxy = target as GalaxyPrefab;
            galaxy.PopulatePreset(prefab, prefab.OriginalPreset);
        }
    }
}
