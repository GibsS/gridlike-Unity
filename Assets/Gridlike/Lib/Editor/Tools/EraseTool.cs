using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class EraseTool : GridTool {

	[SerializeField] int radius;

	public override bool UseWindow() {
		return true;
	}
	public override string Name() {
		return "erase";
	}

	public override void Window() {
		radius = Mathf.Max(1, EditorGUILayout.IntField ("radius", radius));
	}

	public override void OnMouseDown() {
		Erase ();
	}
	public override void OnMouseUp() {
		Erase ();
	}
	public override void OnMouse() {
		Erase ();
	}

	void Erase() {
		int x = mouseX, y = mouseY;
		int r = radius - 1;

		for (int i = -r; i <= r; i++) {
			for (int j = -r; j <= r; j++) {
				grid.Clear (x + i, y + j);
			}
		}
	}
}