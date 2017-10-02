using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridListener : MonoBehaviour {
	
	public Grid grid;

	void Reset() {
		ResetGrid ();
	}
	void Awake() {
		ResetGrid ();
	}

	void OnDestroy() {
		if (this.grid != null) {
			this.grid.RemoveListener (this);
		}
	} 

	public virtual void ResetGrid() {
		if (this.grid == null) {
			Grid grid = GetComponent<Grid> ();

			if (grid == null) {
				// TODO better warning (in inspector)
				Debug.LogWarning ("No Grid on this game object");
			} else {
				this.grid = grid;

				grid.AddListener (this);
			}
		}
	}
		
	public abstract void OnSet(int x, int y, Tile tile);
	public virtual void OnSetState (int x, int y, Tile tile, float oldState1, float oldState2, float oldState3) {
		OnSet (x, y, tile);
	}
	public virtual void OnSetId (int x, int y, Tile tile, int oldId, int oldSubId) {
		OnSet (x, y, tile);
	}
	public virtual void OnSetShape (int x, int y, Tile tile, TileShape oldShape) {
		OnSet (x, y, tile);
	}

	public abstract void OnRegionChange(int regionX, int regionY);

	public abstract void OnTileSizeChange ();
}
