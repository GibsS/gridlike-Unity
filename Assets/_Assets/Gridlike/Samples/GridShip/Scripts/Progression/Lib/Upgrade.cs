using UnityEngine;
using System.Collections.Generic;

public abstract class Upgrade {

	public static int upgradeCurrentId = 0;

	public int id { get; private set; }
	public List<int> dependentIds { get; private set; }

	public bool isSpecial;

	protected GSCharacter character { get; private set; }
	protected GSShip ship { get; private set; }

	public Upgrade() {
		id = upgradeCurrentId++;
		dependentIds = new List<int> ();
	}

	public void _Inject(GSCharacter character, GSShip ship) {
		this.character = character;
		this.ship = ship;
	}

	public void DependOn(Upgrade upgrade) {
		dependentIds.Add (upgrade.id);
	}

	public abstract string Name();
	public abstract string Description ();

	public abstract void Execute();
}