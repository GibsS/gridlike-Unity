// AUTOMATICALLY GENERATED. DO NOT EDIT MANUALLY.

using UnityEngine;
using System;
using System.Collections;

using Gridlike;

[Serializable]
public class SimpleAtlasScript : TileAtlasHelper {

	public static SimpleAtlasScript helper;
	public static TileAtlas atlas { get { return helper._atlas; } }

	public SimpleAtlasScript() {
		helper = this;
	}

	public const int SPRITE = 1;
	public const int PREFAB = 2;
	public const int TRIANGLE_HORIZONTAL = 3;
	public const int TRIANGLE_VERTICAL = 4;
	public const int UP_ONEWAY = 5;
	public const int LEFT_ONEWAY = 6;
}
