using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastJump : MonoBehaviour {

	public Transform target;
	private PlayerController playerController;

	void Awake () {
		playerController = target.GetComponent<PlayerController>();
	}

	void FixedUpdate () {
		if (playerController.IsGrounded()) {
			transform.position = target.position;
		}
	}
}
