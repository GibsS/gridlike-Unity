using UnityEngine;
using UnityEditor;
using System;

namespace Gridlike {
	
	[Serializable]
	public class EraseTool : GridTool {

		[SerializeField] int radius;

		public override bool UseWindow() {
			return true;
		}
		public override string Name() {
			return "erase";
		}

		public override bool Window() {
			radius = Mathf.Max(1, EditorGUILayout.IntField ("radius", radius));
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