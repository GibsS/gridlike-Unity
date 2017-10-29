using UnityEngine;
using System;

namespace Gridlike {
	
	[Serializable]
	public class EraseRegionTool : GridTool {

		public override bool UseWindow () {
			return false;
		}
		public override string Name() {
			return "erase region";
		}

		public override bool OnMouseDown () {
			grid.UnloadContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouseUp () {
			grid.UnloadContainingRegion (mouseX, mouseY);
			return true;
		}
		public override bool OnMouse () {
			grid.UnloadContainingRegion (mouseX, mouseY);
			return true;
		}
	}
}