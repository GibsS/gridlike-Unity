﻿using UnityEngine;
using System.Collections;

namespace Gridlike {
	
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

			generator.SetAlgorithm (this);
		}

		public abstract Tile[,] GenerateTiles (int x, int y, int width, int height);
	}
}