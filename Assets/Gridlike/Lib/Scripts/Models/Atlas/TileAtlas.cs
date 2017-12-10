using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Custom asset that stores a collection of tile types. Every grid needs to have a reference to such an atlas.
	/// </summary>
	[CreateAssetMenu(fileName="GridTileAtlas", menuName="Gridlike/Grid tile atlas", order=1)]
	public class TileAtlas : ScriptableObject {

		/// <summary>
		/// The max pixel size (width and height) of a sprite.
		/// </summary>
		public const int MAX_SPRITE_SHEET_SIZE = 2048;

		/// <summary>
		/// Pixel size of a tile
		/// </summary>
		public int tilePixelSize;
		/// <summary>
		/// Are generated files placed in a path relative to the tileAtlas (true) or in a path relative to the project root (false) ?
		/// </summary>
		public bool useRelativePath = false;
		/// <summary>
		/// The path to the generated sprite sheet.
		/// </summary>
		public string spriteSheetPath;
		/// <summary>
		/// The path to the generated helper script.
		/// </summary>
		public string scriptPath;

		/// <summary>
		/// The generated sprite sheet.
		/// </summary>
		public Texture2D spriteSheet;
		/// <summary>
		/// The empty sprite in the sprite sheet.
		/// </summary>
		public Sprite emptySprite;
		/// <summary>
		/// Major version of the atlas when the sprite sheet was last updated.
		/// </summary>
		public int spriteSheetMajorVersion;
		/// <summary>
		/// Minor version of the atlas when the sprite sheet was last updated.
		/// </summary>
		public int spriteSheetMinorVersion;

		/// <summary>
		/// Tile types.
		/// </summary>
		public TileInfo[] atlas;

		/// <summary>
		/// Major version of the atlas. Entities that depend on this atlas also have a "version at last update" they can use to know if their data is outdated or not.
		/// </summary>
		public int majorVersion;
		/// <summary>
		/// Minor version of the atlas.
		/// </summary>
		public int minorVersion;

		/// <summary>
		/// The material used by GridRenderers
		/// </summary>
		public Material material;

		/// <summary>
		/// The generated helper.
		/// </summary>
		public TileAtlasHelper helper;
		/// <summary>
		/// Major version of the atlas when the helper was last generated.
		/// </summary>
		public int helperMajorVersion;
		/// <summary>
		/// Minor version of the atlas when the helper was last generated.
		/// </summary>
		public int helperMinorVersion;

		/// <summary>
		/// Number of tile types.
		/// </summary>
		public int Count { 
			get {
				int count = 0;

				for (int i = 0; i < atlas.Length; i++) {
					if (atlas [i].id != 0) count++;
				}

				return count;
			}
		}

		/// <summary>
		/// The number of sprite sheet "tile" every sprite in the atlas would occupy
		/// </summary>
		/// <value>The total sprite tile count.</value>
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
		/// <summary>
		/// Number of sprite in the entire atlas.
		/// </summary>
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

			string path = Path.GetFileNameWithoutExtension (scriptPath);

			if (!string.IsNullOrEmpty (path)) {
				Type type = Type.GetType (path);

				if (type != null) {
					helper = Activator.CreateInstance (type) as TileAtlasHelper;
					helper._Inject (this);
				}
			}
		}

		public IEnumerable<TileInfo> GetTileInfos() {
			for (int i = 0; i < atlas.Length; i++) {
				if (atlas [i] != null && atlas[i].id != 0) {
					yield return atlas [i];
				}
			}
		}

		/// <summary>
		/// Gets tile for the given id.
		/// </summary>
		/// <param name="id">id.</param>
		public TileInfo GetTile(int id) {
			return atlas [id];
		}

		/// <summary>
		/// Get tile with the id equal to i
		/// </summary>
		/// <param name="i">The index.</param>
		public TileInfo this[int i] {
			get { return atlas[i]; }
			set { atlas[i] = value; }
		}

		/// <summary>
		/// Checks whether or not the atlas contains the given id.
		/// </summary>
		/// <param name="id">id.</param>
		public bool ContainsTile(int id) {
			return id >= 0 && id < atlas.Length && atlas [id] != null && atlas [id].id != 0;
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
		/// <summary>
		/// Removes a tile type
		/// </summary>
		/// <param name="id">id.</param>
		public void RemoveTile(int id) {
			if (id < atlas.Length && id >= 0) {
				atlas [id] = null;
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

		public void BumpMajor() {
			majorVersion++;
		}
		public void BumpMinor() {
			minorVersion++;
		}
	}
}