using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

namespace Gridlike {
	
	[CustomEditor(typeof(Grid))]
	public class GridEditor : Editor {

		int currentTool = 0;
		GridTool[] tools;

		Vector2 scroll = Vector2.zero;

		void OnChangePlayMode() {
			Grid grid = target as Grid;

			if (!Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
				grid.SaveExtra ();

				if (grid.useLoading && grid.useAgentBasedLoading) {
					grid.HideAll ();
				}
			}
		}

		void OnEnable() {
			EditorApplication.playmodeStateChanged += OnChangePlayMode;

			if(tools == null) {
				currentTool = PlayerPrefs.GetInt ("grid.currentTool");
				scroll = new Vector2(PlayerPrefs.GetFloat("grid.scroll.x"), PlayerPrefs.GetFloat("grid.scroll.y"));

				tools = new GridTool[] {
					new PlaceTool(),
					new EraseTool(),
					new InspectorTool(),
					new ShowRegionTool(),
					new HideRegionTool(),
					new EraseRegionTool(),
					new AreaTool()
				};
			}

			Grid grid = target as Grid;

			grid.LoadExtra ();
		}
		void OnDisable() {
			PlayerPrefs.SetInt ("grid.currentTool", currentTool);
			PlayerPrefs.SetFloat ("grid.scroll.x", scroll.x);
			PlayerPrefs.SetFloat ("grid.scroll.y", scroll.y);

			foreach (GridTool tool in tools) {
				tool.Serialize ();
			}
		}

		public override void OnInspectorGUI() {
			Grid grid = target as Grid;

			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Grid)target), typeof(Grid), false);
			GUI.enabled = true;

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

			if (GUI.changed && !Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		void OnSceneGUI() {
			Grid grid = target as Grid;

			// CAN'T EDIT IF THE ATLAS IS NOT SET
			if (grid.atlas == null) {
				Handles.BeginGUI();

				GUILayout.BeginArea(new Rect(20, 20, 400, 80));

				var rect1 = EditorGUILayout.BeginVertical ();

				GUI.color = Color.white;
				GUI.Box(rect1, GUIContent.none);

				EditorGUILayout.LabelField ("Grid tile atlas is not set (It can be set in the grid inspector)");

				EditorGUILayout.EndVertical ();

				GUILayout.EndArea ();

				Handles.EndGUI ();
				return;
			}

			// REGIONS
			int mouseX, mouseY;

			grid.WorldToGrid(HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin, out mouseX, out mouseY);

			int mouseRegionX = Mathf.FloorToInt (mouseX / (float)Grid.REGION_SIZE);
			int mouseRegionY = Mathf.FloorToInt (mouseY / (float)Grid.REGION_SIZE);
				
			foreach (FiniteGrid region in grid.GetRegions()) {
				float size = Grid.REGION_SIZE;
				float bx = grid.transform.position.x + region.regionX * size;
				float by = grid.transform.position.y + region.regionY * size;

				if (mouseRegionX == region.regionX && mouseRegionY == region.regionY) {
					Handles.color = new Color(1, 1, 1, 0.2f);

					for (int i = 0; i <= Grid.REGION_SIZE; i++) {
						Handles.DrawLine (new Vector2 (bx + i, by), new Vector2 (bx + i, by + Grid.REGION_SIZE));

						Handles.DrawLine (new Vector2 (bx, by + i), new Vector2 (bx + Grid.REGION_SIZE, by + i));
					}
				}


				Handles.color = region.presented ? Color.green : Color.white;

				Handles.DrawLine (new Vector2(bx + 0.2f, by + 0.2f), new Vector2(bx + size - 0.2f, by + 0.2f));
				Handles.DrawLine (new Vector2(bx + size - 0.2f, by + 0.2f), new Vector2(bx + size - 0.2f, by + size - 0.2f));
				Handles.DrawLine (new Vector2(bx + size - 0.2f, by + size - 0.2f), new Vector2(bx + 0.2f, by + size - 0.2f));
				Handles.DrawLine (new Vector2(bx + 0.2f, by + size - 0.2f), new Vector2(bx + 0.2f, by + 0.2f));
			}

			// TOOL
			GridTool tool = tools [currentTool];
			tool._grid = grid;

			bool didSomething = false;
			bool acceptClick = true;

			didSomething = tool.Update () || didSomething;

			Handles.BeginGUI();

			// TOOL LIST
			GUILayout.BeginArea(new Rect(20, 20, 10 + 95 * tools.Length, 80));

			EditorGUILayout.BeginHorizontal ();

			for (int i = 0; i < tools.Length; i++) {
				GridTool optionTool = tools[i];

				GUI.color = i == currentTool ? Color.cyan : Color.white;
				if (GUILayout.Button (optionTool.Name (), GUILayout.MinHeight(40))) {
					currentTool = i;
				}
				GUI.color = Color.white;
			}

			GUILayout.EndHorizontal ();

			GUILayout.EndArea();

			// TOOL WINDOW
			if (tool.UseWindow ()) {
				GUIStyle myStyle = new GUIStyle (GUI.skin.box); 
				myStyle.padding = new RectOffset(8, 8, 8, 8);

				GUILayout.BeginArea(new Rect(20, 90, 400, tool.WindowHeight()));
				scroll = GUILayout.BeginScrollView (scroll);
				//GUI.backgroundColor = Color.clear;
				var rect1 = EditorGUILayout.BeginVertical (myStyle);

				GUI.backgroundColor = Color.white;
				GUI.color = Color.white;
				GUI.Box(rect1, GUIContent.none);

				EditorGUILayout.LabelField(tool.Name().ToUpper(), EditorStyles.boldLabel);

				didSomething = tool.Window () || didSomething;

				EditorGUILayout.EndVertical ();
				GUILayout.EndScrollView ();
				GUILayout.EndArea();

				EventType type = Event.current.type;
				if (Event.current.button == 0 && (type == EventType.mouseDown || type == EventType.mouseUp || type == EventType.mouseDrag)) {
					acceptClick = !rect1.Contains (Event.current.mousePosition);
				}
			}

			Handles.EndGUI();

			// TOOL INPUT
			if (Event.current.button == 0) {
				if (acceptClick) {
					switch (Event.current.type) {
					case EventType.mouseDown:
						{
							didSomething = tool.OnMouseDown () || didSomething;
							break;
						}
					case EventType.mouseDrag:
						{
							didSomething = tool.OnMouse () || didSomething;
							break;
						}
					case EventType.mouseUp:
						{
							didSomething = tool.OnMouseUp () || didSomething;
							break;
						}
					}
				}

				if (Event.current.type == EventType.Layout) {
					HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				}
			}

			SceneView.RepaintAll ();

			if (didSomething && !Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}
}