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
		{ 1, new GSTile {
				id = 1,
				cubeCost = 5,
				HP = 3,
				cubePerHP = 5
			} 
		}
	};

	public static bool TileExists(int id) {
		return tiles.ContainsKey (id);
	}
}