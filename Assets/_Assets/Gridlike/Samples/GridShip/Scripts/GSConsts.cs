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
				cubeCost = 5,
				HP = 3,
				cubePerHP = 1
			} 
		},
		{ 2, new GSTile { // ground2
				id = 2,
				cubeCost = 5,
				HP = 3,
				cubePerHP = 1
			} 
		},
		{ 3, new GSTile { // ground3
				id = 3,
				cubeCost = 5,
				HP = 3,
				cubePerHP = 1
			} 
		},
		{ 4, new GSTile { // ground4
				id = 4,
				cubeCost = 5,
				HP = 3,
				cubePerHP = 1
			} 
		},
		{ 5, new GSTile { // test GSTileBehaviour
				id = 5,
				cubeCost = 5,
				HP = 3,
				cubePerHP = 10
			} 
		}
	};

	public static bool TileExists(int id) {
		return tiles.ContainsKey (id);
	}
}