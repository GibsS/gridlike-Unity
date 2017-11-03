using UnityEngine;
using System;

namespace Gridlike {
	
	[Serializable]
	public class ShowRegionTool : GridTool {

		public override bool UseWindow () {
			return false;
		}
		public override string Name() {
			return "show region";
		}

		public override bool OnMouseUp () {
			grid.PresentContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouseDown () {
			grid.PresentContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouse () {
			grid.PresentContainingRegion (mouseX, mouseY);
			return true;
		}
	}
}