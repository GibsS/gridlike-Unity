using UnityEngine;
using System.Collections.Generic;

public class Utility {

	public static void MoveList<X>(List<X> source, List<X> target, List<X> move) {
		target.AddRange (move);

		foreach (X u in move) {
			source.Remove (u);
		}
	}
}