using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSCharacter : MonoBehaviour {

	public Bow bow;
	public Pickaxe pickaxe;
	public Placer placer;

	public Component currentTool;

	public void Start () {
		GSSingleton.instance.RegisterCharacter (this);
	}

	public bool HasBow() {
		return bow != null && bow.enabled;
	}
	public bool HasPickaxe() {
		return pickaxe != null && pickaxe.enabled;
	}
	public bool HasPlacer() {
		return placer != null && placer.enabled;
	}

	public void AcquireBow() {
		if (bow == null) bow = gameObject.AddComponent<Bow> ();

		bow.enabled = true;
	}
	public void AcquirePickaxe() {
		if (pickaxe == null) pickaxe = gameObject.AddComponent<Pickaxe> ();

		pickaxe.enabled = true;
	}
	public void AcquirePlacer() {
		if (placer == null) placer = gameObject.AddComponent<Placer> ();

		placer.enabled = true;
	}

	public void SelectBow() {
		currentTool = bow;
	}
	public void SelectPickaxe() {
		currentTool = pickaxe;
	}
	public void SelectPlacer() {
		currentTool = placer;
	}
}

