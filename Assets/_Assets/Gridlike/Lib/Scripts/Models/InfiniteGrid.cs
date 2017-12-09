using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Represents an infinite two dimensional array of Tiles. 
	/// (Identical to InfiniteComponentGrid, couldn't figure out how to factor and remain serializable).
	/// </summary>
	[Serializable]
	public class InfiniteGrid {

		/// <summary>
		/// The size of a region. An InfiniteGrid is a collection of "regions" (_regionSize x _regionSize two dimensional arrays of Tiles).
		/// </summary>
		[SerializeField] int _regionSize;

		/// <summary>
		/// The infinite grid's regions.
		/// </summary>
		[SerializeField] List<FiniteGrid> regions;

		public InfiniteGrid(int gridSize) {
			_regionSize = gridSize;

			regions = new List<FiniteGrid> ();
		}

		/// <summary>
		/// Gets the tile at the specified coordinates.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Tile Get(int x, int y) {
			FiniteGrid region = GetContainingRegion (x, y);

			return region != null ? region.Get (x - region.regionX * _regionSize, y - region.regionY * _regionSize) : null;
		}
		/// <summary>
		/// Gets the tile at the specified coordinates and the containing region.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Tile Get(int x, int y, out FiniteGrid region) {
			region = GetContainingRegion (x, y);

			return region != null ? region.Get (x - region.regionX * _regionSize, y - region.regionY * _regionSize) : null;
		}
		/// <summary>
		/// Sets the tile at the specified coordinates.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		/// <param name="value">The new value.</param>
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

		/// <summary>
		/// Sets the tile information for an entire region. Reuses provided FiniteGrid, do not use after this point.
		/// </summary>
		/// <param name="X">The X coordinate of the region.</param>
		/// <param name="Y">The Y coordinate of the region.</param>
		public void SetRegion(int X, int Y, FiniteGrid newRegion) {
			FiniteGrid region = GetRegion (X, Y);

			if (region == null) {
				regions.Add (newRegion);

				region = newRegion;
			}
		}
		/// <summary>
		/// Clears the region at the specified location.
		/// </summary>
		/// <param name="X">The X coordinate of the region.</param>
		/// <param name="Y">The Y coordinate of the region.</param>
		public void ClearRegion(int X, int Y) {
			regions.RemoveAll (e => e.regionX == X && e.regionY == Y);
		}

		/// <summary>
		/// Gets the specified region. null if it doesn't exist.
		/// </summary>
		/// <returns>The region.</returns>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		public FiniteGrid GetRegion(int X, int Y) {
			foreach (FiniteGrid region in regions) {
				if (region.regionX == X && region.regionY == Y) {
					return region;
				}
			}
			return null;
		}
		/// <summary>
		/// Gets the specified region and creates it if there is none yet.
		/// </summary>
		/// <returns>The or create region.</returns>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		public FiniteGrid GetOrCreateRegion(int X, int Y) {
			FiniteGrid region = GetRegion (X, Y);

			if (region == null) {
				region = new FiniteGrid (X, Y, _regionSize);

				regions.Add(region);
			}

			return region;
		}
		/// <summary>
		/// Gets the region that contains the given tile coordinates. Null if none exist.
		/// </summary>
		/// <returns>The containing region.</returns>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public FiniteGrid GetContainingRegion(int x, int y) {
			x = Mathf.FloorToInt(((float) x) / _regionSize);
			y = Mathf.FloorToInt (((float) y) / _regionSize);

			return GetRegion (x, y);
		}
		/// <summary>
		/// returns every regions.
		/// </summary>
		/// <returns>The regions.</returns>
		public List<FiniteGrid> GetRegions() {
			return regions;
		}
	}

	/// <summary>
	/// A simple two dimensional array used as a "region" of tiles in the rest of the library. Is always refered to as a region.
	/// </summary>
	[Serializable]
	public class FiniteGrid {

		/// <summary>
		/// When serializing, storing a dictionnary and name string for every tile is very costly in space. So before serialization,
		/// This information is stored in this region wide list. At deserialization, this information is fetched again.
		/// </summary>
		[SerializeField] List<TileExtra> extras;

		// region space, not tile space
		[SerializeField] int _x;
		[SerializeField] int _y;
		[SerializeField] int _size;

		[SerializeField] Array[] grid;

		public bool presented;

		/// <summary>
		/// Gets the width and height of the region
		/// </summary>
		public int size {
			get { return _size; }
		}

		/// <value>the X coordinate of the region.</value>
		public int regionX { get { return _x; } }
		/// <value>the Y coordinate of the region.</value>
		public int regionY { get { return _y; } }

		public int __X { set { _x = value; } }
		public int __Y { set { _y = value; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Gridlike.FiniteGrid"/> class with the region position and size.
		/// </summary>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		/// <param name="size">The region size.</param>
		public FiniteGrid(int X, int Y, int size) {
			_FiniteGrid (X, Y, size);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Gridlike.FiniteGrid"/> class with the region position and size.
		/// </summary>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		/// <param name="size">The region size.</param>
		/// <param name="tiles">A tile two dimensional array.</param>
		/// <param name="xoffset">The bottom left x coordinates in "tiles" of the current region.</param>
		/// <param name="yoffset">The bottom left y coordinates in "tiles" of the current region.</param>
		public FiniteGrid(int X, int Y, int size, Tile[,] tiles, int xoffset, int yoffset) {
			_FiniteGrid (X, Y, size);

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

		/// <summary>
		/// Gets the tile at the specified location or creates it if it doesn't exist.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Tile GetOrCreate(int x, int y) {
			Tile tile = grid [x] [y];

			if (tile == null) {
				tile = new Tile ();
				grid [x] [y] = tile;
			}

			return tile;
		}
		/// <summary>
		/// Gets the tile at the specified location.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Tile Get(int x, int y) {
			return grid [x] [y];
		}
		/// <summary>
		/// Sets the tile at the specified location.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		/// <param name="value">The new tile value.</param>
		public void Set(int x, int y, Tile value) {
			grid [x] [y] = value;
		}

		/// <summary>
		/// TileDictionnaries and names are serialized in a region wide list, not directly in the list. 
		/// So when deserializing, LoadExtra needs to be called to load them in the correct tiles.
		/// </summary>
		public void LoadExtra() {
			if (extras != null) {
				foreach (TileExtra extra in extras) {
					Tile tile = Get (extra.x, extra.y);

					if (tile != null) {
						tile.ApplyExtra (extra);
					}
				}

				extras = null;
			}
		}
		/// <summary>
		/// TileDictionnaries and names are serialized in a region wide list, not directly in the list. 
		/// So when deserializing, SaveExtra needs to be called to save for later use.
		/// </summary>
		public void SaveExtra() {
			extras = new List<TileExtra> ();

			for (int i = 0; i < _size; i++) {
				for (int j = 0; j < _size; j++) {
					Tile tile = Get (i, j);

					if (tile != null && (tile.dictionary != null || !string.IsNullOrEmpty (tile.name))) {
						TileExtra extra = tile.GetExtra ();
						extra.x = i;
						extra.y = j;
						extras.Add (extra);
					}
				}
			}
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