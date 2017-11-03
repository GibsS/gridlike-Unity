using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace Gridlike {
	
	[Serializable]
	public class InspectorTool : GridTool {

		public override bool UseWindow () {
			return true;
		}
		public override string Name() {
			return "inspect";
		}

		int selectedX;
		int selectedY;

		public override bool Window () {
			Tile tile = grid.Get (selectedX, selectedY);

			if (tile != null) {
				EditorGUILayout.LabelField ("id:" + tile.id.ToString());
				EditorGUILayout.LabelField ("sub id:" + tile.subId.ToString());

				EditorGUILayout.LabelField ("is tile GO center:" +tile.tileGOCenter.ToString());

				tile.state1 = EditorGUILayout.FloatField ("state1", tile.state1);
				tile.state2 = EditorGUILayout.FloatField ("state2", tile.state2);
				tile.state3 = EditorGUILayout.FloatField ("state3", tile.state3);
                /*
				if (tile.dictionary != null) {
					EditorGUILayout.LabelField ("key list size=" + tile.dictionary.KeyCount);
					EditorGUILayout.LabelField ("value list size=" + tile.dictionary.ValueCount);

					for (int i = 0; i < tile.dictionary.Count; i++) {
						EditorGUILayout.BeginHorizontal ();

						tile.dictionary.SetKey (i, EditorGUILayout.TextField (tile.dictionary.GetKey (i), GUILayout.MaxWidth(130))); 
						tile.dictionary.SetValue (i, EditorGUILayout.TextField (tile.dictionary.GetValue (i)));

						if (GUILayout.Button ("X", GUILayout.MaxWidth(20))) {
							tile.dictionary.Remove (tile.dictionary.GetKey (i));
							break;
						}

						EditorGUILayout.EndHorizontal ();
					}
				}

				if (GUILayout.Button ("Add dictionary key-value")) {
					if (tile.dictionary == null) {
						tile.dictionary = new TileDictionary ();
					}

					int i = tile.dictionary.Count;

					tile.dictionary.Add ("key" + i, "value" + i);
				}*/
			} else {
				EditorGUILayout.LabelField ("No tile under the cursor");
			}
			return false;
		}

		public override bool Update() {
			int x = selectedX, y = selectedY;
			Tile tile = grid.Get (x, y);

			DrawTileInformation (x, y, Color.magenta, tile == null ? null : new string[] { "id=" + tile.id });
			return false;
		}
		public override bool OnMouseDown() {
			selectedX = mouseX;
			selectedY = mouseY;
			return false;
		}

		public override bool OnMouse () {
			int x = mouseX, y = mouseY;
			Tile tile = grid.Get (x, y);

			DrawTileInformation (x, y, Color.magenta, tile == null ? null : new string[] { "id=" + tile.id });
			return false;
		}
	}
}