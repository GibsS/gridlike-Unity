using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public abstract class GridDataDelegate : MonoBehaviour {

	[SerializeField] public Grid grid { get; private set; }

	void Reset() {
		ResetDelegate ();
	}
	void Awake() {
		ResetDelegate ();
	}

	public virtual void ResetDelegate() {
		if (this.grid == null) {
			Grid grid = GetComponent<Grid> ();

			if (grid == null) {
				// TODO better warning (in inspector)
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