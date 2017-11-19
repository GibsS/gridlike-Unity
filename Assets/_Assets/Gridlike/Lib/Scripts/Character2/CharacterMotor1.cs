using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CharacterMotor1 : MonoBehaviour {

	[Range(0.2f, 10)]
	public float width;

	[Range(0.2f, 10)]
	public float height;

	[Range(0.001f, 1)]
	public float skin;

	public float midAirForce;
	public float speed;
	public float jumpSpeed;

	float halfWidth;
	float halfHeight;

	Vector2 bounds;
	float diagonalLength;

	Vector2 upRightUnit;
	Vector2 upLeftUnit;
	Vector2 downRightUnit;
	Vector2 downLeftUnit;

	bool goLeft;
	bool goRight;

	bool jump;

	Vector2 velocity;

	Vector2 groundNormal;
	public Collider2D groundCollider;
	bool isGrounded { get { return groundCollider != null; } }

	Vector2 relativePosition;

	void GetRelativePosition() {
		if (isGrounded) {
			Debug.Log ("Get");
			transform.position = (Vector3) groundCollider.transform.TransformPoint(relativePosition);
		}
	}
	void SetRelativePosition() {
		if (isGrounded) {
			relativePosition = groundCollider.transform.InverseTransformPoint (transform.position);
			Debug.Log ("Set=" + relativePosition);
		}
	}

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
		goLeft = Input.GetKey (KeyCode.Q) && !Input.GetKey (KeyCode.D);
		goRight = Input.GetKey (KeyCode.D) && !Input.GetKey (KeyCode.Q);

		jump = Input.GetKey (KeyCode.Z);

		GetRelativePosition ();
	}

	void FixedUpdate() {
		GetRelativePosition ();

		SeparateEnvironment ();

		CheckGround ();

		// UPDATE SPEED
		if (isGrounded) {
			if (goLeft) velocity = new Vector2 (-speed, 0);
			else if (goRight) velocity = new Vector2 (speed, 0);
			else velocity = Vector2.zero;

			if (jump) {
				velocity += jumpSpeed * Vector2.up;
				jump = false;
			}
		} else {
			if (goLeft) velocity += new Vector2 (-midAirForce, 0) * Time.fixedDeltaTime;
			else if (goRight) velocity += new Vector2 (midAirForce, 0) * Time.fixedDeltaTime;

			velocity += Physics2D.gravity * Time.fixedDeltaTime;
		}

		Vector3 velocityUnit = velocity.normalized;
		float distance = velocity.magnitude * Time.fixedDeltaTime;

		RaycastHit2D hit = Physics2D.BoxCast (transform.position, bounds, 0, velocityUnit, distance);

		if (hit) {
			transform.position += velocityUnit * Mathf.Min(distance, Vector2.Dot (((Vector3)hit.point - transform.position), velocityUnit));
		} else {
			transform.position += velocityUnit * distance;
		}

		SeparateEnvironment ();

		SetRelativePosition ();
	}

	void CheckGround() {
		RaycastHit2D hit = Physics2D.BoxCast (
			transform.position + (Vector3) (halfHeight * Vector2.down), 
			new Vector2(width, skin), 
			0,
			Vector2.down, 
			4 * skin
		);

		if (hit) {
			if (!isGrounded) Debug.Log ("Is grounded!");

			groundNormal = hit.normal;
			groundCollider = hit.collider;
		} else {
			if (isGrounded) Debug.Log ("Is no longer grounded!");

			groundCollider = null;
		}
	}

	void SeparateEnvironment() {
		RaycastAndSeparate1 (Vector2.up, halfHeight + skin);
		RaycastAndSeparate1 (upRightUnit, diagonalLength + skin);
		RaycastAndSeparate1 (upLeftUnit, diagonalLength + skin);

		RaycastAndSeparate1 (Vector2.right, halfWidth + skin);
		RaycastAndSeparate1 (Vector2.left, halfWidth + skin);

		RaycastAndSeparate1 (downRightUnit, diagonalLength + skin);
		RaycastAndSeparate1 (downLeftUnit, diagonalLength + skin);
		RaycastAndSeparate1 (Vector2.down, halfHeight + skin);
	}

	void RaycastAndSeparate1(Vector2 direction, float distance) {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, distance);

		if (hit && !hit.collider.usedByEffector) {

			Vector2 offset = -direction * (distance - Vector2.Distance (hit.point, transform.position));
			offset = hit.normal * Vector2.Dot (offset, hit.normal);

			transform.position += (Vector3) offset;
		}
	}
}