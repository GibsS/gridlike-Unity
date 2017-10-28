using System;
using UnityEngine;

[Serializable]
public class TileInfo {

	public int id;
	public string name;

	public TileShape shape;
	public bool isVertical;

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

	public void AddSubId() {
		if (subIdSpriteInfo == null) {
			subIdSpriteInfo = new TileSpriteInfo[1];
		} else {
			int length = subIdSpriteInfo.Length;

			TileSpriteInfo[] newSprites = new TileSpriteInfo[length + 1];
			for (int i = 0; i < length; i++) newSprites [i] = subIdSpriteInfo [i];
			subIdSpriteInfo = newSprites;
		}
	}
	public void RemoveSubId() {
		if (subIdSpriteInfo != null) {
			if (subIdSpriteInfo.Length == 1) {
				subIdSpriteInfo = null; 
			} else {
				int length = subIdSpriteInfo.Length;

				TileSpriteInfo[] newSprites = new TileSpriteInfo[length - 1];
				for (int i = 0; i < length - 1; i++) newSprites [i] = subIdSpriteInfo [i];
				subIdSpriteInfo = newSprites;
			}
		}
	}

	public void AddSpriteSize(int subId) {
		TileSpriteInfo spriteInfo;

		if (subId == -1) {
			if (idSpriteInfo == null) {
				idSpriteInfo = new TileSpriteInfo ();
			}

			spriteInfo = idSpriteInfo;
		} else {
			if (subIdSpriteInfo != null && subId < subIdSpriteInfo.Length) {
				spriteInfo = subIdSpriteInfo [subId];
			} else {
				return;
			}
		}


		if (spriteInfo.importedSprites == null) {
			spriteInfo.importedSprites = new Sprite[1];
		} else {
			Sprite[] sprites = spriteInfo.importedSprites;

			spriteInfo.importedSprites = new Sprite[sprites.Length + 1];
			for (int i = 0; i < sprites.Length; i++) {
				spriteInfo.importedSprites [i] = sprites [i];
			}
		}
	}
	public void RemoveSpriteSize(int subId) {
		TileSpriteInfo spriteInfo = null;

		if (subId == -1) {
			if (idSpriteInfo != null) {
				spriteInfo = idSpriteInfo;
			} else {
				return;
			}
		} else {
			if (subIdSpriteInfo != null && subId < subIdSpriteInfo.Length) {
				spriteInfo = subIdSpriteInfo [subId];
			} else {
				return;
			}
		}

		if (spriteInfo.importedSprites != null) {
			if (spriteInfo.importedSprites.Length == 1) {
				spriteInfo.importedSprites = null;
			} else {
				Sprite[] sprites = spriteInfo.importedSprites;

				spriteInfo.importedSprites = new Sprite[sprites.Length - 1];
				for (int i = 0; i < spriteInfo.importedSprites.Length; i++) {
					spriteInfo.importedSprites [i] = sprites [i];
				}
			}
		}
	}
}