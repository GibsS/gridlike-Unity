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
		for (int i = 1; i < atlas.Length; i++) {
			if (atlas [i].id == 0) {
				TileInfo tile = CreateTileInfo (i);
				atlas [i] = tile;
				return i;
			}
		}

		// if full, expand and add tile info at the end
		TileInfo[] newAtlas = new TileInfo[atlas.Length + 10];
		for (int i = 1; i < atlas.Length; i++) {
			newAtlas [i] = atlas [i];
		}

		int newPosition = atlas.Length;
		newAtlas [newPosition] = CreateTileInfo (newPosition);

		atlas = newAtlas;

		return newPosition;
	}
	public void RemoveTile(int tile) {
		if (tile < atlas.Length && tile >= 0) {
			atlas [tile] = null;
		}
	}

	TileInfo CreateTileInfo(int id) {
		TileInfo tile = new TileInfo();

		tile.id = id;
		tile.name = "tile " + id;
		tile.defaultShape = TileShape.FULL;

		tile.idSpriteInfo = null;
		tile.subIdSpriteInfo = null;

		return tile;
	}
}