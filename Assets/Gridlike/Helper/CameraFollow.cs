using UnityEngine;
using System.Collections;

namespace Gridlike {
	
    public class CameraFollow : MonoBehaviour {
		
        public Transform target;

        // Update is called once per frame
        void LateUpdate() {
            Vector3 pos = transform.position;
            pos.x = target.position.x;
            pos.y = target.position.y;

            transform.position = pos;
        }
    }
}
