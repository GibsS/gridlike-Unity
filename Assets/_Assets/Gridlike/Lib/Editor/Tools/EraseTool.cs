using UnityEngine;
using UnityEditor;
using System;

namespace Gridlike {
	
	public class EraseTool : GridTool {

		int radius;

		public EraseTool() {
			radius = Mathf.Max(1, PlayerPrefs.GetInt ("grid.erase.radius"));
		}

		public override void Serialize() {
			PlayerPrefs.SetInt ("grid.erase.radius", radius);
		}

		public override bool UseWindow() {
			return true;
		}
		public override string Name() {
			return "erase";
		}

		public override bool Window() {
			radius = Mathf.Max(1, EditorGUILayout.IntField ("Radius", radius));
			return false;
		}

		public override bool Update () {
			Vector2 position = grid.transform.TransformPoint (new Vector2 (mouseX, mouseY));
			DrawSquare (position.x - radius + 1, position.y - radius + 1, position.x + radius, position.y + radius, Color.magenta);
			return false;
		}

		public override bool OnMouseDown() {
			Erase ();
			return true;
		}
		public override bool OnMouseUp() {
			Erase ();
			return true;
		}
		public override bool OnMouse() {
			Erase ();
			return true;
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
}