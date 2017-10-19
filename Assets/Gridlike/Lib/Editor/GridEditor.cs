using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

	void OnChangePlayMode() {
		Grid grid = target as Grid;

		if (!Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode && grid.useLoading && grid.useAgentBasedLoading) {

			grid.HideAll ();
		}
	}

	void OnEnable() {
		EditorApplication.playmodeStateChanged += OnChangePlayMode;
	}

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
			grid.saveOnClose = EditorGUILayout.ToggleLeft ("Save grid on destroy", grid.saveOnClose);
		}
	}

	void OnSceneGUI() {
		Grid grid = target as Grid;

		int mouseX, mouseY;

		grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

		// REGIONS
		foreach (FiniteGrid region in grid.GetRegions()) {
			float size = Grid.REGION_SIZE * grid.tileSize;
			float bx = grid.transform.position.x + region.regionX * size;
			float by = grid.transform.position.y + region.regionY * size;

			Handles.color = region.presented ? Color.green : Color.white;

			Handles.DrawLine (new Vector2(bx + 0.2f, by + 0.2f), new Vector2(bx + size - 0.2f, by + 0.2f));
			Handles.DrawLine (new Vector2(bx + size - 0.2f, by + 0.2f), new Vector2(bx + size - 0.2f, by + size - 0.2f));
			Handles.DrawLine (new Vector2(bx + size - 0.2f, by + size - 0.2f), new Vector2(bx + 0.2f, by + size - 0.2f));
			Handles.DrawLine (new Vector2(bx + 0.2f, by + size - 0.2f), new Vector2(bx + 0.2f, by + 0.2f));
		}

		// TILE
		// DrawTileInformation(grid, mouseX, mouseY);

		// BRUSH
		if (Event.current.button == 0) {
			switch (Event.current.type) {
				/*case EventType.mouseDown: {
					Event.current.Use ();
					break;
				}*/
				case EventType.mouseUp: {
					GridEditorWindow window = GridEditorWindow.ShowWindow ();
					grid.Set (mouseX, mouseY, window.id, window.subid, window.state1, window.state2, window.state3);

					grid.PresentContainingRegion (mouseX, mouseY);

					Event.current.Use ();
					break;
				}
			}

			if (Event.current.type == EventType.Layout) {
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			}
		}

		// SceneView.RepaintAll ();
	}

	void DrawTileInformation(Grid grid, int mouseX, int mouseY) {
		Vector2 position = grid.transform.TransformPoint(new Vector2(mouseX * grid.tileSize, mouseY * grid.tileSize));

		Handles.color = Color.white;

		Handles.DrawLine (new Vector2(position.x + 0.1f, position.y + 0.1f), new Vector2(position.x + 0.9f, position.y + 0.1f));
		Handles.DrawLine (new Vector2(position.x + 0.9f, position.y + 0.1f), new Vector2(position.x + 0.9f, position.y + 0.9f));
		Handles.DrawLine (new Vector2(position.x + 0.9f, position.y + 0.9f), new Vector2(position.x + 0.1f, position.y + 0.9f));
		Handles.DrawLine (new Vector2(position.x + 0.1f, position.y + 0.9f), new Vector2(position.x + 0.1f, position.y + 0.1f));

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		Handles.Label (new Vector2 (position.x + 1.1f, position.y + 1), "Hello", style);
	}
}
