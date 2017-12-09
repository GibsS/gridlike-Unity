using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gridship {

	public class TilePickerView : MonoBehaviour {

		public GameObject choiceContainerGO;
		public GameObject tilePickerChoicePrefab;

		public void AddChoice(string name, Action action) {
			GameObject obj = Instantiate (tilePickerChoicePrefab);
			obj.transform.SetParent (tilePickerChoicePrefab.transform, false);

			obj.GetComponentInChildren<Text> ().text = name;
			obj.GetComponent<Button> ().onClick.AddListener (() => action ());
		}

		public void Show() {
			gameObject.SetActive (true);
		}
		public void Hide() {
			gameObject.SetActive (false);
		}
	}
}