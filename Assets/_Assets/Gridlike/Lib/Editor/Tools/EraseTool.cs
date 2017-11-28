using UnityEngine;
using UnityEditor;
using System;

namespace Gridlike {
	
	public class EraseTool : GridTool {

		int radius;

		bool brushIsCircular;

		bool replace;
		int replacedId;

		public EraseTool() {
			radius = Mathf.Max(1, PlayerPrefs.GetInt ("grid.erase.radius"));

			brushIsCircular = PlayerPrefs.GetInt ("grid.erase.brushIsCircular") == 1;

			replace = PlayerPrefs.GetInt ("grid.erase.replace") == 1;
			replacedId = PlayerPrefs.GetInt ("grid.erase.replaceId");
		}

		public override void Serialize() {
			PlayerPrefs.SetInt ("grid.erase.radius", radius);

			PlayerPrefs.SetInt ("grid.erase.brushIsCircular", brushIsCircular ? 1 : 0);

			PlayerPrefs.GetInt ("grid.erase.replace", replace ? 1 : 0);
			PlayerPrefs.GetInt ("grid.erase.replaceId", replacedId);
		}

		public override bool UseWindow() {
			return true;
		}
		public override string Name() {
			return "erase";
		}

		public override bool Window() {
			EditorGUILayout.BeginHorizontal ();

			radius = Mathf.Max(1, EditorGUILayout.IntField ("Radius", radius));

			if (GUILayout.Button ("-", GUILayout.Width(40))) {
				radius -= 1;
				if (radius == 0) radius = 1;
			}
			if (GUILayout.Button ("+", GUILayout.Width(40))) {
				radius += 1;
			}

			EditorGUILayout.EndHorizontal ();

			brushIsCircular = !EditorGUILayout.Toggle ("Square", !brushIsCircular);
			brushIsCircular = EditorGUILayout.Toggle ("Circle", brushIsCircular);

			replace = EditorGUILayout.Toggle ("Replace", replace);

			if (replace) {
				replacedId = Mathf.Max (1, EditorGUILayout.IntField ("Replaced id", replacedId));
			}

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

			if (brushIsCircular || replace) {
				for (int i = 0; i < 2 * r + 1; i++) {
					for (int j = 0; j < 2 * r + 1; j++) {

						if (brushIsCircular) {
							float dx = i - r;
							float dy = j - r;

							if (dx * dx + dy * dy - 1 > r * r) {
								continue;
							}
						}

						if (replace && grid.GetId (x + i - r, y + j - r) != replacedId) {
							continue;
						}

						grid.Clear (x + i - r, y + j - r);
					}
				}
			} else {
				grid.Clear (x - r, y - r, 2 * r + 1, 2 * r + 1);
			}
		}
	}
}