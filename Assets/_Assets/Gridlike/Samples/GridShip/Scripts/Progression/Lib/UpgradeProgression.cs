using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class UpgradeProgression {

	List<Upgrade> upgrades;
	List<Upgrade> readyUpgrades;
	List<Upgrade> achievedUpgrades;

	public UpgradeProgression() {
		upgrades = new List<Upgrade> ();
		readyUpgrades = new List<Upgrade> ();
		achievedUpgrades = new List<Upgrade> ();

		Initialize ();
	}

	void Initialize() {
		ProgressionDefinition ();

		List<Upgrade> move = new List<Upgrade>();
		foreach (Upgrade upgrade in upgrades) {
			if (upgrade.dependentIds.Count == 0) {
				move.Add (upgrade);
			}
		}

		Utility.MoveList (upgrades, readyUpgrades, move);
	}

	public Upgrade GetUpgrade(int id) {
		Upgrade upgrade = upgrades.Find (u => u.id == id);

		if (upgrade != null) return upgrade;

		upgrade = readyUpgrades.Find (u => u.id == id);

		if (upgrade != null) return upgrade;

		upgrade = achievedUpgrades.Find (u => u.id == id);

		return upgrade;
	}

	public List<Upgrade> GetSpecialUpgrades(int count) {
		return new List<Upgrade>(readyUpgrades.Where(x => x.isSpecial)
			.OrderBy (x => Random.Range (0, 1))
			.Take (Mathf.Min (count, readyUpgrades.Count)));
	}
	public List<Upgrade> GetNormalUpgrades(int count) {
		return new List<Upgrade>(readyUpgrades.OrderBy (x => Random.Range (0, 1))
			.Where(x => !x.isSpecial)
			.Take (Mathf.Min (count, readyUpgrades.Count)));
	}
	public void CompleteUpgrade(Upgrade upgrade) {
		upgrades.Remove (upgrade);
		if (readyUpgrades.Remove (upgrade)) {
			achievedUpgrades.Add (upgrade);
			upgrade.isDone = true;
		}

		List<Upgrade> move = new List<Upgrade> ();
		foreach (Upgrade u in upgrades) {
			if (u.dependentIds.All (u1 => GetUpgrade(u1).isDone)) {
				move.Add (u);
			}
		}

		Utility.MoveList (upgrades, readyUpgrades, move);
	}

	protected Upgrade Add(Upgrade upgrade, params Upgrade[] dependecies) {
		upgrades.Add (upgrade);

		foreach (Upgrade dependency in dependecies) {
			upgrade.DependOn (dependency);
		}

		return upgrade;
	}

	public abstract void ProgressionDefinition ();
}