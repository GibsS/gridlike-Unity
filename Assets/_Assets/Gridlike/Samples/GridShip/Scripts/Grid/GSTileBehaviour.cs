using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class GSTileBehaviour : TileBehaviour {

	public int width;
	public int height;

	public override bool[,] area { get { return CreateBoolGrid (width, height, true); } }
	public override int areaBottomLeftXOffset { get { return 0; } }
	public override int areaBottomLeftYOffset { get { return 0; } }
}
