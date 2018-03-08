using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedFollower : MonoBehaviour {

	public GameObject target;
	private PlayerController playerController;
	private float groundReference;

	// Use this for initialization
	void Awake () {
		playerController = target.GetComponent<PlayerController>();
		groundReference = target.transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerController.grounded) {
			transform.position = target.transform.position;
			groundReference = target.transform.position.y;
		}
		else {
			Vector3 foo = target.transform.position;
			foo.y = groundReference;
			transform.position = foo;
		}
	}
}
