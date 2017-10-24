using UnityEngine;
using System;

// TODO
[Serializable]
public class EraseRegionTool : GridTool {

	public override bool UseWindow () {
		return false;
	}
	public override string Name() {
		return "erase region";
	}

	public override void OnMouseDown () {
		grid.UnloadContainingRegion (mouseX, mouseY);
	}
	public override void OnMouseUp () {
		grid.UnloadContainingRegion (mouseX, mouseY);
	}
	public override void OnMouse () {
		grid.UnloadContainingRegion (mouseX, mouseY);
	}
}