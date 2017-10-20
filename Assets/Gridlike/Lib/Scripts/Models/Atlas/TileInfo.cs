using System;
using UnityEngine;

[Serializable]
public class TileInfo {

	public int id;
	public string name;

	public TileShape shape;
	public bool isSensor;
	public int layer;
	public string tag;

	public bool isGODetached;
	public GameObject tileGO;

	public TileSpriteInfo idSpriteInfo;
	public TileSpriteInfo[] subIdSpriteInfo;

	public Sprite GetSprite(int subId, int size = 1) {
		TileSpriteInfo info;

		if (subId == -1) {
			info = idSpriteInfo;
		} else {
			if (subIdSpriteInfo [subId] != null) {
				info = subIdSpriteInfo [subId];
			} else {
				info = idSpriteInfo;
			}
		}

		if (size == 1) {
			return info.sprite;
		} else {
			return info.sprites [size - 2];
		}
	}
}