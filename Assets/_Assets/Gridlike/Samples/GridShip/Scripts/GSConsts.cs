using UnityEngine;
using System.Collections.Generic;

public class GSTile {

	public int id;
	public int cubeCost;
	public int HP;
	public float cubePerHP;
}

public static class GSConsts {

	public const int GROUND2 = 2;

	public const int ONE_WAY_PLATFORM = 10;

	public const int ENGINE = 12;
	public const int ANTI_GRAVITY = 13;

	public static Dictionary<int, GSTile> tiles = new Dictionary<int, GSTile> { 
		{ 1, new GSTile { // ground1
				id = 1,
				cubeCost = 6,
				HP = 2,
				cubePerHP = 1
			} 
		},
		{ GROUND2, new GSTile { // ground2
				id = GROUND2,
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
				HP = 2,
				cubePerHP = 3
			} 
		},
		{ 6, new GSTile { // ore2
				id = 6,
				cubeCost = 1000,
				HP = 2,
				cubePerHP = 5
			} 
		},
		{ 7, new GSTile { // ore3
				id = 7,
				cubeCost = 1000,
				HP = 4,
				cubePerHP = 10
			} 
		},
		{ 8, new GSTile { // ore4
				id = 8,
				cubeCost = 1000,
				HP = 5,
				cubePerHP = 15
			} 
		},
		{ ONE_WAY_PLATFORM, new GSTile {
				id = ONE_WAY_PLATFORM,
				cubeCost = 100,
				HP = 10,
				cubePerHP = 1
			}
		},
		{ ENGINE, new GSTile { // engine
				id = ENGINE,
				cubeCost = 200,
				HP = 50,
				cubePerHP = 1
			} 
		},
		{ ANTI_GRAVITY, new GSTile { // anti gravity
				id = ANTI_GRAVITY,
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