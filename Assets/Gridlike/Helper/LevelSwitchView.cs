using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSwitchView : MonoBehaviour {

	public string scene;

	Button button;

	void Start() {
		button = GetComponent<Button> ();

		button.onClick.AddListener (() => SceneManager.LoadScene (scene));
	}
}
