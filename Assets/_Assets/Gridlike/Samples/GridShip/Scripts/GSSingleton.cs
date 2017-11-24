using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSSingleton : MonoBehaviour {

	public static GSSingleton instance;

	public int totalCube;
	public int level;
	public int cubeProgress;
	public int cubeToNextLevel;

	RootView root;

	GSCharacter character;
	GSShip ship;

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

		character.onAddCube += HandleAddCube;
		character.onRemoveCube += HandleRemoveCube;
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
	}
	void PickPickaxe() {
		character.SelectPickaxe ();
	}
	void PickPlacer() {
		character.SelectPlacer ();
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

	void IncreaseCubeProgress(int added) {
		totalCube += added;
		cubeProgress += added;

		if (cubeProgress >= cubeToNextLevel) {
			cubeProgress -= cubeToNextLevel;
			level++;

			root.notifications.ShowNotification ("New level!");
		}

		root.status.SetLevel (level);
		root.status.SetProgress (cubeProgress, cubeToNextLevel);
	}
}
