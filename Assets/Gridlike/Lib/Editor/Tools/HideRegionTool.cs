using UnityEngine;
using System;

namespace Gridlike {
	
	[Serializable]
	public class HideRegionTool : GridTool {

		public override bool UseWindow () {
			return false;
		}
		public override string Name() {
			return "hide region";
		}

		public override bool OnMouseDown () {
			grid.HideContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouseUp () {
			grid.HideContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouse (){
			grid.HideContainingRegion (mouseX, mouseY);
			return true;
		}
	}
}