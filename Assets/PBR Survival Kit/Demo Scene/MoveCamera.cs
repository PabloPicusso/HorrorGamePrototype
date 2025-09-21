using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

	private Camera can;
	public float speed = 0.1f;
	private bool Direction;

	void Start () {
		can = GetComponent<Camera> ();
		Direction = true;
	}

	void Update () {

		if (transform.position.x >= 54) {
			Direction = true;
		}
		if (transform.position.x <= -54) {
			Direction = false;
		}

		if (Direction == true) {
			transform.Translate (Vector3.left * -speed);
		}
		if (Direction == false) {
			transform.Translate (Vector3.left * speed);
		} 

		if (transform.position.x <= 16) {
			can.fieldOfView = (Mathf.Lerp (can.fieldOfView, 40, Time.deltaTime / 4));
		}
		if (transform.position.x >= 17) {
			can.fieldOfView = (Mathf.Lerp (can.fieldOfView, 60, Time.deltaTime / 4));
		}
	}
}
