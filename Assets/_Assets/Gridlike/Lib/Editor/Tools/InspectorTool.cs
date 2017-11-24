using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace Gridlike {
	
	public class InspectorTool : GridTool {

		int selectedX;
		int selectedY;

		public InspectorTool() {
			selectedX = PlayerPrefs.GetInt ("grid.inspector.selectedX");
			selectedY = PlayerPrefs.GetInt ("grid.inspector.selectedY");
		}

		public override void Serialize() {
			PlayerPrefs.SetInt ("grid.inspector.selectedX", selectedX);
			PlayerPrefs.SetInt ("grid.inspector.selectedY", selectedY);
		}

		public override bool UseWindow () {
			return true;
		}
		public override string Name() {
			return "inspect";
		}

		public override bool Window () {
			Tile tile = grid.Get (selectedX, selectedY);

			if (tile != null) {
				EditorGUILayout.LabelField ("Id:" + tile.id.ToString());
				EditorGUILayout.LabelField ("Sub id:" + tile.subId.ToString());

				EditorGUILayout.LabelField ("Is tile GO center:" + tile.tileGOCenter.ToString());
				EditorGUILayout.LabelField ("Has tile component:" + (grid.GetTileComponent (selectedX, selectedY) != null));

				tile.state1 = EditorGUILayout.FloatField ("State1", tile.state1);
				tile.state2 = EditorGUILayout.FloatField ("State2", tile.state2);
				tile.state3 = EditorGUILayout.FloatField ("State3", tile.state3);

				tile.name = EditorGUILayout.TextField ("Name", tile.name);
                
				EditorGUILayout.LabelField ("Tile dictionary");

				if (tile.dictionary != null) {

					for (int i = 0; i < tile.dictionary.Count; i++) {
						EditorGUILayout.Space ();

						EditorGUILayout.BeginHorizontal ();

						tile.dictionary.SetKey (i, EditorGUILayout.TextField ("key " + i, tile.dictionary.GetKey (i))); 

						if (GUILayout.Button ("X", GUILayout.MaxWidth(20))) {
							tile.dictionary.Remove (tile.dictionary.GetKey (i));

							if (tile.dictionary.Count == 0) {
								tile.dictionary = null;
							}
							break;
						}

						EditorGUILayout.EndHorizontal ();
						tile.dictionary.SetValue (i, EditorGUILayout.TextField (tile.dictionary.GetValue (i)));
					}
				}

				if (GUILayout.Button ("Add dictionary key-value", GUILayout.MinHeight(30))) {
					if (tile.dictionary == null) {
						tile.dictionary = new TileDictionary ();
					}

					int i = tile.dictionary.Count;
					while (tile.dictionary.ContainsKey ("key" + i)) {
						i++;
					}

					tile.dictionary.Add ("key" + i, "value" + i);
				}
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
			selectedX = mouseX;
			selectedY = mouseY;
			return false;
		}
	}
}