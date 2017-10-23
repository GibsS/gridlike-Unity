using UnityEngine;
using System;

[Serializable]
public class HideRegionTool : GridTool {

	public override bool UseWindow () {
		return false;
	}
	public override string Name() {
		return "hide region";
	}

	public override void OnMouse (){
		grid.HideContainingRegion (mouseX, mouseY);
	}
}