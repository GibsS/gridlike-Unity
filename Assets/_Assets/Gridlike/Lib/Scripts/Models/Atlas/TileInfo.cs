using System;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Stores information on a tile type.
	/// </summary>
	[Serializable]
	public class TileInfo {

		/// <summary>
		/// Atlas wide unique id of the tile type.
		/// </summary>
		public int id;
		/// <summary>
		/// Displayed name of the tile.
		/// </summary>
		public string name;

		/// <summary>
		/// The physical shape of the tile.
		/// </summary>
		public TileShape shape;
		/// <summary>
		/// Addendum to the shape field, used for knowing in what direction triangle tiles stretch.
		/// </summary>
		public bool isVertical;

		/// <summary>
		/// Is the collider for that tile is a trigger?
		/// </summary>
		public bool isTrigger;
		/// <summary>
		/// The Unity layer of the collider of the tile.
		/// </summary>
		public int layer;
		/// <summary>
		/// The Unity tag of the collider of the tile.
		/// </summary>
		public string tag;

		/// <summary>
		/// [Not used] Determines whether the tileGO needs to get removed from the grid on initialization
		/// (can be used for creating creature that spawn on specific tiles)
		/// </summary>
		// TODO use as a spawning mechanism
		public bool isGODetached;
		/// <summary>
		/// The prefab of the tileGO. If null, the tile is a regular tile. If not null, overwrite shape and display fields.
		/// </summary>
		public GameObject tileGO;

		/// <summary>
		/// Sprites for the main subId (0)
		/// </summary>
		public TileSpriteInfo idSpriteInfo;
		/// <summary>
		/// Sprites for the all non main subId (1 -> subIdSriteInfo.Length). It's size determines the number of subId.
		/// </summary>
		public TileSpriteInfo[] subIdSpriteInfo;

		/// <summary>
		/// Gets the sprite for this tile for a given subId and size (size is used for stretched triangles). Nonsensical if the tile is
		/// a tileGO.
		/// </summary>
		public Sprite GetSprite(int subId = 0, int size = 1) {
			TileSpriteInfo info;

			if (subId == 0) {
				info = idSpriteInfo;
			} else {
				if (subIdSpriteInfo [subId - 1] != null) {
					info = subIdSpriteInfo [subId - 1];
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

		/// <summary>
		/// Gets the sprite for this tile for a given subId and size (size is used for stretched triangles). Nonsensical if the tile is
		/// a tileGO. If the specified size doesn't exist, it gets the first non null sprite and returns the size.
		/// </summary>
		public Sprite GetSprite(out int actualSize, int subId = 0, int size = 1) {
			TileSpriteInfo info;

			if (subId == 0) {
				info = idSpriteInfo;
			} else {
				if (subIdSpriteInfo [subId - 1] != null) {
					info = subIdSpriteInfo [subId - 1];
				} else {
					info = idSpriteInfo;
				}
			}

			if (size == 1) {
				actualSize = 1;
				return info.sprite;
			} else if (size - 2 < info.sprites.Length) {
				actualSize = size;
				return info.sprites [size - 2];
			} else {
				actualSize = info.sprites.Length + 1;
				return info.sprites [info.sprites.Length - 1];
			}
		}

		/// <summary>
		/// Adds a subId to the tile.
		/// </summary>
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
		/// <summary>
		/// Removes the last subId of the tile.
		/// </summary>
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

		/// <summary>
		/// For a given subId, adds a sprite slot for another size.
		/// </summary>
		public void AddSpriteSize(int subId) {
			TileSpriteInfo spriteInfo;

			if (subId == 0) {
				if (idSpriteInfo == null) {
					idSpriteInfo = new TileSpriteInfo ();
				}

				spriteInfo = idSpriteInfo;
			} else {
				if (subIdSpriteInfo != null && (subId - 1) < subIdSpriteInfo.Length) {
					spriteInfo = subIdSpriteInfo [subId - 1];
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

		/// <summary>
		/// For a given subId, removes a the sprite slot with the greatest size.
		/// </summary>
		public void RemoveSpriteSize(int subId) {
			TileSpriteInfo spriteInfo = null;

			if (subId == 0) {
				if (idSpriteInfo != null) {
					spriteInfo = idSpriteInfo;
				} else {
					return;
				}
			} else {
				if (subIdSpriteInfo != null && (subId - 1) < subIdSpriteInfo.Length) {
					spriteInfo = subIdSpriteInfo [subId - 1];
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
}