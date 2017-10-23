using UnityEngine;
using System;

// TODO
[Serializable]
public class EraseRegionTool : GridTool {

	public override bool UseWindow () {
		return true;
	}
	public override string Name() {
		return "erase region";
	}

	public override void OnMouse () {
		grid.UnloadContainingRegion (mouseX, mouseY);
	}
}