using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridlike {
	
	[ExecuteInEditMode]
	[RequireComponent(typeof(Grid))]
	public abstract class GridDataDelegate : MonoBehaviour {

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
				Debug.LogError ("A Grid can only have one grid data delegate, destroying " + this.GetType ().Name);
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
					Debug.LogWarning ("No Grid on this game object");
				} else {
					this.grid = grid;

					grid.SetDelegate (this);
				}
			}
		}

		public abstract FiniteGrid LoadTiles (int regionX, int regionY);
		public abstract void SaveTiles (int regionX, int regionY, FiniteGrid tiles);
	}
}