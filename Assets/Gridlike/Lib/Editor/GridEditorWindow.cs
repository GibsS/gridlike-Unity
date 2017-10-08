using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridEditorWindow : EditorWindow {

	public int id;
	public int subid;

	public int state1;
	public int state2;
	public int state3;

	[MenuItem ("Gridlike/Grid editor")]
	public static GridEditorWindow ShowWindow () {
		return EditorWindow.GetWindow(typeof(GridEditorWindow), false, "Grid editor") as GridEditorWindow;
	}

	void OnGUI () {
		id = EditorGUILayout.IntField ("Tile id", id);
		subid = EditorGUILayout.IntField ("Tile sub-id", subid);

		state1 = EditorGUILayout.IntField ("Tile state var 1", state1);
		state2 = EditorGUILayout.IntField ("Tile state var 2", state2);
		state3 = EditorGUILayout.IntField ("Tile state var 3", state3);
	}
}