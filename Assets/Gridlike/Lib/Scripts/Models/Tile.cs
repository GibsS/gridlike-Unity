using System;
using UnityEngine;

namespace Gridlike {

	[Serializable]
	public class Tile {

		public TileDictionary dictionary;

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
	}
}