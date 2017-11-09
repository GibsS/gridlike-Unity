using System;
using UnityEngine;

namespace Gridlike {

	[Serializable]
	public class TileExtra {

		public int x;
		public int y;
		public TileDictionary dictionary;
		public string name;
	}

	[Serializable]
	public class Tile {

		[NonSerialized] public TileDictionary dictionary;
		[NonSerialized] public string name;

		public bool tileGOCenter;

		public int id;
		public int subId;

		public float state1;
		public float state2;
		public float state3;

		public Tile Clone() {
			return new Tile {
				dictionary = dictionary.Clone (),

				tileGOCenter = tileGOCenter,

				id = id,
				subId = subId,

				state1 = state1,
				state2 = state2,
				state3 = state3
			};
		}

		public void ApplyExtra(TileExtra extra) {
			dictionary = extra.dictionary;
			name = extra.name;
		}
		public TileExtra GetExtra() {
			return new TileExtra {
				dictionary = dictionary,
				name = name
			};
		}
	}
}