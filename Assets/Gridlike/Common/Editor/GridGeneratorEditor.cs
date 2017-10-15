using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor {
	
	public override void OnInspectorGUI() {
		GridGenerator generator = target as GridGenerator;

		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GridGenerator)target), typeof(GridGenerator), false);
		GUI.enabled = true;

		generator.useSave = EditorGUILayout.ToggleLeft ("Use saving", generator.useSave);

		if (generator.useSave) {
			generator.usePersistentPath = EditorGUILayout.ToggleLeft ("Persistent data path as root", generator.usePersistentPath);
			generator.path = EditorGUILayout.TextField ("Path", generator.path);
			EditorGUILayout.LabelField ("Saved at: " + generator.rootPath);

			GridSaveManifest man = generator.gridSaveManifest;
			// Bug: always shows 0 when in editor mode
			EditorGUILayout.LabelField ("Number of Region saved: " + (man != null ? man.regionPositions.Count : 0).ToString());

			if (GUILayout.Button ("Clear save")) {
				generator.ClearSave ();
			}
		}
	}
}