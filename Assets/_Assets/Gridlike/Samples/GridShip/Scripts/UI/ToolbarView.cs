using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarView : MonoBehaviour {

	public event Action onBowClick;
	public event Action onPickaxeClick;
	public event Action onPlacerClick;

	public Button bowButton;
	public Button pickaxeButton;
	public Button placerButton;

	public void EnableBow() {
		bowButton.gameObject.SetActive (true);
	}
	public void EnablePickaxe() {
		pickaxeButton.gameObject.SetActive (true);
	}
	public void EnablePlacer() {
		placerButton.gameObject.SetActive (true);
	}
	public void DisableBow() {
		bowButton.gameObject.SetActive (false);
	}
	public void DisablePickaxe() {
		pickaxeButton.gameObject.SetActive (false);
	}
	public void DisablePlacer() {
		placerButton.gameObject.SetActive (false);
	}

	public void Initialize() {
		bowButton.onClick.AddListener (() => {
			if(onBowClick != null) onBowClick();
		});
		pickaxeButton.onClick.AddListener (() => {
			if(onPickaxeClick != null) onPickaxeClick();
		});
		placerButton.onClick.AddListener (() => {
			if(onPlacerClick != null) onPlacerClick();
		});
	}

	public void Show() {
		gameObject.SetActive (true);
	}
	public void Hide() {
		gameObject.SetActive (false);
	}
}
