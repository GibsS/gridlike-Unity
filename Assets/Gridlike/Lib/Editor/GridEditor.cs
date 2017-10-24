using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(Grid))]
// TODO manage to serialize
public class GridEditor : Editor {

	[SerializeField] public int currentTool = 0;

	[SerializeField] public GridTool[] tools;

	void OnChangePlayMode() {
		Grid grid = target as Grid;

		if (!Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode && grid.useLoading && grid.useAgentBasedLoading) {

			grid.HideAll ();
		}
	}

	void OnEnable() {
		EditorApplication.playmodeStateChanged += OnChangePlayMode;

		if(tools == null) {
			Debug.Log ("reset");
			tools = new GridTool[] {
				new PlaceTool(),
				new EraseTool(),
				new DragTool(),
				new InspectorTool(),
				new ShowRegionTool(),
				new HideRegionTool(),
				new EraseRegionTool(),
				new AreaTool()
			};
		}
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

		Handles.BeginGUI();

		// TOOL
		GridTool tool = tools [currentTool];
		tool._grid = grid;

		bool sendInput = true;

		// TOOL LIST
		GUILayout.BeginArea(new Rect(20, 20, 10 + 95 * tools.Length, 80));

		var rect = EditorGUILayout.BeginHorizontal ();

		for (int i = 0; i < tools.Length; i++) {
			GridTool optionTool = tools[i];

			if (GUI.Button (new Rect(85 * i, 0, 80, 20), optionTool.Name ())) {
				currentTool = i;
			}
		}

		GUILayout.EndHorizontal ();

		GUILayout.EndArea();

		// TOOL WINDOW
		if (tool.UseWindow ()) {
			GUILayout.BeginArea(new Rect(20, 60, 300, tool.WindowHeight()));

			var rect1 = EditorGUILayout.BeginVertical ();

			GUI.color = Color.white;
			GUI.Box(rect1, GUIContent.none);

			tool.Window ();

			sendInput = !rect.Contains (Event.current.mousePosition);

			EditorGUILayout.EndVertical ();

			GUILayout.EndArea();
		}

		Handles.EndGUI();

		tool.Update ();

		// TOOL INPUT
		if (Event.current.button == 0) {
			if (sendInput) {
				switch (Event.current.type) {
				case EventType.mouseDown:
					{
						tool.OnMouseDown ();
						break;
					}
				case EventType.mouseDrag:
					{
						tool.OnMouse ();
						break;
					}
				case EventType.mouseUp:
					{
						tool.OnMouseUp ();
						break;
					}
				}
			}

			if (Event.current.type == EventType.Layout) {
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			}
		}

		SceneView.RepaintAll ();
	}
}
