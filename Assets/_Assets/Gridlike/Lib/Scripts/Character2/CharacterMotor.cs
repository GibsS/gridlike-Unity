using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CharacterMotor : MonoBehaviour {

	[Range(0.2f, 10)]
	public float width;

	[Range(0.2f, 10)]
	public float height;

	[Range(0.001f, 1)]
	public float skin;

	public float speed;

	float halfWidth;
	float halfHeight;

	Vector2 bounds;
	float diagonalLength;

	Vector2 upRightUnit;
	Vector2 upLeftUnit;
	Vector2 downRightUnit;
	Vector2 downLeftUnit;

	float xSpeed;
	float ySpeed;

	Vector2 debugPoint;

	// Use this for initialization
	void Start () {
		halfWidth = width / 2;
		halfHeight = height / 2;

		bounds = new Vector2 (width, height);
		diagonalLength = Mathf.Sqrt (halfWidth * halfWidth + halfHeight * halfHeight);

		upRightUnit = new Vector2 (width, height).normalized;
		upLeftUnit = new Vector2 (-width, height).normalized;
		downRightUnit = new Vector2 (width, -height).normalized;
		downLeftUnit = new Vector2 (-width, -height).normalized;
	}

	void Update() {
		if (Input.GetKey (KeyCode.Q)) {
			xSpeed = -speed;
		} else if (Input.GetKey (KeyCode.D)) {
			xSpeed = speed;
		} else {
			xSpeed = 0;
		}

		if (Input.GetKey (KeyCode.S)) {
			ySpeed = -speed;
		} else if (Input.GetKey (KeyCode.Z)) {
			ySpeed = speed;
		} else {
			ySpeed = 0;
		}
	}

	void FixedUpdate() {
		//SeparateEnvironment ();

		transform.position += new Vector3(xSpeed * Time.fixedDeltaTime, ySpeed * Time.fixedDeltaTime);

		SeparateEnvironment ();
	}

	void SeparateEnvironment() {
		RaycastAndSeparate1 (upRightUnit, diagonalLength + skin);
		RaycastAndSeparate1 (upLeftUnit, diagonalLength + skin);
		RaycastAndSeparate1 (downRightUnit, diagonalLength + skin);
		RaycastAndSeparate1 (downLeftUnit, diagonalLength + skin);

		RaycastAndSeparate1 (Vector2.right, halfWidth + skin);
		RaycastAndSeparate1 (Vector2.left, halfWidth + skin);
		RaycastAndSeparate1 (Vector2.up, halfHeight + skin);
		RaycastAndSeparate1 (Vector2.down, halfHeight + skin);
	}

	private void RaycastAndSeparate1(Vector2 direction, float distance) {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, distance);

		if (hit && !hit.collider.usedByEffector
			//&& hit.point.x >= transform.position.x - halfWidth && hit.point.x <= transform.position.x + halfWidth
			//&& hit.point.y >= transform.position.y - halfHeight && hit.point.y <= transform.position.y + halfHeight
		) {

			Vector2 offset = -direction * (distance - Vector2.Distance (hit.point, transform.position));
			offset = hit.normal * Vector2.Dot (offset, hit.normal);

			transform.position += (Vector3) offset;
		}
	}

	private Vector2 GetPointOnBounds(Vector3 toPoint) {
		// From http://stackoverflow.com/questions/4061576/finding-points-on-a-rectangle-at-a-given-angle
		float angle = Vector3.Angle(Vector3.right, toPoint);

		if (toPoint.y < 0) {
			angle = 360f - angle;
		}

		float multiplier = 1f;

		if ((angle >= 0f && angle < 45f) ||
			angle > 315f ||
			(angle >= 135f && angle < 225f))
		{

			if (angle >= 135f && angle < 225f) {
				multiplier = -1f;
			}

			return new Vector2(
				multiplier * halfWidth + transform.position.x,
				transform.position.y + multiplier * (halfHeight * Mathf.Tan(angle * Mathf.Deg2Rad))
			);
		}

		if (angle >= 225f) {
			multiplier = -1f;
		}

		return new Vector2(
			transform.position.x + multiplier * halfHeight / Mathf.Tan(angle * Mathf.Deg2Rad),
			multiplier * halfHeight + transform.position.y
		);
	}

	void OnDrawGizmos() {
		Gizmos.DrawSphere (debugPoint, 0.1f);
	}
}