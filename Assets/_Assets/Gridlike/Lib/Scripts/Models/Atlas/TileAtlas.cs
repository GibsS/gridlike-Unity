using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	[CreateAssetMenu(fileName="GridTileAtlas", menuName="Gridlike/Grid tile atlas", order=1)]
	public class TileAtlas : ScriptableObject {

		public const int MAX_SPRITE_SHEET_SIZE = 2048;

		public int tilePixelSize;
		public bool useRelativePath = false;
		public string spriteSheetPath;
		public string scriptPath;

		public Texture2D spriteSheet;
		public Sprite emptySprite;

		public TileInfo[] atlas;

		public Material material;

		public TileAtlasHelper helper;

		public int Count { 
			get {
				int count = 0;

				for (int i = 0; i < atlas.Length; i++) {
					if (atlas [i].id != 0) count++;
				}

				return count;
			}
		}

		public int TotalSpriteTileCount {
			get {
				int count = 0;

				foreach (TileInfo info in GetTileInfos()) {
					count += TileTextureCountInTileSpriteInfo (info.idSpriteInfo);

					if (info.subIdSpriteInfo != null) {
						for (int i = 0; i < info.subIdSpriteInfo.Length; i++) {
							count += TileTextureCountInTileSpriteInfo (info.subIdSpriteInfo [i]);
						}
					}
				}

				return count;
			}
		}
		int TileTextureCountInTileSpriteInfo(TileSpriteInfo info) {
			int count = 0;

			if (info != null) {
				if (info.importedSprite != null) {
					count++;
				}

				if (info.importedSprites != null) {
					for (int i = 0; i < info.importedSprites.Length; i++) {
						count += i + 2;
					}
				}
			}

			return count;
		}
		public int SpriteCount {
			get {
				int count = 0;

				foreach (TileInfo info in GetTileInfos()) {
					count += TileSpriteCountInTileSpriteInfo (info.idSpriteInfo);

					if (info.subIdSpriteInfo != null) {
						for (int i = 0; i < info.subIdSpriteInfo.Length; i++) {
							count += TileSpriteCountInTileSpriteInfo (info.subIdSpriteInfo [i]);
						}
					}
				}

				return count;
			}
		}
		int TileSpriteCountInTileSpriteInfo(TileSpriteInfo info) {
			int count = 0;

			if (info != null) {
				if (info.importedSprite != null) {
					count++;
				}

				if (info.importedSprites != null) {
					count += info.importedSprites.Length;
				}
			}

			return count;
		}

		void OnEnable() {
			if (atlas == null) {
				atlas = new TileInfo[30];

				for (int i = 0; i < atlas.Length; i++) {
					atlas [i] = new TileInfo ();
				}

			}

			Type type = Type.GetType (Path.GetFileNameWithoutExtension (scriptPath));

			if (type != null) {
				helper = Activator.CreateInstance (type) as TileAtlasHelper;
				helper._Inject (this);
			}
		}

		public IEnumerable<TileInfo> GetTileInfos() {
			for (int i = 0; i < atlas.Length; i++) {
				if (atlas [i] != null && atlas[i].id != 0) {
					yield return atlas [i];
				}
			}
		}

		public TileInfo GetTile(int id) {
			return atlas [id];
		}

		public TileInfo this[int i] {
			get { return atlas[i]; }
			set { atlas[i] = value; }
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

		public void RegenerateMaterial() {
			if (material == null) {
				material = new Material (Shader.Find ("Sprites/Default"));
				material.mainTexture = spriteSheet;
			}
		}
	}
}