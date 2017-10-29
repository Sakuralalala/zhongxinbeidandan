using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour {
    public float speed = 90;
	public Transform rotationCenter;

	void Update () {
		Vector3 zero = rotationCenter.position;
        transform.RotateAround(zero, Vector3.back,speed* Time.deltaTime);
	}
}
