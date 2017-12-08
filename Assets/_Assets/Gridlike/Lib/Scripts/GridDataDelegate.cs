using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Handles loading and saving of a grids data. A Grid can theoretically infinite but RAM isn't. Grid Data Delegate takes care of storing
	/// currently unloaded data of the Grid.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Grid))]
	[DisallowMultipleComponent]
	public abstract class GridDataDelegate : MonoBehaviour {

		/// <summary>
		/// The Grid the GridDataDelegate is attached to
		/// </summary>
		/// <value>The grid.</value>
		[SerializeField] public Grid grid { get; private set; }

		protected bool gettingDestroyed;

		void Reset() {
			ResetDelegate ();
		}
		void Awake() {
			ResetDelegate ();
		}

		public virtual void ResetDelegate() {
			GridDataDelegate[] all = GetComponents<GridDataDelegate>();

			if (all.Length > 1) {
				Debug.LogError ("[Gridlike] A Grid GameObject can only have one grid data delegate, destroying " + this.GetType ().Name);
				if (Application.isEditor)
					DestroyImmediate (this);
				else
					Destroy (this);
				
				gettingDestroyed = true;
				return;
			}
				
			if (this.grid == null) {
				Grid grid = GetComponent<Grid> ();

				if (grid == null) {
					Debug.LogWarning ("[Gridlike] No Grid on this game object");
				} else {
					this.grid = grid;

					grid._SetDelegate (this);
				}
			}
		}

		/// <summary>
		/// Asynchronously returns the data for a region through the callback
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="callback">The callback to call once the region is loaded.</param>
		public abstract void LoadTiles (int regionX, int regionY, Action<FiniteGrid> callback);
		/// <summary>
		/// Saves the region data
		/// </summary>
		/// <param name="regionX">The X coordinate of the region.</param>
		/// <param name="regionY">The Y coordinate of the region.</param>
		/// <param name="tiles">The data for the region.</param>
		public abstract void SaveTiles (int regionX, int regionY, FiniteGrid tiles);
	}
}