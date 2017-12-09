using UnityEngine;
using System.Collections;

namespace Gridship {

	public class AntiGravity : GSTileBehaviour {

		public int extraMass;

		public override void OnShow () {
			GSShip ship = grid.GetComponent<GSShip> ();

			if(ship != null) ship.maxMass += extraMass;
		}

		public override void OnHide() {
			GSShip ship = grid.GetComponent<GSShip> ();

			if(ship != null) ship.maxMass -= extraMass;
		}
	}
}