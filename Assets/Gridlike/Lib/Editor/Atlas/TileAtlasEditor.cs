using System.Collections;
using System.Collections.Generic;
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

	void TileInfoUI(TileAtlas atlas, TileInfo tile) {
		GUILayout.BeginVertical("HelpBox");

		EditorGUI.indentLevel++;

		TileEditorInfo info = GetTileEditorInfo (tile.id);

		EditorGUILayout.BeginHorizontal ();
		
		info.isRevealed = EditorGUILayout.Foldout (info.isRevealed, "ID:" + tile.id + " " + tile.name);

		GUILayout.FlexibleSpace ();

		if (tile.GetSprite (-1) != null) GUILayout.Label (tile.GetSprite (-1).texture);
		
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
				// TODO
			} else {
				tile.idSpriteInfo.importedSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tile.idSpriteInfo.importedSprite, typeof(Sprite), false);
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

	void GenerateSpriteSheet() {
		TileAtlas atlas = target as TileAtlas;

		int size = atlas.tilePixelSize;
		int tilePerRow = Mathf.FloorToInt(TileAtlas.PIXEL_PER_ROW / size);

		Texture2D texture = new Texture2D (32, 32);

		SpriteMetaData[] sprites = new SpriteMetaData[4];

		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				Sprite importedSprite = atlas.GetTile (1).idSpriteInfo.importedSprite;
				if(i != 1 && j != 1) texture.SetPixels (i * size, j * size, size, size, importedSprite.texture.GetPixels ());
			}
		}

		texture.Apply ();

		var bytes = texture.EncodeToPNG ();

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

		importer.spritesheet = new SpriteMetaData[] {
			new SpriteMetaData {
				name = "sprite1",
				rect = new Rect(0, 0, 16, 16)
			},
			new SpriteMetaData {
				name = "sprite2",
				rect = new Rect(16, 16, 16, 16)
			},
			new SpriteMetaData {
				name = "sprite3",
				rect = new Rect(16, 0, 16, 16)
			},
			new SpriteMetaData {
				name = "sprite4",
				rect = new Rect(0, 16, 16, 16)
			}
		};

		AssetDatabase.ImportAsset(assetPath);
		AssetDatabase.Refresh();

		foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath (assetPath)) {
			Debug.Log (obj);
		}
	}
}