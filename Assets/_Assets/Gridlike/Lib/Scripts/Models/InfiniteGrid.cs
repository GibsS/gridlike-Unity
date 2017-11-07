using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
		
	[Serializable]
	public class InfiniteGrid {

		[SerializeField] int _regionSize;

		[SerializeField] List<FiniteGrid> regions;

		public InfiniteGrid(int gridSize) {
			_regionSize = gridSize;

			regions = new List<FiniteGrid> ();
		}

		public Tile Get(int x, int y) {
			FiniteGrid region = GetContainingRegion (x, y);

			return region != null ? region.Get (x - region.regionX * _regionSize, y - region.regionY * _regionSize) : null;
		}
		public Tile Get(int x, int y, out FiniteGrid region) {
			region = GetContainingRegion (x, y);

			return region != null ? region.Get (x - region.regionX * _regionSize, y - region.regionY * _regionSize) : null;
		}
		public FiniteGrid Set(int x, int y, Tile value) {
			FiniteGrid region = GetContainingRegion (x, y);

			if (region == null) {
				int X = Mathf.FloorToInt (((float)x) / _regionSize);
				int Y = Mathf.FloorToInt (((float)y) / _regionSize);

				region = new FiniteGrid (X, Y, _regionSize);

				regions.Add (region);
			} 

			region.Set (x - region.regionX * _regionSize, y - region.regionY * _regionSize, value);

			return region;
		}

		public void SetRegion(int X, int Y, FiniteGrid newRegion) {
			FiniteGrid region = GetRegion (X, Y);

			if (region == null) {
				regions.Add (newRegion);

				region = newRegion;
			}
		}
		public void ClearRegion(int X, int Y) {
			regions.RemoveAll (e => e.regionX == X && e.regionY == Y);
		}

		public FiniteGrid GetRegion(int X, int Y) {
			foreach (FiniteGrid region in regions) {
				if (region.regionX == X && region.regionY == Y) {
					return region;
				}
			}
			return null;
		}
		public FiniteGrid GetOrCreateRegion(int X, int Y) {
			FiniteGrid region = GetRegion (X, Y);

			if (region == null) {
				region = new FiniteGrid (X, Y, _regionSize);

				regions.Add(region);
			}

			return region;
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

		public bool presented;

		public int size {
			get { return _size; }
		}

		public int regionX { get { return _x; } }
		public int regionY { get { return _y; } }

		public int __X { set { _x = value; } }
		public int __Y { set { _y = value; } }

		public FiniteGrid(int x, int y, int size) {
			_FiniteGrid (x, y, size);
		}
		public FiniteGrid(int x, int y, int size, Tile[,] tiles, int xoffset, int yoffset) {
			_FiniteGrid (x, y, size);

			int width = Mathf.Min (xoffset + size, tiles.GetLength (0));
			int height = Mathf.Min (yoffset + size, tiles.GetLength (1));

			for (int i = xoffset; i < width; i++) {
				for (int j = yoffset; j < height; j++) {
					grid [i - xoffset] [j - yoffset] = tiles [i, j];
				}
			}
		}

		void _FiniteGrid(int x, int y, int size) {
			_x = x;
			_y = y;
			_size = size;

			grid = new Array[size];
			for (int i = 0; i < size; i++) {
				grid [i] = new Array(size);
			}
		}

		public Tile GetOrCreate(int x, int y) {
			Tile tile = grid [x] [y];

			if (tile == null) {
				tile = new Tile ();
				grid [x] [y] = tile;
			}

			return tile;
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
}