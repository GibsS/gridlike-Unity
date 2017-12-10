using UnityEngine;
using System.Collections;

using Gridlike;

namespace Gridship {

	public class GridUtility {

		public static void ExplodeInAllGrid(GSCharacter character, Vector2 position, int radius, int damage) {
			foreach (Gridlike.Grid grid in Gridlike.Grid.GetAllGrids()) {
				GSGrid wrapper = grid.GetComponent<GSGrid> ();

				if (wrapper != null) {
					wrapper.Explosion (character, position, radius, damage);
				} else {
					Debug.LogWarning ("Grid has no GSGrid");
				}
			}
		}
	}
}