using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileBehaviour : MonoBehaviour {

	public abstract bool[,] area { get; }
	public abstract int areaBottomLeftXOffset { get; }
	public abstract int areaBottomLeftYOffset { get; }
}
