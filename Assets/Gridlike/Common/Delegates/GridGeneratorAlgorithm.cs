using UnityEngine;
using System.Collections;

namespace Gridlike {

	/// <summary>
	/// Paired with a GridGenerator, specifies the algorithm used for generating tiles.
	/// </summary>
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

			if (generator == null) {
				Debug.LogError ("[Gridlike] Generation algorithm requires a GridGenerator");

				if(Application.isPlaying)
					Destroy (this);
				else 
					DestroyImmediate (this);
				return;
			}

			generator._SetAlgorithm (this);
		}

		/// <summary>
		/// The generation algorithm.
		/// </summary>
		/// <returns>The tiles in the specified area</returns>
		/// <param name="x">The bottom left x coordinate of the generated area.</param>
		/// <param name="x">The bottom left y coordinate of the generated area.</param>
		/// <param name="width">The width of the area in tiles.</param>
		/// <param name="height">The height of the area in tiles.</param>
		public abstract Tile[,] GenerateTiles (int x, int y, int width, int height);
	}
}