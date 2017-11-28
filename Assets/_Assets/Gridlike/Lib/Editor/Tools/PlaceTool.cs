using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Gridlike {

	public class PlaceTool : GridTool {

		int radius = 1;
		int id = 1;

		bool brushIsCircular;

		bool replace;
		int replacedId;

		Dictionary<string, Texture2D> thumbnailCache;
		TileAtlas cachedAtlas;

		public PlaceTool() {
			radius = Mathf.Max(1, PlayerPrefs.GetInt ("grid.place.radius"));
			id = Mathf.Max(1, PlayerPrefs.GetInt ("grid.place.id"));

			brushIsCircular = PlayerPrefs.GetInt ("grid.place.brushIsCircular") == 1;

			replace = PlayerPrefs.GetInt ("grid.place.replace") == 1;
			replacedId = Mathf.Max (1, PlayerPrefs.GetInt ("grid.place.replaceId"));

			thumbnailCache = null;
		}

		public override void Serialize() {
			PlayerPrefs.SetInt ("grid.place.radius", radius);
			PlayerPrefs.SetInt ("grid.place.id", id);

			PlayerPrefs.SetInt ("grid.place.brushIsCircular", brushIsCircular ? 1 : 0);

			PlayerPrefs.SetInt ("grid.place.replace", replace ? 1 : 0);
			PlayerPrefs.SetInt ("grid.place.replaceId", replacedId);
		}

		public override bool UseWindow() {
			return true;
		}
		public override string Name() {
			return "place";
		}

		Texture GetThumbnail(Sprite sprite) {
			Texture2D texture = null;
			if (thumbnailCache == null || cachedAtlas != grid.atlas) {
				thumbnailCache = new Dictionary<string, Texture2D> ();
				cachedAtlas = grid.atlas;
			}

			if (!thumbnailCache.TryGetValue(sprite.name, out texture))
			{
				var rect = sprite.textureRect;
				texture = new Texture2D((int)rect.width, (int)rect.height);
				texture.SetPixels(sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
				texture.Apply();

				thumbnailCache[sprite.name] = texture;
			}
			return texture;
		}

		public override bool Window() {

			id = EditorGUILayout.IntField ("Id", id);
			if (grid.atlas == null || id <= 0 || id >= grid.atlas.atlas.Length) id = 0;
			if (grid.atlas == null || grid.atlas [id] == null) id = 0;

			EditorGUILayout.Space ();

			if (grid.atlas [id].tileGO == null) {
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

				EditorGUILayout.Space ();

				replace = EditorGUILayout.Toggle ("Replace", replace);

				if (replace) {
					replacedId = Mathf.Max (1, EditorGUILayout.IntField ("Replaced id", replacedId));
				}
			} else {
				GUI.enabled = false;
				radius = EditorGUILayout.IntField ("Radius", 1);
				GUI.enabled = true;
			}

			int count = 0;

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();

			foreach (TileInfo info in grid.atlas.GetTileInfos()) {
				if (count % 8 == 0 && count != 0) {
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
				}
				count++;
				GUIContent content;
				if (info.idSpriteInfo.sprite == null || info.tileGO != null) {
					content = new GUIContent (info.id.ToString());
				} else {
					content = new GUIContent (GetThumbnail (info.idSpriteInfo.sprite));
				}

				if (GUILayout.Toggle (info.id == id, content, GUI.skin.button, GUILayout.Width (40), GUILayout.Height (40))) {
					id = info.id;
				}
			}

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

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
		public override bool OnMouse() {
			Place ();
			return true;
		}

		void Place() {
			int x = mouseX, y = mouseY;

			int r = radius - 1;

			int[,] ids = new int[2 * r + 1, 2 * r + 1];
			for (int i = 0; i < ids.GetLength (0); i++) {
				for (int j = 0; j < ids.GetLength (1); j++) {
					// CHECK IF, IN REPLACE MODE, THE CURRENT ID IS CORRECT
					if (replace && grid.GetId (x - r + i, y - r + j) != replacedId) {
						continue;
					}

					// CHECK BRUSH TYPE
					float dx = i - r;
					float dy = j - r;
					if (brushIsCircular && dx * dx + dy * dy - 1 > r * r) {
						continue;
					}

					ids [i, j] = id;
				}
			}

			grid.Set (x - r, y - r, ids, true);

			grid.PresentContainingRegion (mouseX, mouseY);
		}
	}
}