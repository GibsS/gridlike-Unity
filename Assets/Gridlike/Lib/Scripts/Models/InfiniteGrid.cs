using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InfiniteGrid {

	[SerializeField]
	public int _regionSize;

	[SerializeField]
	public List<FiniteGrid> regions;

	public InfiniteGrid(int gridSize) {
		_regionSize = gridSize;

		regions = new List<FiniteGrid> ();
	}

	public Tile Get(int x, int y) {
		FiniteGrid region = GetContainingRegion (x, y);

		return region != null ? region.Get (x - region.x * _regionSize, y - region.y * _regionSize) : null;
	}
	public void Set(int x, int y, Tile value) {
		FiniteGrid region = GetContainingRegion (x, y);

		if (region == null) {
			int X = Mathf.FloorToInt (((float)x) / _regionSize);
			int Y = Mathf.FloorToInt (((float)y) / _regionSize);

			region = new FiniteGrid (X, Y, _regionSize);

			regions.Add (region);
		} 

		region.Set (x - region.x * _regionSize, y - region.y * _regionSize, value);
	}

	public FiniteGrid GetRegion(int X, int Y) {
		foreach (FiniteGrid region in regions) {
			if (region.x == X && region.y == Y) {
				return region;
			}
		}
		return null;
	}
	public FiniteGrid GetContainingRegion(int x, int y) {
		x = Mathf.FloorToInt(((float) x) / _regionSize);
		y = Mathf.FloorToInt (((float) y) / _regionSize);

		return GetRegion (x, y);
	}
	public List<FiniteGrid> GetRegions() {
		return regions;
	}
}

[Serializable]
public class FiniteGrid {

	// region space, not tile space
	[SerializeField]
	int _x;
	[SerializeField]
	int _y;
	[SerializeField]
	int _size;

	[SerializeField]
	Array[] grid;

	public bool presented = true;

	public int size {
		get { return _size; }
	}

	public int x { get { return _x; } }
	public int y { get { return _y; } }

	public FiniteGrid(int x, int y, int size) {
		_x = x;
		_y = y;
		_size = size;

		grid = new Array[size];
		for (int i = 0; i < size; i++) {
			grid [i] = new Array(size);
		}
	}

	public Tile Get(int x, int y) {
		return grid [x] [y];
	}
	public void Set(int x, int y, Tile value) {
		grid [x] [y] = value;
	}
}

[Serializable]
public class Array {

	public Tile[] array;

	public Tile this[int i] {
		get { return array[i]; }
		set { array[i] = value; }
	}

	public Array(int size) {
		array = new Tile[size];	
	}
}