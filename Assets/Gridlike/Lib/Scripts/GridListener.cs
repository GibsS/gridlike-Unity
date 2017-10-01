using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridListener {

	void OnTileChange(int x, int y);
	void OnRegionChange(int regionX, int regionY);

}

public abstract class GridListener : MonoBehaviour, IGridListener {
	
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
		
	public abstract void OnTileChange(int x, int y);
	public abstract void OnRegionChange(int regionX, int regionY);
}
