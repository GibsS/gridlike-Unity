using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class Arrow : MonoBehaviour {

	GSCharacter character;
	int damage;

	public void Initialize(GSCharacter character, int damage, Vector2 position, Vector2 velocity) {
		this.character = character;
		this.damage = damage;

		transform.position = position;

		GetComponent<Rigidbody2D> ().velocity = velocity;

		StartCoroutine (DestroyAfterTime (10));
	}

	void OnCollisionEnter2D(Collision2D collision) {
		GameObject go = TransformUtility.GetTopParent (collision.gameObject);

		Grid grid = go.GetComponent<Grid> ();
		if (grid != null) {
			int x;
			int y;

			GridUtility.GetClosestNonEmptyTile (grid, transform.position, out x, out y);
			if (grid != null) {
				GSGrid gsGrid = grid.GetComponent<GSGrid> ();

				gsGrid.Damage (character, x, y, damage, transform.position);
			}
		}

		StartCoroutine (DestroyAnimation ());
	}

	IEnumerator DestroyAnimation() {

		yield return null;
		Destroy (gameObject);
	}

	IEnumerator DestroyAfterTime(int time) {
		yield return new WaitForSeconds (time);

		Destroy (gameObject);
	}
}