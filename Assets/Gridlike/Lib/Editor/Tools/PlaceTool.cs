using UnityEngine;
using System.Collections;
using UnityEditor;

// TODO Add area setting
public class PlaceTool : GridTool {

	int id = 1;

	public override void Window() {
		GUILayout.BeginArea(new Rect(20, 60, 300, 60));

		var rect = EditorGUILayout.BeginVertical ();

		GUI.color = Color.white;
		GUI.Box(rect, GUIContent.none);

		id = EditorGUILayout.IntField ("id", id);

		if (grid.atlas == null || id <= 0 || id >= grid.atlas.atlas.Length) id = 0;

		if (grid.atlas == null || grid.atlas [id] == null) id = 0;

		EditorGUILayout.EndVertical ();

		GUILayout.EndArea();
	}

	public override bool UseWindow() {
		return true;
	}
	public override string Name() {
		return "place";
	}

	public override void OnMouse() {
		int x = mouseX, y = mouseY;

		if(grid.CanSet(x, y, id)) grid.Set (x, y, id, -1, 0, 0, 0);

		grid.PresentContainingRegion (mouseX, mouseY);
	}
}