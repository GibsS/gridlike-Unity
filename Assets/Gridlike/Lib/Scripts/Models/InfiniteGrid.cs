using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InfiniteGrid<X> where X : class, new() {

	public class XFiniteGrid : FiniteGrid<X> { 

		public XFiniteGrid(int x, int y, int size) : base(x, y, size) { }
	}

	[SerializeField]
	public int _regionSize;

	[SerializeField]
	public List<XFiniteGrid> regions;

	public InfiniteGrid(int gridSize) {
		_regionSize = gridSize;

		regions = new List<XFiniteGrid> ();
	}

	public X Get(int x, int y) {
		XFiniteGrid region = GetContainingRegion (x, y);

		return region != null ? region.Get (x - region.x * _regionSize, y - region.y * _regionSize) : null;
	}
	public void Set(int x, int y, X value) {
		XFiniteGrid region = GetContainingRegion (x, y);

		if (region == null) {
			int X = Mathf.FloorToInt (((float) x) / _regionSize);
			int Y = Mathf.FloorToInt (((float) y) / _regionSize);

			region = new XFiniteGrid (X, Y, _regionSize);

			regions.Add(region);
		}

		region.Set (x - region.x * _regionSize, y - region.y * _regionSize, value);
	}

	public XFiniteGrid GetRegion(int X, int Y) {
		foreach (XFiniteGrid region in regions) {
			if (region.x == X && region.y == Y) {
				return region;
			}
		}
		return null;
	}
	public XFiniteGrid GetContainingRegion(int x, int y) {
		x = Mathf.FloorToInt(((float) x) / _regionSize);
		y = Mathf.FloorToInt (((float) y) / _regionSize);

		return GetRegion (x, y);
	}
	public List<XFiniteGrid> GetRegions() {
		return regions;
	}
}

[Serializable]
public class FiniteGrid<X> where X : class, new() {

	public class XArray : Array<X> {

		public XArray(int size) : base(size) { }
	}

	// region space, not tile space
	[SerializeField]
	int _x;
	[SerializeField]
	int _y;
	[SerializeField]
	int _size;

	[SerializeField]
	XArray[] grid;

	public int size {
		get { return _size; }
	}

	public int x { get { return _x; } }
	public int y { get { return _y; } }

	public FiniteGrid(int x, int y, int size) {
		_x = x;
		_y = y;
		_size = size;

		grid = new XArray[size];
		for (int i = 0; i < size; i++) {
			grid [i] = new XArray(size);
		}
	}

	public X Get(int x, int y) {
		return grid [x] [y];
	}
	public void Set(int x, int y, X value) {
		grid [x] [y] = value;
	}
}

[Serializable]
public class Array<X> {

	public X[] array;

	public X this[int i] {
		get { return array[i]; }
		set { array[i] = value; }
	}

	public Array(int size) {
		array = new X[size];	
	}
}