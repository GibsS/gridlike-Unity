using System.Collections;
using System.Collections.Generic;
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
				tile.idSpriteInfo.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tile.idSpriteInfo.sprite, typeof(Sprite), false);
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
}