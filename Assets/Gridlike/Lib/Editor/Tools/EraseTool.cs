using UnityEngine;
using System.Collections;

public class EraseTool : GridTool {

	public override bool UseWindow() {
		return false;
	}
	public override string Name() {
		return "erase";
	}

	public override void OnMouse() {
		grid.Clear (mouseX, mouseY);
	}
}