using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridship {

	public class SpecialUpgradeCube : GSTileBehaviour {

		public override void OnHide() {
			if (GSSingleton.instance != null) {
				GSSingleton.instance.AcquireSpecialUpgrade ();
			} else {
				Debug.LogError ("Can't get special upgrade");
			}
		}
	}
}