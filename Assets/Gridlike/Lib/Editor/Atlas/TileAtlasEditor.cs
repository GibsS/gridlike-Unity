using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileAtlas))]
public class GridTileAtlasEditor : Editor {

	bool showPosition;

	public override void OnInspectorGUI() {
		TileAtlas atlas = target as TileAtlas;

		/*GUILayout.Label ("Tile count :" + atlas.Count);

		SerializedObject o = new SerializedObject (atlas);
		SerializedProperty array = o.FindProperty ("atlas");

		for(int i = 0; i < atlas.atlas.Length; i++) {
			if (atlas.atlas[i].id != 0) {
				EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i));
			}
		}

		if (GUILayout.Button ("Create new tile")) {
			Debug.Log("[Inspector] create tile" + atlas.AddTile ());
		}*/

		DrawDefaultInspector ();
	}
}