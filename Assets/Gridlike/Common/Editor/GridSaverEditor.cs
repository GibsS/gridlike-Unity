using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSaver))]
public class GridSaverEditor : Editor {

	public override void OnInspectorGUI() {
		GridSaver saver = target as GridSaver;

		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GridSaver)target), typeof(GridSaver), false);
		GUI.enabled = true;

		saver.usePersistentPath = EditorGUILayout.ToggleLeft ("Persistent data path as root", saver.usePersistentPath);
		saver.path = EditorGUILayout.TextField ("Path", saver.path);
		EditorGUILayout.LabelField ("Save at: " + saver.rootPath);

		GridSaveManifest man = saver.gridSaveManifest;
		// Bug: always shows 0 when in editor mode
		EditorGUILayout.LabelField ("Number of Region saved: " + (man != null ? man.regionPositions.Count : 0).ToString ());

		if (GUILayout.Button ("Clear save")) {
			saver.ClearSave ();
		}
	}
}