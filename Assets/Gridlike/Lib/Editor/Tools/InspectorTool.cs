using UnityEngine;
using System;

// TODO
// TODO [Tool] Modifier of dictionary fields (maybe in inspector?)  
[Serializable]
public class InspectorTool : GridTool {

	public override bool UseWindow () {
		return true;
	}
	public override string Name() {
		return "inspect";
	}

	public override void OnMouse () {
		int x = mouseX, y = mouseY;
		Tile tile = grid.Get (x, y);

		DrawTileInformation (x, y, Color.magenta, tile == null ? null : new string[] { "id=" + tile.id });
	}
}