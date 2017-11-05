using UnityEngine;
using System;
using UnityEditor;

namespace Gridlike {

	public class PlaceTool : GridTool {

		int radius = 1;
		int id = 1;
		int subId = 0;

		public PlaceTool() {
			radius = Mathf.Max(1, PlayerPrefs.GetInt ("grid.place.radius"));
			id = Mathf.Max(1, PlayerPrefs.GetInt ("grid.place.id"));
			subId = Mathf.Max(0, PlayerPrefs.GetInt ("grid.place.subId"));
		}

		public override void Serialize() {
			PlayerPrefs.SetInt ("grid.place.radius", radius);
			PlayerPrefs.SetInt ("grid.place.id", id);
			PlayerPrefs.SetInt ("grid.place.subId", subId);
		}

		public override bool UseWindow() {
			return true;
		}
		public override string Name() {
			return "place";
		}

		public override bool Window() {

			id = EditorGUILayout.IntField ("id", id);
			if (grid.atlas == null || id <= 0 || id >= grid.atlas.atlas.Length) id = 0;
			if (grid.atlas == null || grid.atlas [id] == null) id = 0;

			subId = EditorGUILayout.IntField ("Sub id", subId);
			if (subId < 0) subId = 0;

			if (grid.atlas [id].tileGO == null) {
				radius = Mathf.Max(1, EditorGUILayout.IntField ("radius", radius));
			} else {
				radius = 1;
			}
			return false;
		}

		public override bool Update () {
			Vector2 position = grid.transform.TransformPoint (new Vector2 (mouseX, mouseY));
			DrawSquare (position.x - radius + 1, position.y - radius + 1, position.x + radius, position.y + radius, Color.magenta);
			return false;
		}

		public override bool OnMouseDown() {
			Place ();
			return true;
		}
		public override bool OnMouseUp() {
			Place ();
			return true;
		}
		public override bool OnMouse() {
			Place ();
			return true;
		}

		void Place() {
			int x = mouseX, y = mouseY;

			bool hasPlaced = false;

			int r = radius - 1;

			for (int i = -r; i <= r; i++) {
				for (int j = -r; j <= r; j++) {
					if (grid.CanSet (x, y, id)) {
						hasPlaced = true;
						grid.Set (x + i, y + j, id, subId, 0, 0, 0);
					}
				}
			}

			if(hasPlaced) grid.PresentContainingRegion (mouseX, mouseY);
		}
	}
}