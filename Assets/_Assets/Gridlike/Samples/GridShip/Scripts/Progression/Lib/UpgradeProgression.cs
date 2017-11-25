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

		readyUpgrades.AddRange (move);
		foreach (Upgrade upgrade in move) upgrades.Remove (upgrade);
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
		}
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