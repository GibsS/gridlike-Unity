﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Gridlike {
	
	public class TileEditorInfo {

		public bool isRevealed;
	}

	[CustomEditor(typeof(TileAtlas))]
	public class GridTileAtlasEditor : Editor {

		bool showPosition;

		Dictionary<int, TileEditorInfo> tileEditorInfo;


		public string spriteSheetPath { 
			get { 
				return Application.dataPath + "/" + spriteSheetProjectPath;
			} 
		}
		public string scriptPath {
			get {
				return Application.dataPath + "/" + scriptProjectPath;
			}
		}
		// PATH AFTER ASSETS example: for sprite sheet to be place at c:/a folder/Unity project/Assets/test/sprite_sheet.png, 
		// will return test/sprite_sheet.png
		public string spriteSheetProjectPath {
			get { 
				TileAtlas atlas = target as TileAtlas;

				if (atlas.useRelativePath) {
					return Path.GetDirectoryName (AssetDatabase.GetAssetPath (target)).Substring(7) + "/" + atlas.spriteSheetPath;
				} else {
					return atlas.spriteSheetPath;
				}
			}
		}
		public string scriptProjectPath {
			get {
				TileAtlas atlas = target as TileAtlas;

				if (atlas.useRelativePath) {
					return Path.GetDirectoryName (AssetDatabase.GetAssetPath (target)).Substring(7) + "/" + atlas.scriptPath;
				} else {
					return atlas.scriptPath;
				}
			}
		}

		public override void OnInspectorGUI() {
			TileAtlas atlas = target as TileAtlas;

			atlas.useRelativePath = EditorGUILayout.Toggle("Paths are relative", atlas.useRelativePath);

			if (GUILayout.Button ("bump minor")) {
				atlas.BumpMinor ();
			}
			if (GUILayout.Button ("bump major")) {
				atlas.BumpMajor ();
			}

			GUI.enabled = false;
			EditorGUILayout.ObjectField("Sprite sheet", atlas.spriteSheet, typeof(Texture2D), false);
			GUI.enabled = true;

			atlas.material = EditorGUILayout.ObjectField ("Material", atlas.material, typeof(Material), false) as Material;
			if (atlas.material == null) {
				atlas.material = new Material (Shader.Find ("Sprites/Default"));
			}
			atlas.material.mainTexture = atlas.spriteSheet;

			// SPRITE SHEET PATH
			atlas.spriteSheetPath = EditorGUILayout.TextField ("Sprite sheet path", atlas.spriteSheetPath);
			if (string.IsNullOrEmpty (atlas.spriteSheetPath)) {
				atlas.spriteSheetPath = "sprite_sheet.png";
			} else {
				if (!atlas.spriteSheetPath.EndsWith (".png")) {
					atlas.spriteSheetPath += ".png";
				}
			}

			// TILE PIXEL SIZE
			atlas.tilePixelSize = EditorGUILayout.IntField ("Tile pixel size", atlas.tilePixelSize);
			atlas.tilePixelSize = Mathf.Clamp (atlas.tilePixelSize, 1, 1024);

			if (atlas.majorVersion != atlas.spriteSheetMajorVersion) {
				EditorGUILayout.HelpBox ("Sprite sheet is outdated. It requires regeneration", MessageType.Error);
			} else if (atlas.minorVersion != atlas.spriteSheetMinorVersion) {
				EditorGUILayout.HelpBox ("Sprite sheet is outdated. It requires regeneration", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox ("Sprite sheet is up to date", MessageType.Info);
			}

			if (GUILayout.Button ("Generate sprite sheet")) {
				GenerateSpriteSheet ();
			}

			// HELPER SCRIPT
			atlas.scriptPath = EditorGUILayout.TextField ("Script path", atlas.scriptPath);
			if (string.IsNullOrEmpty (atlas.scriptPath)) {
				atlas.scriptPath = atlas.name + "Script.cs";
			} else {
				if (!atlas.scriptPath.EndsWith (".cs")) {
					atlas.scriptPath += ".cs";
				}
			}

			if (atlas.majorVersion != atlas.helperMajorVersion) {
				EditorGUILayout.HelpBox ("Helper is outdated. It requires regeneration", MessageType.Error);
			} else if (atlas.minorVersion != atlas.helperMinorVersion) {
				EditorGUILayout.HelpBox ("Helper is outdated. It requires regeneration", MessageType.Warning);
			} else {
				EditorGUILayout.HelpBox ("Helper is up to date", MessageType.Info);
			}

			if (GUILayout.Button ("Generate script")) {
				GenerateScript ();
			}

			foreach(TileInfo info in atlas.GetTileInfos()) {
				TileInfoUI (atlas, info);
			}

			if (GUILayout.Button ("Create new tile")) {
				atlas.AddTile ();

				atlas.BumpMinor ();
			}

			if (GUI.changed) EditorUtility.SetDirty (target);
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

			Sprite mainSprite = tile.GetSprite ();
			if (mainSprite != null) DrawOnGUISprite(mainSprite);
			
			if (GUILayout.Button ("remove", GUILayout.MaxWidth(80))) {
				atlas.RemoveTile (tile.id);

				atlas.BumpMajor ();
			}
			EditorGUILayout.EndHorizontal ();

			if (info.isRevealed) {
				string old = EditorGUILayout.TextField ("Name", tile.name);
				if (old != tile.name) {
					tile.name = old;

					atlas.BumpMinor ();
				}

				if (tile.tileGO == null) {
					TileShape shape = (TileShape) EditorGUILayout.EnumPopup ("Shape", tile.shape);
					if (shape != tile.shape) {
						tile.shape = shape;

						atlas.BumpMinor ();
					}

					bool isVertical;

					if (TileShapeHelper.IsTriangle (tile.shape)) {
						isVertical = EditorGUILayout.Toggle ("Triangle can stretch vertically", tile.isVertical);
						isVertical = !EditorGUILayout.Toggle ("Triangle can stretch horizontally", !tile.isVertical);
					} else {
						isVertical = tile.shape == TileShape.LEFT_ONEWAY || tile.shape == TileShape.RIGHT_ONEWAY;
					}

					if (tile.isVertical != isVertical) {
						tile.isVertical = isVertical;

						atlas.BumpMinor ();
					}

					bool isTrigger = EditorGUILayout.Toggle ("Is trigger?", tile.isTrigger);
					if (isTrigger != tile.isTrigger) {
						tile.isTrigger = isTrigger;

						atlas.BumpMinor ();
					}
					int layer = EditorGUILayout.LayerField ("Layer", tile.layer);
					if(layer != tile.layer) {
						tile.layer = layer;

						atlas.BumpMinor ();
					}
					string tag = EditorGUILayout.TagField ("Tag", tile.tag);
					if (tag != tile.tag) {
						tile.tag = tag;

						atlas.BumpMinor ();
					}
				} else {
					if (tile.shape != TileShape.EMPTY) {
						tile.shape = TileShape.EMPTY;

						atlas.BumpMinor ();
					}
				}

				GameObject obj = EditorGUILayout.ObjectField ("Tile GO", tile.tileGO, typeof(GameObject), false) as GameObject;
				if (tile.tileGO != obj) {
					tile.tileGO = obj;

					atlas.BumpMajor ();
				}
				if (tile.tileGO != null) {
					bool val = EditorGUILayout.Toggle ("Tile GO is detached", tile.isGODetached);
					if(val != tile.isGODetached) {
						tile.isGODetached = val;

						atlas.BumpMinor ();
					}
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
					Sprite sprite = (Sprite) EditorGUILayout.ObjectField("Main sprite", tile.idSpriteInfo.importedSprite, typeof(Sprite), false);
					if(tile.idSpriteInfo.importedSprite != sprite) {
						tile.idSpriteInfo.importedSprite = sprite;

						atlas.BumpMinor ();
					}

					if (tile.subIdSpriteInfo != null) {
						for (int i = 0; i < tile.subIdSpriteInfo.Length; i++) {
							if (tile.subIdSpriteInfo [i] != null) {
								sprite = (Sprite)EditorGUILayout.ObjectField ("Sub id sprite " + i, tile.subIdSpriteInfo [i].importedSprite, typeof(Sprite), false);

								if(sprite != tile.subIdSpriteInfo[i].importedSprite) {
									tile.subIdSpriteInfo [i].importedSprite = sprite;

									atlas.BumpMinor ();
								}
							}
						}
					}
				}

				if (GUILayout.Button ("Add sub id sprite")) {
					tile.AddSubId ();

					atlas.BumpMinor ();
				}
				if (GUILayout.Button ("Remove sub id sprite")) {
					tile.RemoveSubId ();

					atlas.BumpMajor ();
				}

				EditorGUILayout.BeginHorizontal ();

				EditorGUILayout.EndHorizontal ();
			}

			EditorGUI.indentLevel--;

			GUILayout.EndVertical ();

			if (GUI.changed) {
				EditorUtility.SetDirty (target);
			}
		}

		void TriangleSpriteChooser(TileInfo tile, TileSpriteInfo spriteInfo, int subId) {
			TileAtlas atlas = target as TileAtlas;

			GUILayout.BeginVertical("HelpBox");

			Sprite sprite = (Sprite) EditorGUILayout.ObjectField("Main sprite size=1", spriteInfo.importedSprite, typeof(Sprite), false);
			if (spriteInfo.importedSprite != sprite) {
				spriteInfo.importedSprite = sprite;

				atlas.BumpMinor ();
			}

			if (spriteInfo.importedSprites != null) {
				for (int i = 0; i < spriteInfo.importedSprites.Length; i++) {
					sprite = (Sprite) EditorGUILayout.ObjectField (
						"Main sprite size=" + (i + 2), 
						spriteInfo.importedSprites [i], 
						typeof(Sprite), 
						false
					);

					if (spriteInfo.importedSprites [i] != sprite) {
						spriteInfo.importedSprites [i] = sprite;

						atlas.BumpMinor ();
					}
				}
			}

			if (GUILayout.Button ("Add sprite size")) {
				tile.AddSpriteSize (subId);

				atlas.BumpMinor ();
			}
			if (GUILayout.Button ("Remove sprite size")) {
				tile.RemoveSpriteSize (subId);

				atlas.BumpMinor ();
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
			int tilePerRow = Mathf.Clamp(Mathf.FloorToInt(Mathf.Sqrt(tileCount * 4)), 2, TileAtlas.MAX_SPRITE_SHEET_SIZE / tilePixelSize);
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

			var directory = Path.GetDirectoryName(spriteSheetPath);
			Directory.CreateDirectory(directory);
			File.WriteAllBytes (spriteSheetPath, bytes);

			AssetDatabase.Refresh (ImportAssetOptions.ForceUpdate);

			texture = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/" + spriteSheetProjectPath, typeof(Texture2D));

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

			foreach (TileInfo info in atlas.GetTileInfos()) {
				if (info.idSpriteInfo != null && info.idSpriteInfo.importedSprites != null && info.idSpriteInfo.importedSprites.Length > 0) {
					info.idSpriteInfo.sprites = new Sprite[info.idSpriteInfo.importedSprites.Length];
				}

				if (info.subIdSpriteInfo != null) {
					for (int i = 0; i < info.subIdSpriteInfo.Length; i++) {
						if (info.subIdSpriteInfo [i].importedSprites != null && info.subIdSpriteInfo [i].importedSprites.Length > 0) {
							info.subIdSpriteInfo [i].sprites = new Sprite[info.subIdSpriteInfo [i].importedSprites.Length];
						}
					}
				}
			}

			foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath (assetPath)) {
				if (!string.IsNullOrEmpty (obj.name) && obj.name [0] == 's' && obj.name != "sprite_sheet" && obj is Sprite) {
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
							spriteInfo.sprites [tileSize - 2] = sprite;
						}
					}
				}
			}

			EditorUtility.ClearProgressBar();

			atlas.spriteSheetMajorVersion = atlas.majorVersion;
			atlas.spriteSheetMinorVersion = atlas.minorVersion;
		}

		void PackHorizontalSpriteInfo(int tilePerRow, int tileSize, TileSpriteInfo tileSpriteInfo, bool isVertical, int id, int subId, ref int tileX, ref int tileY, ref int spriteInd, ref SpriteMetaData[] sprites, ref Texture2D texture) {
			if (tileSpriteInfo.importedSprite != null) {
				PackHorizontalSprite (tilePerRow, tileSize, tileSpriteInfo.importedSprite, id, subId, 1, ref tileX, ref tileY, ref spriteInd, ref sprites, ref texture);
			}

			// ONLY FOR TRIANGLES
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

		void GenerateScript() {
			TileAtlas atlas = target as TileAtlas;

			string className = Path.GetFileNameWithoutExtension (scriptProjectPath);

			string copyPath = "Assets/" + scriptProjectPath;
			using (StreamWriter outfile = new StreamWriter(copyPath)) {
				outfile.WriteLine("// AUTOMATICALLY GENERATED. DO NOT EDIT MANUALLY.");
				outfile.WriteLine("");
				outfile.WriteLine("using UnityEngine;");
				outfile.WriteLine("using System;");
				outfile.WriteLine("using System.Collections;");
				outfile.WriteLine("");
				outfile.WriteLine("using Gridlike;");
				outfile.WriteLine("");
				outfile.WriteLine("[Serializable]");
				outfile.WriteLine("public class " + className + " : TileAtlasHelper {");
				outfile.WriteLine("");
				outfile.WriteLine("\tpublic static " + className + " helper;");
				outfile.WriteLine("\tpublic static TileAtlas atlas { get { return helper._atlas; } }");
				outfile.WriteLine("");
				outfile.WriteLine("\tpublic " + className + "() {");
				outfile.WriteLine("\t\thelper = this;");
				outfile.WriteLine("\t}");
				outfile.WriteLine("");

				foreach (TileInfo tile in atlas.GetTileInfos()) {
					outfile.WriteLine ("\tpublic const int " + tile.name.Replace (" ", "_").ToUpper() + " = " + tile.id + ";");
				}

				outfile.WriteLine("}");
			}
			AssetDatabase.Refresh();

			atlas.helperMajorVersion = atlas.majorVersion;
			atlas.helperMinorVersion = atlas.minorVersion;
		}
	}
}