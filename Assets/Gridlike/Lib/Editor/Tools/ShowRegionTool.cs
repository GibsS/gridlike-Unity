using UnityEngine;
using System;

[Serializable]
public class ShowRegionTool : GridTool {

	public override bool UseWindow () {
		return false;
	}
	public override string Name() {
		return "show region";
	}

	public override void OnMouse (){
		grid.PresentContainingRegion (mouseX, mouseY);
	}
}