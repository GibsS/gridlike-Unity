using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="GridTileAtlas", menuName="Gridlike/Grid tile atlas", order=1)]
public class TileAtlas : ScriptableObject {

	public TileInfo[] atlas;

	public int Count { 
		get {
			int count = 0;

			for (int i = 0; i < atlas.Length; i++) {
				if (atlas [i].id != 0) count++;
			}

			return count;
		}
	}

	void OnEnable() {
		// TODO increase size once tested
		if (atlas == null) {
			atlas = new TileInfo[5];

			for (int i = 0; i < atlas.Length; i++) {
				atlas [i] = new TileInfo ();
			}
		}
	}

	public IEnumerable<TileInfo> GetTilesInfo() {
		for (int i = 0; i < atlas.Length; i++) {
			if (atlas [i] != null) {
				yield return atlas [i];
			}
		}
	}

	public TileInfo GetTile(int id) {
		return atlas [id];
	}

	public int AddTile() {
		for (int i = 0; i < atlas.Length; i++) {
			if (atlas [i] == null) {
				atlas [i] = new TileInfo ();
			}
		}

		for (int i = 1; i < atlas.Length; i++) {
			if (atlas [i].id == 0) {
				atlas [i] = CreateTileInfo (i);
				return i;
			}
		}

		// if full, expand and add tile info at the end
		TileInfo[] newAtlas = new TileInfo[atlas.Length + 10];
		for (int i = 1; i < atlas.Length; i++) {
			newAtlas [i] = atlas [i];
		}

		int newPosition = Mathf.Max(1, atlas.Length);
		newAtlas [newPosition] = CreateTileInfo (newPosition);

		atlas = newAtlas;

		return newPosition;
	}
	public void RemoveTile(int tile) {
		if (tile < atlas.Length && tile >= 0) {
			atlas [tile] = null;
		}
	}

	public Sprite GetSprite(int id, int subId, int size = 1) {
		return atlas [id].GetSprite (subId, size);
	}

	TileInfo CreateTileInfo(int id) {
		TileInfo tile = new TileInfo();

		tile.id = id;
		tile.name = "tile " + id;
		tile.shape = TileShape.FULL;
		tile.tag = "Untagged";

		tile.idSpriteInfo = new TileSpriteInfo();
		tile.subIdSpriteInfo = null;

		return tile;
	}
}