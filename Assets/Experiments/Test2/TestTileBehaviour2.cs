using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class TestTileBehaviour2 : TileBehaviour {

	static bool[,] _area = { { true, true }, { true, true } };

	public override bool[,] area { get { return _area; } }
	public override int areaBottomLeftXOffset { get { return -1; } }
	public override int areaBottomLeftYOffset { get { return -1; } }
}