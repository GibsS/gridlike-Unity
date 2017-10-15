using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

	public override void OnInspectorGUI() {
		Grid grid = target as Grid;

		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Grid)target), typeof(Grid), false);
		GUI.enabled = true;

		float tileSize = EditorGUILayout.FloatField ("Tile size", grid.tileSize);
		grid.tileSize = tileSize < 0 ? 0 : tileSize;

		if (GUILayout.Button ("Open grid editor")) {
			GridEditorWindow.ShowWindow ();
		}
		if (GUILayout.Button ("Rebuild")) {
			grid.PresentAllAgain ();
		}
		if (GUILayout.Button ("Hide all")) {
			grid.HideAll ();
		}

		if (grid.atlas == null) {
			EditorGUILayout.HelpBox ("The grid requires a tile atlas", MessageType.Warning);
		}
		grid.atlas = EditorGUILayout.ObjectField("Tile atlas", grid.atlas, typeof(ScriptableObject), false) as TileAtlas;

		if (GUILayout.Button ("Create new tile atlas")) {
			grid.atlas = ScriptableObjectUtility.CreateAsset<TileAtlas> ();

			Selection.activeObject = grid.atlas;
		}

		grid.useLoading = EditorGUILayout.ToggleLeft ("Use loading", grid.useLoading);

		if (grid.useLoading) {
			grid.useAgentBasedLoading = EditorGUILayout.ToggleLeft ("Use agent based loading", grid.useAgentBasedLoading);
		}

		grid.saveOnClose = EditorGUILayout.ToggleLeft ("Save grid on destroy", grid.saveOnClose);
	}

	void OnSceneGUI() {
		Grid grid = target as Grid;

		foreach (FiniteGrid region in grid.GetRegions()) {
			float size = Grid.REGION_SIZE * grid.tileSize;
			float bx = grid.transform.position.x + region.regionX * size;
			float by = grid.transform.position.y + region.regionY * size;

			Handles.color = region.presented ? Color.green : Color.white;

			Handles.DrawLine (new Vector2(bx + 1, by + 1), new Vector2(bx + size - 1, by + 1));
			Handles.DrawLine (new Vector2(bx + size - 1, by + 1), new Vector2(bx + size - 1, by + size - 1));
			Handles.DrawLine (new Vector2(bx + size - 1, by + size - 1), new Vector2(bx + 1, by + size - 1));
			Handles.DrawLine (new Vector2(bx + 1, by + size - 1), new Vector2(bx + 1, by + 1));
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
					grid.Set (x, y, window.id, window.subid, window.state1, window.state2, window.state3);

					Event.current.Use ();
					break;
				}
			}
		}

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
	}
}
