using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gridlike;

public class Arrow : MonoBehaviour {

	public void Initialize(Vector2 position, Vector2 velocity) {
		transform.position = position;

		GetComponent<Rigidbody2D> ().velocity = velocity;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		GameObject go = TransformUtility.GetTopParent (collision.gameObject);

		StartCoroutine (DestroyAnimation ());

		Grid grid = go.GetComponent<Grid> ();
		if (grid != null) {
			int x;
			int y;

			GridUtility.GetClosestNonEmptyTile (grid, transform.position, out x, out y);
			if(grid != null) grid.Clear (x, y);
		}
	}

	IEnumerator DestroyAnimation() {

		yield return null;
		Destroy (gameObject);
	}
}