using System;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Used for serialization and optimizing space usage.
	/// </summary>
	[Serializable]
	public class TileExtra {

		/// <summary>
		/// The x tile coordinate this data is associated to.
		/// </summary>
		public int x;
		/// <summary>
		/// The y tile coordinate this data is associated to.
		/// </summary>
		public int y;
		/// <summary>
		/// The tile dictionary.
		/// </summary>
		public TileDictionary dictionary;
		/// <summary>
		/// The tile name.
		/// </summary>
		public string name;
	}

	[Serializable]
	public class Tile {

		/// <summary>
		/// The dictionary. Used only by tileGO's. Used to store more complex data.
		/// </summary>
		[NonSerialized] public TileDictionary dictionary;
		/// <summary>
		/// The name. Used only by tileGO's.
		/// </summary>
		// TODO make sure the name can't be specified on none tileGO in the grid editor.
		[NonSerialized] public string name;

		/// <summary>
		/// True if this tile is the center of a tileGO.
		/// </summary>
		public bool tileGOCenter;

		/// <summary>
		/// The tile type id of the tile.
		/// </summary>
		public int id;
		/// <summary>
		/// The subId of the tile.
		/// </summary>
		public int subId;

		public float state1;
		public float state2;
		public float state3;

		/// <summary>
		/// Creates a deep copy of this tile.
		/// </summary>
		public Tile Clone() {
			return new Tile {
				dictionary = dictionary.Clone (),
				name = name,

				tileGOCenter = tileGOCenter,

				id = id,
				subId = subId,

				state1 = state1,
				state2 = state2,
				state3 = state3
			};
		}

		/// <summary>
		/// Load extra information (dictionary and name).
		/// </summary>
		public void ApplyExtra(TileExtra extra) {
			dictionary = extra.dictionary;
			name = extra.name;
		}
		/// <summary>
		/// Save extra information (dictionary and name).
		/// </summary>
		public TileExtra GetExtra() {
			return new TileExtra {
				dictionary = dictionary,
				name = name
			};
		}
	}
}