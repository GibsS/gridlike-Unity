using System;
using UnityEngine;

[Serializable]
public class TileInfo {

	public int id;
	public new string name;
	public TileShape defaultShape;

	public TileSpriteInfo idSpriteInfo;
	public TileSpriteInfo[] subIdSpriteInfo;
}