using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour {

	public float fallMultiplier = 2.5f;
	public float riseMultiplier = 2.0f;

	Rigidbody rb;

	void Awake () {
		rb = GetComponent<Rigidbody>();
	}

	/*
		Skews gravity effect when both ascending and descending.
	*/
	void FixedUpdate () {
		if (rb.velocity.y < 0) {
			rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
		else {
			rb.velocity += Vector3.up * Physics.gravity.y * (riseMultiplier - 1) * Time.deltaTime;
		}
	}
}
