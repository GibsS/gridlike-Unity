using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InfiniteComponentGrid {

	[SerializeField]
	public int _regionSize;

	[SerializeField]
	public List<FiniteComponentGrid> regions;

	public InfiniteComponentGrid(int gridSize) {
		_regionSize = gridSize;

		regions = new List<FiniteComponentGrid> ();
	}

	public Component Get(int x, int y) {
		FiniteComponentGrid region = GetContainingRegion (x, y);

		return region != null ? region.Get (x - region.x * _regionSize, y - region.y * _regionSize) : null;
	}
	public void Set(int x, int y, Component value) {
		FiniteComponentGrid region = GetContainingRegion (x, y);

		if (region == null) {
			int X = Mathf.FloorToInt (((float) x) / _regionSize);
			int Y = Mathf.FloorToInt (((float) y) / _regionSize);

			region = new FiniteComponentGrid (X, Y, _regionSize);

			regions.Add(region);
		}

		region.Set (x - region.x * _regionSize, y - region.y * _regionSize, value);
	}

	public FiniteComponentGrid GetRegion(int X, int Y) {
		foreach (FiniteComponentGrid region in regions) {
			if (region.x == X && region.y == Y) {
				return region;
			}
		}
		return null;
	}
	public FiniteComponentGrid GetContainingRegion(int x, int y) {
		x = Mathf.FloorToInt(((float) x) / _regionSize);
		y = Mathf.FloorToInt (((float) y) / _regionSize);

		return GetRegion (x, y);
	}
	public List<FiniteComponentGrid> GetRegions() {
		return regions;
	}
}

[Serializable]
public class FiniteComponentGrid {

	// region space, not tile space
	[SerializeField]
	int _x;
	[SerializeField]
	int _y;
	[SerializeField]
	int _size;

	[SerializeField]
	ComponentArray[] grid;

	public int size {
		get { return _size; }
	}

	public int x { get { return _x; } }
	public int y { get { return _y; } }

	public FiniteComponentGrid(int x, int y, int size) {
		_x = x;
		_y = y;
		_size = size;

		grid = new ComponentArray[size];
		for (int i = 0; i < size; i++) {
			grid [i] = new ComponentArray(size);
		}
	}

	public Component Get(int x, int y) {
		return grid [x] [y];
	}
	public void Set(int x, int y, Component value) {
		grid [x] [y] = value;
	}
}

[Serializable]
public class ComponentArray {

	public Component[] array;

	public Component this[int i] {
		get { return array[i]; }
		set { array[i] = value; }
	}

	public ComponentArray(int size) {
		array = new Component[size];	
	}
}