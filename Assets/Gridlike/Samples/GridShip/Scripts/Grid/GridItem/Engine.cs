using UnityEngine;
using System.Collections;

namespace Gridship {

	public class Engine : GSTileBehaviour {

		public int extraPropulsion;

		public override void OnShow () {
			GSShip ship = grid.GetComponent<GSShip> ();

			if(ship != null) ship.propulsionForce += extraPropulsion;
		}

		public override void OnHide() {
			GSShip ship = grid.GetComponent<GSShip> ();

			if(ship != null) ship.propulsionForce -= extraPropulsion;
		}
	}
}