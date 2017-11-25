using System;
using System.Collections;
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

		button1.gameObject.SetActive (false);
		button2.gameObject.SetActive (false);
		button3.gameObject.SetActive (false);

		AddListener (choice2, callback2, button2);
		AddListener (choice1, callback1, button1);
		AddListener (choice3, callback3, button3);
	}

	public bool IsPicking() {
		return gameObject.activeSelf;
	}

	void AddListener(string choice, Action callback, Button button) {
		if (!string.IsNullOrEmpty(choice)) {
			button.gameObject.SetActive (true);
			button.GetComponentInChildren<Text> ().text = choice;
			button.onClick.RemoveAllListeners ();
			button.onClick.AddListener (() => {
				DoAfterDelay(0.1f, () => {
					callback ();
					gameObject.SetActive (false);
				});
			});
		}
	}

	public void Show() {
		gameObject.SetActive (true);
	}
	public void Hide() {
		gameObject.SetActive (false);
	}

	void DoAfterDelay(float sec, Action action) {
		StartCoroutine (_DoAfterDelay (sec, action));
	}
	IEnumerator _DoAfterDelay(float sec, Action action) {
		yield return new WaitForSeconds (sec);

		action ();
	}
}