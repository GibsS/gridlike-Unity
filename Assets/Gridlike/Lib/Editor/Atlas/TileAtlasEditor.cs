using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;

public class TileEditorInfo {

	public bool isRevealed;
}

[CustomEditor(typeof(TileAtlas))]
public class GridTileAtlasEditor : Editor {

	bool showPosition;

	Dictionary<int, TileEditorInfo> tileEditorInfo;

	public override void OnInspectorGUI() {
		TileAtlas atlas = target as TileAtlas;

		GUI.enabled = false;
		EditorGUILayout.ObjectField("Sprite sheet", atlas.spriteSheet, typeof(Texture2D), false);
		GUI.enabled = true;

		atlas.tilePixelSize = EditorGUILayout.IntField ("Tile pixel size", atlas.tilePixelSize);

		if (GUILayout.Button ("Generate sprite sheet")) {
			GenerateSpriteSheet ();
		}

		foreach(TileInfo info in atlas.GetTileInfos()) {
			TileInfoUI (atlas, info);
		}

		if (GUILayout.Button ("Create new tile")) {
			atlas.AddTile ();
		}
	}
	void DrawOnGUISprite(Sprite aSprite) {
		Rect c = aSprite.rect;
		float spriteW = c.width;
		float spriteH = c.height;
		Rect rect = GUILayoutUtility.GetRect(spriteW, spriteH);
		if (Event.current.type == EventType.Repaint)
		{
			var tex = aSprite.texture;
			c.xMin /= tex.width;
			c.xMax /= tex.width;
			c.yMin /= tex.height;
			c.yMax /= tex.height;
			GUI.DrawTextureWithTexCoords(rect, tex, c);
		}
	}

	void TileInfoUI(TileAtlas atlas, TileInfo tile) {
		GUILayout.BeginVertical("HelpBox");

		EditorGUI.indentLevel++;

		TileEditorInfo info = GetTileEditorInfo (tile.id);

		EditorGUILayout.BeginHorizontal ();
		
		info.isRevealed = EditorGUILayout.Foldout (info.isRevealed, "ID:" + tile.id + " " + tile.name);

		GUILayout.FlexibleSpace ();

		if (tile.GetSprite (-1) != null) DrawOnGUISprite(tile.GetSprite(-1));
		
		if (GUILayout.Button ("remove", GUILayout.MaxWidth(80))) {
			atlas.RemoveTile (tile.id);
		}
		EditorGUILayout.EndHorizontal ();

		if (info.isRevealed) {
			tile.name = EditorGUILayout.TextField ("Name", tile.name);

			if (tile.tileGO == null) {
				tile.shape = (TileShape)EditorGUILayout.EnumPopup ("Shape", tile.shape);
				tile.isSensor = EditorGUILayout.Toggle ("Is sensor?", tile.isSensor);
				tile.layer = EditorGUILayout.LayerField ("Layer", tile.layer);
				tile.tag = EditorGUILayout.TagField ("Tag", tile.tag);
			} else {
				tile.shape = TileShape.EMPTY;
			}

			tile.tileGO = EditorGUILayout.ObjectField ("Tile GO", tile.tileGO, typeof(GameObject), false) as GameObject;
			if (tile.tileGO != null) {
				tile.isGODetached = EditorGUILayout.Toggle ("Tile GO is detached", tile.isGODetached);
			}

			if (TileShapeHelper.IsTriangle (tile.shape)) {
				//tile.idSpriteInfo
			} else {
				tile.idSpriteInfo.importedSprite = (Sprite) EditorGUILayout.ObjectField("Main sprite", tile.idSpriteInfo.importedSprite, typeof(Sprite), false);

				if (tile.subIdSpriteInfo != null) {
					for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
						if (tile.subIdSpriteInfo [i] != null) {
							tile.subIdSpriteInfo [i].importedSprite = (Sprite)EditorGUILayout.ObjectField ("Sub sprite " + i, tile.subIdSpriteInfo [i].importedSprite, typeof(Sprite), false);
						}
					}
				}

				if (GUILayout.Button ("Add sub id sprite")) {
					if (tile.subIdSpriteInfo == null) {
						tile.subIdSpriteInfo = new TileSpriteInfo[1];
					} else {
						int length = tile.subIdSpriteInfo.Length;

						TileSpriteInfo[] newSprites = new TileSpriteInfo[length + 1];
						for (int i = 0; i < length; i++) newSprites [i] = tile.subIdSpriteInfo [i];
						tile.subIdSpriteInfo = newSprites;
					}
				}
				if (GUILayout.Button ("Remove sub id sprite")) {
					if (tile.subIdSpriteInfo != null) {
						if (tile.subIdSpriteInfo.Length == 1) {
							tile.subIdSpriteInfo = null; 
						} else {
							int length = tile.subIdSpriteInfo.Length;

							TileSpriteInfo[] newSprites = new TileSpriteInfo[length - 1];
							for (int i = 0; i < length - 1; i++) newSprites [i] = tile.subIdSpriteInfo [i];
							tile.subIdSpriteInfo = newSprites;
						}
					}
				}
			}
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.EndHorizontal ();
		}

		EditorGUI.indentLevel--;

