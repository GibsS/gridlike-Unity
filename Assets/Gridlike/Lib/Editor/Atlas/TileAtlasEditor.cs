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

		// SPRITE SHEET PATH
		atlas.spriteSheetPath = EditorGUILayout.TextField ("Sprite sheet path", atlas.spriteSheetPath);
		if (string.IsNullOrEmpty (atlas.spriteSheetPath)) {
			atlas.spriteSheetPath = "Resources/sprite_sheet.png";
		} else {
			if (!atlas.spriteSheetPath.EndsWith (".png")) {
				atlas.spriteSheetPath += ".png";
			}
		}

		// TILE PIXEL SIZE
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
		if (GUI.changed)
			EditorUtility.SetDirty (target);
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

		if (tile.GetSprite () != null) DrawOnGUISprite(tile.GetSprite());
		
		if (GUILayout.Button ("remove", GUILayout.MaxWidth(80))) {
			atlas.RemoveTile (tile.id);
		}
		EditorGUILayout.EndHorizontal ();

		if (info.isRevealed) {
			tile.name = EditorGUILayout.TextField ("Name", tile.name);

			if (tile.tileGO == null) {
				tile.shape = (TileShape)EditorGUILayout.EnumPopup ("Shape", tile.shape);

				if (TileShapeHelper.IsTriangle (tile.shape)) {
					tile.isVertical = EditorGUILayout.Toggle ("Triangle can stretch vertically", tile.isVertical);
					tile.isVertical = !EditorGUILayout.Toggle ("Triangle can stretch horizontally", !tile.isVertical);
				} else {
					tile.isVertical = false;
				}

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

			bool isTriangle = TileShapeHelper.IsTriangle (tile.shape);

			if (isTriangle) {
				TriangleSpriteChooser (tile, tile.idSpriteInfo, 0);

				if (tile.subIdSpriteInfo != null) {
					for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
						TriangleSpriteChooser (tile, tile.subIdSpriteInfo [i], i);
					}
				}
			} else {
				tile.idSpriteInfo.importedSprite = (Sprite) EditorGUILayout.ObjectField("Main sprite", tile.idSpriteInfo.importedSprite, typeof(Sprite), false);

				if (tile.subIdSpriteInfo != null) {
					for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
						if (tile.subIdSpriteInfo [i] != null) {
							tile.subIdSpriteInfo [i].importedSprite = (Sprite)EditorGUILayout.ObjectField ("Sub id sprite " + i, tile.subIdSpriteInfo [i].importedSprite, typeof(Sprite), false);
						}
					}
				}
			}

			if (GUILayout.Button ("Add sub id sprite")) {
				tile.AddSubId ();
			}
			if (GUILayout.Button ("Remove sub id sprite")) {
				tile.RemoveSubId ();
			}

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.EndHorizontal ();
		}

		EditorGUI.indentLevel--;

		GUILayout.EndVertical ();

		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	void TriangleSpriteChooser(TileInfo tile, TileSpriteInfo spriteInfo, int subId) {
		GUILayout.BeginVertical("HelpBox");

		spriteInfo.importedSprite = (Sprite) EditorGUILayout.ObjectField("Main sprite size=1", spriteInfo.importedSprite, typeof(Sprite), false);

		if (spriteInfo.importedSprites != null) {
			for (int i = 0; i < spriteInfo.importedSprites.Length; i++) {
				spriteInfo.importedSprites [i] = (Sprite)EditorGUILayout.ObjectField ("Main sprite size=" + (i + 2), spriteInfo.importedSprites [i], typeof(Sprite), false);
			}
		}

		if (GUILayout.Button ("Add sprite size")) {
			tile.AddSpriteSize (subId);
		}
		if (GUILayout.Button ("Remove sprite size")) {
			tile.RemoveSpriteSize (subId);
		}

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

	void GenerateSpriteSheet() {
		TileAtlas atlas = target as TileAtlas;

		EditorUtility.DisplayProgressBar("Gridlike", "Generating tile atlas sprite sheet", 0);

		int tilePixelSize = atlas.tilePixelSize;

		int tileCount = atlas.TotalSpriteTileCount;
		int tilePerRow = Mathf.Min(Mathf.FloorToInt(Mathf.Sqrt(tileCount * 4)), TileAtlas.MAX_SPRITE_SHEET_SIZE / tilePixelSize);
		int tilePerColumn = tilePerRow;

		Texture2D texture = new Texture2D (tilePerRow * tilePixelSize, tilePerColumn * tilePixelSize);
		Color[] colors = texture.GetPixels();
		for (int i = 0; i < colors.Length; i++) {
			colors [i] = Color.clear;
		}
		texture.SetPixels (colors);

		SpriteMetaData[] sprites = new SpriteMetaData[atlas.SpriteCount+1];

		int tileX = 0;
		int tileY = 0;

		int spriteInd = 0;

		// THE EMPTY SPRITE
		Color[] clearColors = new Color[tilePixelSize * tilePixelSize];
		for (int i = 0; i < clearColors.Length; i++) clearColors [i] = Color.clear;
		PackHorizontalSprite (tilePerRow, tilePixelSize, clearColors, -1, 0, 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);

		// ALL HORIZONTAL SPRITES
		foreach (TileInfo tile in atlas.GetTileInfos()) {
			if (tile.idSpriteInfo != null) {
				PackHorizontalSpriteInfo (tilePerRow, tilePixelSize, tile.idSpriteInfo, tile.isVertical, tile.id, 0, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
			}

			if (tile.subIdSpriteInfo != null) {
				for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
					if (tile.subIdSpriteInfo [i] != null) {
						PackHorizontalSpriteInfo (tilePerRow, tilePixelSize, tile.subIdSpriteInfo [i], tile.isVertical, tile.id, i + 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
					}
				}
			}
		}

		tileX = 0;
		tileY += 1;

		int minTileY = tileY;

		// ALL VERTICAL SPRITES
		foreach (TileInfo tile in atlas.GetTileInfos()) {
			if (tile.isVertical) {
				if (tile.idSpriteInfo != null) {
					PackVerticalSpriteInfo (tilePerRow, minTileY, tilePixelSize, tile.idSpriteInfo, tile.id, 0, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
				}

				if (tile.subIdSpriteInfo != null) {
					for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
						if (tile.subIdSpriteInfo [i] != null) {
							PackVerticalSpriteInfo (tilePerRow, minTileY, tilePixelSize, tile.subIdSpriteInfo [i], tile.id, i + 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
						}
					}
				}
			}
		}

		texture.Apply ();

		var bytes = texture.EncodeToPNG ();

		var directory = Path.GetDirectoryName(Application.dataPath + "/" + atlas.spriteSheetPath);
		Directory.CreateDirectory(directory);
		File.WriteAllBytes (Application.dataPath + "/" + atlas.spriteSheetPath, bytes);

		AssetDatabase.Refresh (ImportAssetOptions.ForceUpdate);

		texture = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/" + atlas.spriteSheetPath, typeof(Texture2D));

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
		importer.spritePixelsPerUnit = tilePixelSize;
		importer.spritesheet = sprites;

		AssetDatabase.ImportAsset(assetPath);
		AssetDatabase.Refresh();

		string pattern = @"sprite_(.*)_(.*)_(.*)";
		Regex rgx = new Regex(pattern);

		foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath (assetPath)) {
			if (!string.IsNullOrEmpty (obj.name) && obj.name [0] == 's' && obj.name != "sprite_sheet") {
				Sprite sprite = obj as Sprite;

				string[] values = sprite.name.Split ('_');

				int id = int.Parse (values [1]);
				int subId = int.Parse (values [2]);
				int tileSize = int.Parse (values [3]);

				if(id == -1) {
					atlas.emptySprite = sprite;
				} else {
					TileSpriteInfo spriteInfo;

					if (subId == 0) {
						spriteInfo = atlas [id].idSpriteInfo;
					} else {
						spriteInfo = atlas [id].subIdSpriteInfo [subId - 1];
					}

					if (tileSize == 1) {
						spriteInfo.sprite = sprite;
					} else {
						spriteInfo.sprites = new Sprite[spriteInfo.importedSprites.Length];
						spriteInfo.sprites [tileSize - 2] = sprite;
					}
				}
			}
		}

		EditorUtility.ClearProgressBar();
	}

	void PackHorizontalSpriteInfo(int tilePerRow, int tileSize, TileSpriteInfo tileSpriteInfo, bool isVertical, int id, int subId, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileSpriteInfo.importedSprite != null) {
			PackHorizontalSprite (tilePerRow, tileSize, tileSpriteInfo.importedSprite, id, subId, 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
		}

		if (!isVertical && tileSpriteInfo.importedSprites != null) {
			for (int i = 0; i < tileSpriteInfo.importedSprites.Length; i++) {
				if (tileSpriteInfo.importedSprites [i] != null) {
					PackHorizontalSprite (tilePerRow, tileSize, tileSpriteInfo.importedSprites[i], id, subId, i+2, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
				}
			}
		}
	}
	void PackVerticalSpriteInfo(int tilePerColumn, int minTileY, int tileSize, TileSpriteInfo tileSpriteInfo, int id, int subId, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileSpriteInfo.importedSprites != null) {
			for (int i = 0; i < tileSpriteInfo.importedSprites.Length; i++) {
				if (tileSpriteInfo.importedSprites [i] != null) {
					PackVerticalSprite (tilePerColumn, minTileY, tileSize, tileSpriteInfo.importedSprites [i], id, subId, i + 2, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
				}
			}
		}
	}

	void PackHorizontalSprite(int tilePerRow, int tileSize, Sprite sprite, int id, int subId, int tileWidth, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		PackHorizontalSprite (
			tilePerRow, 
			tileSize, 
			sprite.texture.GetPixels (
				(int) sprite.textureRect.x,
				(int) sprite.textureRect.y,
				tileSize * tileWidth,
				tileSize
			),
			id,
			subId,
			tileWidth,
			ref tileX,
			ref tileY,
			ref spriteInd,
			ref sprites,
			ref texture
		);
	}

	void PackHorizontalSprite(int tilePerRow, int tileSize, Color[] colors, int id, int subId, int tileWidth, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileX + tileWidth >= tilePerRow) {
			tileX = 0;
			tileY += 1;
		}

		texture.SetPixels (
			tileX * tileSize, 
			tileY * tileSize, 
			tileSize * tileWidth, 
			tileSize,
			colors
		);

		sprites [spriteInd] = new SpriteMetaData {
			name = "sprite_" + id + "_" + subId + "_" + tileWidth,
			rect = new Rect (tileX * tileSize, tileY * tileSize, tileSize * tileWidth, tileSize)
		};

		spriteInd += 1;
		tileX += tileWidth;
	}

	void PackVerticalSprite(int tilePerColumn, int minTileY, int tileSize, Sprite sprite, int id, int subId, int tileHeight, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
		if (tileY + tileHeight >= tilePerColumn) {
			tileX += 1;
			tileY = minTileY;
		}

		texture.SetPixels (
			tileX * tileSize, 
			tileY * tileSize, 
			tileSize, 
			tileSize * tileHeight,
			sprite.texture.GetPixels (
				(int) sprite.textureRect.x,
				(int) sprite.textureRect.y,
				tileSize,
				tileSize * tileHeight
			)
		);

		sprites [spriteInd] = new SpriteMetaData {
			name = "sprite_" + id + "_" + subId + "_" + tileHeight,
			rect = new Rect (tileX * tileSize, tileY * tileSize, tileSize, tileSize * tileHeight)
		};

		spriteInd += 1;
		tileY += tileHeight;
	}
}