using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(GridGenerator))]
public abstract class GridGeneratorAlgorithm : MonoBehaviour {
	
	void Reset() {
		ResetAlgorithm ();
	}
	void Awake() {
		ResetAlgorithm ();
	}

	public virtual void ResetAlgorithm() {
		GridGenerator generator = GetComponent<GridGenerator> ();
		generator.SetAlgorithm (this);
	}

	public abstract Tile[,] GenerateTiles (int x, int y, int width, int height);
}