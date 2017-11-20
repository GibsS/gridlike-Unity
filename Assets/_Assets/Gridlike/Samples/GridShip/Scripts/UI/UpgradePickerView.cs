using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePickerView : MonoBehaviour {

	public Action pick1;
	public Action pick2;
	public Action pick3;

	public Button button1;
	public Button button2;
	public Button button3;

	public void ProposeUpgrades(string choice1, Action callback1, string choice2, Action callback2, string choice3, Action callback3) {
		gameObject.SetActive (true);

		AddListener (choice1, callback1, button1);
		AddListener (choice2, callback2, button2);
		AddListener (choice3, callback3, button3);
	}

	void AddListener(string choice, Action callback, Button button) {
		button.GetComponentInChildren<Text> ().text = choice;
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (() => {
			callback();
			gameObject.SetActive(false);
		});
	}

	public void Show() {
		gameObject.SetActive (true);
	}
	public void Hide() {
		gameObject.SetActive (false);
	}
}