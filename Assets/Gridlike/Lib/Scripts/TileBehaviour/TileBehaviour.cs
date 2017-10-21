using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileBehaviour : MonoBehaviour {

	public int _x;
	public int _y;

	public int x { get { return _x; } }
	public int y { get { return _y; } }

	public abstract bool[,] area { get; }
	public abstract int areaBottomLeftXOffset { get; }
	public abstract int areaBottomLeftYOffset { get; }
}
