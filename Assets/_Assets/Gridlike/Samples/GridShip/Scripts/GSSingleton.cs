using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INotifier {

	void Notify (string notification);
}

public class GSSingleton : MonoBehaviour, INotifier {

	public static GSSingleton instance;

	public int totalCube;
	public int level = 1;
	public int cubeProgress;
	public int cubeToNextLevel;

	RootView root;

	public GSCharacter character { get; private set; }
	public GSShip ship { get; private set; }

	Tutorial tutorial;

	UpgradeProgression progression;
	Queue<bool> upgradeQueue;

	void Awake() {

		if (instance != null) {
			Destroy (this);
			return;
		}

		instance = this;

		Initialize ();
	}

	void Initialize() {
		if (root == null) {
			GameObject uiGO = GameObject.Find ("UI");
			root = uiGO.GetComponent<RootView> ();
			root.Initialize ();


			root.toolbar.onBowClick += PickBow;
			root.toolbar.onPickaxeClick += PickPickaxe;
			root.toolbar.onPlacerClick += PickPlacer;

			root.notifications.Hide ();
			root.tilePicker.Hide ();
			root.upgrades.Hide ();

			root.Hide ();

			progression = new UpgradeProgression1 ();

			upgradeQueue = new Queue<bool> ();

			tutorial = new Tutorial ();
			tutorial._Inject (this);
		}
	}

	void SetupUI() {
		root.Show ();

		if (character.HasBow ()) root.toolbar.EnableBow ();
		else root.toolbar.DisableBow ();

		if (character.HasPickaxe ()) root.toolbar.EnablePickaxe ();
		else root.toolbar.DisablePickaxe ();

		if (character.HasPlacer ()) root.toolbar.EnablePlacer ();
		else root.toolbar.DisablePlacer ();

		if (character.currentTool as Bow) root.toolbar.SelectBow ();
		if (character.currentTool as Pickaxe) root.toolbar.SelectPickaxe ();
		if (character.currentTool as Placer) root.toolbar.SelectPlacer ();

		root.status.SetCubeCount (character.cubeCount);

		character.onAddCube += HandleAddCube;
		character.onRemoveCube += HandleRemoveCube;
	}

	void Update() {
		tutorial.Upgrade ();

		if (character.HasBow()) {
			root.toolbar.EnableBow ();
		}
		if (character.HasPlacer()) {
			root.toolbar.EnablePlacer ();
		}

		if (upgradeQueue.Count > 0 && !root.upgrades.IsPicking()) {
			List<Upgrade> upgrades;

			if (upgradeQueue.Dequeue ()) {
				upgrades = progression.GetSpecialUpgrades (3);

				if (upgrades.Count == 1) {
					HandleUprade (upgrades [0]);

					upgrades.Clear ();
				}
			} else {
				upgrades = progression.GetNormalUpgrades (3);
			}

			if (upgrades.Count > 0) {

				Upgrade upgrade1 = null;
				Upgrade upgrade2 = null;
				Upgrade upgrade3 = null;

				if (upgrades.Count > 0)
					upgrade1 = upgrades [0]; 
				if (upgrades.Count > 1)
					upgrade2 = upgrades [1]; 
				if (upgrades.Count > 2)
					upgrade3 = upgrades [2];

				root.upgrades.ProposeUpgrades (
					upgrade1 != null ? upgrade1.Name () : null, () => HandleUprade (upgrade1),
					upgrade2 != null ? upgrade2.Name () : null, () => HandleUprade (upgrade2),
					upgrade3 != null ? upgrade3.Name () : null, () => HandleUprade (upgrade3)
				);
			}
		}
	}

	void HandleUprade(Upgrade upgrade) {
		upgrade._Inject (character, ship);
		upgrade.Execute ();
		progression.CompleteUpgrade (upgrade);

		Notify (upgrade.Description ());
	}

	public void RegisterCharacter(GSCharacter character) {
		if (this.character == null) {
			this.character = character;

			Initialize ();

			SetupUI ();
		}
	}
	public void RegisterShip(GSShip ship) {
		if (this.ship == null) {
			this.ship = ship;

			Initialize ();
		}
	}

	void HandleAddCube(int cube) {
		root.status.SetCubeCount (character.cubeCount);

		IncreaseCubeProgress (character.cubeCount - cube);
	}
	void HandleRemoveCube(int cube) {
		root.status.SetCubeCount (character.cubeCount);
	}

	void PickBow() {
		character.SelectBow ();
		root.toolbar.SelectBow ();
	}
	void PickPickaxe() {
		character.SelectPickaxe ();
		root.toolbar.SelectPickaxe ();
	}
	void PickPlacer() {
		character.SelectPlacer ();
		root.toolbar.SelectPlacer ();
	}

	void AcquireBow() {
		character.AcquireBow ();
		root.toolbar.EnableBow ();
	}
	void AcquirePickaxe() {
		character.AcquirePickaxe ();
		root.toolbar.EnablePickaxe ();
	}
	void AcquirePlacer() {
		character.AcquirePlacer ();
		root.toolbar.EnablePlacer ();
	}

	void InitializeLevel() {
		root.status.SetLevel (level);
		cubeToNextLevel = 100;
	}

	public void AcquireSpecialUpgrade() {
		upgradeQueue.Enqueue (true);
	}
	void IncreaseCubeProgress(int added) {
		totalCube += added;
		cubeProgress += added;

		if (cubeProgress >= cubeToNextLevel) {
			cubeProgress -= cubeToNextLevel;
			level++;
			cubeToNextLevel += (level-1) * 100;

			upgradeQueue.Enqueue (false);
		}

		root.status.SetLevel (level);
		root.status.SetProgress (cubeProgress, cubeToNextLevel);
	}

	public void Notify(string notification) {
		root.notifications.ShowNotification (notification);
	}
}
