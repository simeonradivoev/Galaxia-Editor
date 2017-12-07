// ----------------------------------------------------------------
// Galaxia
// ©2016 Simeon Radivoev
// Written by Simeon Radivoev (simeonradivoev@gmail.com)
// ----------------------------------------------------------------

using Galaxia;
using UnityEditor;
using UnityEngine;

namespace GalaxyGeneratorEditor
{
	[CustomEditor(typeof (ImageDistributor))]
	public class ImageDistributorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			OnGUI();
		}

		internal void OnGUI()
		{
			ImageDistributor distributor = (ImageDistributor)target;
			serializedObject.UpdateIfRequiredOrScript();

			distributor.SetDistributionMap(DrawTexField(new GUIContent("Distribution Map (Grayscale)"), distributor.GetDistributionMap()));
			distributor.SetColorMap(DrawTexField(new GUIContent("Color Map (RGB)"), distributor.GetColorMap()));
			distributor.SetHeightMap(DrawTexField(new GUIContent("Height Map (Grayscale)"), distributor.GetHeightMap()));

			DrawDefaultInspector();

			distributor.DistributonDownsample = Mathf.Clamp(EditorGUILayout.DelayedIntField(new GUIContent("Distribution Downsample", "The resolution downsample of the distribution map."), distributor.DistributonDownsample),1,10);

			serializedObject.ApplyModifiedProperties();
		}

		private Texture2D DrawTexField(GUIContent content, Texture2D tex2D)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
			rect = EditorGUI.PrefixLabel(rect, content);
			tex2D = (Texture2D)EditorGUI.ObjectField(rect, GUIContent.none, tex2D, typeof(Texture2D), false);
			CheckTextureReadability(tex2D);
			return tex2D;
		}

		private void CheckTextureReadability(Texture2D texture)
		{
			TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
			if (texture && textureImporter && !textureImporter.isReadable)
			{
				EditorGUILayout.HelpBox("Texture Must be readable. Enable read/write in the texture import settings!", MessageType.Error, true);
			}
		}
	}
}