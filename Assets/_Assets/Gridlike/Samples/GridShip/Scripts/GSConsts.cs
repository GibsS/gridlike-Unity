using UnityEngine;
using System.Collections.Generic;

public class GSTile {

	public int id;
	public int cubeCost;
	public int HP;
	public float cubePerHP;
}

public static class GSConsts {

	public static Dictionary<int, GSTile> tiles = new Dictionary<int, GSTile> { 
		{ 1, new GSTile { // ground1
				id = 1,
				cubeCost = 6,
				HP = 5,
				cubePerHP = 1
			} 
		},
		{ 2, new GSTile { // ground2
				id = 2,
				cubeCost = 15,
				HP = 12,
				cubePerHP = 1
			} 
		},
		{ 3, new GSTile { // ground3
				id = 3,
				cubeCost = 60,
				HP = 50,
				cubePerHP = 1
			} 
		},
		{ 4, new GSTile { // ground4
				id = 4,
				cubeCost = 220,
				HP = 200,
				cubePerHP = 1
			} 
		},
		{ 5, new GSTile { // ore1
				id = 5,
				cubeCost = 1000,
				HP = 3,
				cubePerHP = 10
			} 
		},
		{ 6, new GSTile { // ore2
				id = 6,
				cubeCost = 1000,
				HP = 5,
				cubePerHP = 15
			} 
		},
		{ 7, new GSTile { // ore3
				id = 7,
				cubeCost = 1000,
				HP = 7,
				cubePerHP = 15
			} 
		},
		{ 8, new GSTile { // ore4
				id = 8,
				cubeCost = 1000,
				HP = 9,
				cubePerHP = 20
			} 
		},
		{ 12, new GSTile { // engine
				id = 12,
				cubeCost = 200,
				HP = 50,
				cubePerHP = 1
			} 
		},
		{ 13, new GSTile { // anti gravity
				id = 13,
				cubeCost = 500,
				HP = 50,
				cubePerHP = 1
			} 
		}
	};

	public static bool TileExists(int id) {
		return tiles.ContainsKey (id);
	}
}