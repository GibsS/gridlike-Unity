using UnityEngine;
using UnityEditor;
using System.Collections;

public class EraseTool : GridTool {

	int radius;

	public override bool UseWindow() {
		return true;
	}
	public override string Name() {
		return "erase";
	}

	public override void Window() {
		GUILayout.BeginArea(new Rect(20, 60, 300, 60));

		var rect = EditorGUILayout.BeginVertical ();

		GUI.color = Color.white;
		GUI.Box(rect, GUIContent.none);

		radius = Mathf.Max(1, EditorGUILayout.IntField ("radius", radius));

		EditorGUILayout.EndVertical ();

		GUILayout.EndArea();
	}

	public override void OnMouse() {
		int x = mouseX, y = mouseY;
		int r = radius - 1;

		for (int i = -r; i <= r; i++) {
			for (int j = -r; j <= r; j++) {
				grid.Clear (x + i, y + j);
			}
		}
	}
}