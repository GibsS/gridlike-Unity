using UnityEngine;

using Gridlike;

namespace PC2D
{
    public class SimpleLeftRight : MonoBehaviour
    {
        public float leftRightAmount;
        public float speed;

        private MovingPlatformMotor2D _mpMotor;
        private float _startingX;

		float lastTime;

        // Use this for initialization
        void Start()
        {
            _mpMotor = GetComponent<MovingPlatformMotor2D>();
            _startingX = transform.position.x;
            _mpMotor.velocity = -Vector2.right * speed;
			lastTime = Time.time;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
			if (_mpMotor.velocity.x < 0 && (_startingX - transform.position.x >= leftRightAmount || Time.time - lastTime > 4))
            {
				lastTime = Time.time;
                //transform.position += Vector3.right * ((_startingX - transform.position.x) - leftRightAmount);
                _mpMotor.velocity = Vector2.right * speed;
            }
			else if (_mpMotor.velocity.x > 0 && (transform.position.x - _startingX >= leftRightAmount || Time.time - lastTime > 4))
			{
				lastTime = Time.time;
                //transform.position += -Vector3.right * ((transform.position.x - _startingX) - leftRightAmount);
                _mpMotor.velocity = -Vector2.right * speed;
            }
        }
    }
}
