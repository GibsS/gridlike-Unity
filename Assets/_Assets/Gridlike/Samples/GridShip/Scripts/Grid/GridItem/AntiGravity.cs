using UnityEngine;
using System.Collections;

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