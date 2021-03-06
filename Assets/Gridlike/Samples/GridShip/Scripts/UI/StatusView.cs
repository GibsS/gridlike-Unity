﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gridship {

	public class StatusView : MonoBehaviour {

		public Slider progressSlider;
		public Text cubeCountText;
		public Text levelText;

		public void Initialize() {
			progressSlider.interactable = false;
		}

		public void SetProgress(float progress, float progressMax) {
			progressSlider.value = progress;
			progressSlider.maxValue = progressMax;
		}
		public void SetCubeCount(int cubeCount) {
			cubeCountText.text = cubeCount + " cube" + (cubeCount == 1 ? "" : "s");
		}

		public void SetLevel(int level) {
			levelText.text = "LEVEL " + level;
		}

		public void Show() {
			gameObject.SetActive (true);
		}
		public void Hide() {
			gameObject.SetActive (false);
		}
	}
}