		GUILayout.EndVertical ();
	}

	TileEditorInfo GetTileEditorInfo(int id) {
		if (tileEditorInfo == null) tileEditorInfo = new Dictionary<int, TileEditorInfo> ();

		TileEditorInfo info;

		if (!tileEditorInfo.TryGetValue (id, out info)) {
			info = new TileEditorInfo ();
			tileEditorInfo [id] = info;
		}
		return info;
	}

	// TODO HANDLE LONG TRIANGLES
	// TODO SHRINK TO FIT EXACTLY
	void GenerateSpriteSheet() {
		TileAtlas atlas = target as TileAtlas;

		int size = atlas.tilePixelSize;
		int tilePerRow = Mathf.FloorToInt(TileAtlas.PIXEL_PER_ROW / size);

		Texture2D texture = new Texture2D (TileAtlas.PIXEL_PER_ROW, 256);
		Color[] colors = texture.GetPixels();
		for (int i = 0; i < colors.Length; i++) {
				colors [i] = Color.clear;
		}
		texture.SetPixels (colors);

		// TODO allow for more space
		SpriteMetaData[] sprites = new SpriteMetaData[atlas.Count * 4];

		int tileX = 0;
		int tileY = 0;

		int spriteInd = 0;

		foreach (TileInfo tile in atlas.GetTileInfos()) {
			if (tile.idSpriteInfo != null) {
				PackHorizontalSpriteInfo (tilePerRow, size, tile.idSpriteInfo, tile.id, -1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
			}

			if (tile.subIdSpriteInfo != null) {
				for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
					if (tile.subIdSpriteInfo [i] != null) {
						PackHorizontalSpriteInfo (tilePerRow, size, tile.subIdSpriteInfo [i], tile.id, i, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
					}
				}
			}
		}

		texture.Apply ();

		var bytes = texture.EncodeToPNG ();

		// TODO allow custom location
		// TODO default to a random name in a resource folder
		Directory.CreateDirectory(Application.dataPath + "/Resources");
		File.WriteAllBytes (Application.dataPath + "/Resources/ok.png", bytes);

		AssetDatabase.Refresh (ImportAssetOptions.ForceUpdate);

		texture = (Texture2D) Resources.Load("ok");

		atlas.spriteSheet = texture;

		// UPDATE IMPORT SETTINGS
		string assetPath = AssetDatabase.GetAssetPath(texture);
		TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

		importer.textureType = TextureImporterType.Sprite;

		importer.isReadable = true;
		importer.filterMode = FilterMode.Point;
		importer.textureType = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Multiple;
		importer.mipmapEnabled = false;
		importer.isReadable = true;
		importer.spritePixelsPerUnit = size;
		importer.spritesheet = sprites;

		AssetDatabase.ImportAsset(assetPath);
		AssetDatabase.Refresh();

		string pattern = @"sprite_(.*)_(.*)_(.*)";
		Regex rgx = new Regex(pattern);

		foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath (assetPath)) {
			if (!string.IsNullOrEmpty (obj.name) && obj.name [0] == 's') {
				Sprite sprite = obj as Sprite;

				string[] values = sprite.name.Split ('_');

				int id = int.Parse (values [1]);
				int subId = int.Parse (values [2]);
				int tileSize = int.Parse (values [3]);

				TileSpriteInfo spriteInfo;

				if (subId == -1) {
					spriteInfo = atlas [id].idSpriteInfo;
				} else {
					spriteInfo = atlas [id].subIdSpriteInfo [subId];
				}

				if (tileSize == 1) {
					spriteInfo.sprite = sprite;
				} else {
					spriteInfo.sprites [tileSize - 2] = sprite;
				}
			}
		}
	}

	void PackHorizontalSpriteInfo(int tilePerRow, int tileSize, TileSpriteInfo tileSpriteInfo, int id, int subId, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileSpriteInfo.importedSprite != null) {
			Debug.Log ("sub id=" + subId);
			PackHorizontalSprite (tilePerRow, tileSize, tileSpriteInfo.importedSprite, id, subId, 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
		}

		if (tileSpriteInfo.importedSprites != null) {
			for (int i = 0; i < tileSpriteInfo.importedSprites.Length; i++) {
				if (tileSpriteInfo.importedSprites [i] != null) {
					PackHorizontalSprite (tilePerRow, tileSize, tileSpriteInfo.importedSprites[i], id, subId, i+2, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
				}
			}
		}
	}

	void PackHorizontalSprite(int tilePerRow, int tileSize, Sprite sprite, int id, int subId, int tileWidth, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileX + tileWidth >= tilePerRow) {
			tileX = 0;
			tileY += 1;
		}

		texture.SetPixels (
			tileX * tileSize, 
			tileY * tileSize, 
			tileSize * tileWidth, 
			tileSize,
			sprite.texture.GetPixels (
				(int) sprite.textureRect.x,
				(int) sprite.textureRect.y,
				tileSize * tileWidth,
				tileSize
			)
		);

		sprites [spriteInd] = new SpriteMetaData {
			name = "sprite_" + id + "_" + subId + "_" + tileWidth,
			rect = new Rect (tileX * tileSize, tileY * tileSize, tileSize, tileSize)
		};

		spriteInd += 1;
		tileX += tileWidth;
	}
}