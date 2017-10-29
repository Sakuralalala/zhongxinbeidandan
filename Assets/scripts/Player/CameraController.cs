using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float velocity = 10;
	public void move(Vector3 dir){
		Vector3 tmpPosition = transform.position + dir*velocity;
		if (tmpPosition.x > 10)
			tmpPosition.x = 10;
		if (tmpPosition.x < -10)
			tmpPosition.x = -10;
		if (tmpPosition.y > 5.5)
			tmpPosition.y = 5.5f;
		if (tmpPosition.y < -5.5)
			tmpPosition.y = -5.5f;
		transform.position = tmpPosition;
	}
}
