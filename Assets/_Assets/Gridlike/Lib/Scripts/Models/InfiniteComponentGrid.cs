using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Represents an infinite two dimensional array of Unity Components. 
	/// (Identical to InfiniteGrid, couldn't figure out how to factor and remain serializable).
	/// </summary>
	[Serializable]
	public class InfiniteComponentGrid {

		/// <summary>
		/// The size of a region. An InfiniteComponentGrid is a collection of "regions" (_regionSize x _regionSize two dimensional arrays of Components).
		/// </summary>
		[SerializeField] public int _regionSize;

		/// <summary>
		/// The infinite grid's regions.
		/// </summary>
		[SerializeField] public List<FiniteComponentGrid> regions;

		public InfiniteComponentGrid(int gridSize) {
			_regionSize = gridSize;

			regions = new List<FiniteComponentGrid> ();
		}

		/// <summary>
		/// Gets the component at the specified coordinates.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		public Component Get(int x, int y) {
			FiniteComponentGrid region = GetContainingRegion (x, y);

			return region != null ? region.Get (x - region.x * _regionSize, y - region.y * _regionSize) : null;
		}
		/// <summary>
		/// Sets the component at the specified coordinates.
		/// </summary>
		/// <param name="x">The x tile coordinate.</param>
		/// <param name="y">The y tile coordinate.</param>
		/// <param name="value">The new value.</param>
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

		/// <summary>
		/// Removes the specified region.
		/// </summary>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		public void ClearRegion(int X, int Y) {
			regions.RemoveAll (e => e.x == X && e.y == Y);
		}

		/// <summary>
		/// Gets the specified region. null if it doesn't exist.
		/// </summary>
		/// <returns>The region.</returns>
		/// <param name="X">The X region coordinate.</param>
		/// <param name="Y">The Y region coordinate.</param>
		public FiniteComponentGrid GetRegion(int X, int Y) {
			foreach (FiniteComponentGrid region in regions) {
				if (region.x == X && region.y == Y) {
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
		public FiniteComponentGrid GetOrCreateRegion(int X, int Y) {
			FiniteComponentGrid region = GetRegion (X, Y);

			if (region == null) {
				region = new FiniteComponentGrid (X, Y, _regionSize);

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
		public FiniteComponentGrid GetContainingRegion(int x, int y) {
			x = Mathf.FloorToInt(((float) x) / _regionSize);
			y = Mathf.FloorToInt (((float) y) / _regionSize);

			return GetRegion (x, y);
		}
		/// <summary>
		/// returns every regions.
		/// </summary>
		/// <returns>The regions.</returns>
		public IEnumerable<FiniteComponentGrid> GetRegions() {
			return regions;
		}
	}

	/// <summary>
	/// A simple two dimensional array used as a "region" of components in the rest of the library. Is always refered to as a region.
	/// </summary>
	[Serializable]
	public class FiniteComponentGrid {

		/// <summary>
		/// X coordinate of the region in "region space" (not tile space)
		/// </summary>
		[SerializeField] int _x;
		/// <summary>
		/// Y coordinate of the region 
		/// </summary>
		[SerializeField] int _y;
		/// <summary>
		/// Width and height of the two dimensional array
		/// </summary>
		[SerializeField] int _size;

		/// <summary>
		/// Associated grid.
		/// </summary>
		[SerializeField] ComponentArray[] grid;

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
}