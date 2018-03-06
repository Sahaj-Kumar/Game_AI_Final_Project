using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAI : MonoBehaviour {

	float tick = 0;
	public float radius;
	Vector3 initialPosition;

	void Start () {
		initialPosition = transform.position;
	}
	
	// Make AI agent travel mindlessly in a circle.
	void FixedUpdate () {
		Vector3 offset = new Vector3(radius * Mathf.Sin(tick * Mathf.Deg2Rad), 0, radius * Mathf.Cos(tick * Mathf.Deg2Rad));
		Debug.Log(initialPosition + offset);
		transform.position = initialPosition + offset;
		tick++;
	}
}
