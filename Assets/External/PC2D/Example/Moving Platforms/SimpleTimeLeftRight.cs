using UnityEngine;
using System.Collections;

public class SimpleTimeLeftRight : MonoBehaviour
{

	public float speed;
	[Range(1, 100)]
	public float duration;

	MovingPlatformMotor2D _mpMotor;
	float lastTime;

	// Use this for initialization
	void Start()
	{
		_mpMotor = GetComponent<MovingPlatformMotor2D>();
		_mpMotor.velocity = -Vector2.right * speed;
		lastTime = Time.time;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (_mpMotor.velocity.x < 0 && Time.time - lastTime > duration)
		{
			lastTime = Time.time;
			_mpMotor.velocity = Vector2.right * speed;
		}
		else if (_mpMotor.velocity.x > 0 && Time.time - lastTime > duration)
		{
			lastTime = Time.time;
			_mpMotor.velocity = -Vector2.right * speed;
		}
	}
}