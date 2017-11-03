using UnityEngine;
using UnityEditor;

namespace Gridlike {

	[CustomEditor(typeof(GridGenerator))]
	public class GridGeneratorEditor : Editor {
		
		public override void OnInspectorGUI() {
			GridGenerator generator = target as GridGenerator;

			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GridGenerator)target), typeof(GridGenerator), false);
			GUI.enabled = true;

			if (generator.algorithm == null) {
				EditorGUILayout.HelpBox ("The grid generator requires a subclass of GridGeneratorAlgorithm", MessageType.Warning);
			}

			GUI.enabled = !Application.isPlaying;

			generator.algorithm = EditorGUILayout.ObjectField ("Algorithm", (GridGeneratorAlgorithm)generator.algorithm, typeof(GridGeneratorAlgorithm), false) as GridGeneratorAlgorithm;

			generator.generationRegionWidth = Mathf.Max (1, EditorGUILayout.IntField ("Generation region width", generator.generationRegionWidth));
			generator.generationRegionHeight = Mathf.Max (1, EditorGUILayout.IntField ("Generation region height", generator.generationRegionHeight));

			generator.useSave = EditorGUILayout.ToggleLeft ("Use saving", generator.useSave);

			if (generator.useSave) {
				generator.usePersistentPath = EditorGUILayout.ToggleLeft ("Persistent data path as root", generator.usePersistentPath);
				generator.path = EditorGUILayout.TextField ("Path", generator.path);
				EditorGUILayout.LabelField ("Saved at: " + generator.rootPath);

				GridSaveManifest man = generator.gridSaveManifest;
				// Bug: always shows 0 when in editor mode
				EditorGUILayout.LabelField ("Number of Region saved: " + man.regionPositions.Count);

				GUI.enabled = true;

				if (GUILayout.Button ("Clear save")) {
					generator.ClearSave ();
				}
			}
			GUI.enabled = true;
		}
	}
}