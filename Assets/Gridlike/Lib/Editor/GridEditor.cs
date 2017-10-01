using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

	public override void OnInspectorGUI() {
		Grid grid = target as Grid;

		int tileSize = EditorGUILayout.IntField ("Tile size", grid.tileSize);
		grid.tileSize = tileSize < 1 ? 1 : tileSize;

		if (GUILayout.Button ("Open grid editor")) {
			GridEditorWindow.ShowWindow ();
		}
	}

	void OnSceneGUI() {
		Grid grid = target as Grid;

		foreach (FiniteGrid region in grid.tiles.GetRegions()) {
			float bx = grid.transform.position.x + region.x * Grid.REGION_SIZE * grid.tileSize;
			float by = grid.transform.position.y + region.y * Grid.REGION_SIZE * grid.tileSize;

			for (int i = 0; i < Grid.REGION_SIZE; i++) {
				for (int j = 0; j < Grid.REGION_SIZE; j++) {
					Tile tile = region.Get (i, j);
					if (tile != null && tile.shape == TileShape.FULL) {
						//Debug.Log ("tile shape=" + tile.shape.ToString ());
						Handles.DrawLine (
							new Vector2 (bx + i * grid.tileSize, by + j * grid.tileSize),
							new Vector2 (bx + (i + 1) * grid.tileSize, by + (j + 1) * grid.tileSize)
						);
					}
				}
			}
		}

		if (Event.current.button == 0) {
			switch (Event.current.type) {
				case EventType.mouseDown: {
					Event.current.Use ();
					break;
				}
				case EventType.mouseUp: {
					int x, y;

					grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out x, out y);
					GridEditorWindow window = GridEditorWindow.ShowWindow ();
					grid.Set (x, y, window.shape, window.id, window.subid, window.state1, window.state2, window.state3);

					Debug.Log ("Mouse up at x=" + x + " y=" + y);
					Event.current.Use ();
					break;
				}
			}
		}

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
	}
}
