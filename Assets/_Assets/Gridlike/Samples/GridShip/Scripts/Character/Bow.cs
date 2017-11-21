using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Tool {

	public GameObject arrowPrefab;

	public int cubeArrowCost;
	public float cooldown;
	public float speed;
	public int damage;

	float lastShot = -100000;

	public override void OnMouseDown (Vector2 position) {
		TryShoot (position);
	}
	public override void OnMouse (Vector2 position) {
		TryShoot (position);
	}
	public override void OnMouseUp (Vector2 position) {
		TryShoot (position);
	}

	void TryShoot(Vector2 position) {
		if (Time.time - lastShot > cooldown && character.GetCubeCount() >= cubeArrowCost) {
			lastShot = Time.time;
			character.ConsumeCubes (cubeArrowCost);
			Shoot (position);
		}
	}

	void Shoot(Vector2 position) {
		// TODO - factorize into factory
		GameObject arrow = Instantiate (arrowPrefab);
		arrow.GetComponent<Arrow> ().Initialize (
			character,
			damage,
			transform.position, 
			speed * (position - (Vector2) transform.position).normalized
		);
	}
}
