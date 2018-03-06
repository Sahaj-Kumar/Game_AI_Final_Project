﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;

	[Header("Basic Settings")]
	public float jumpVelocity = 10.0f;
	public float boostVelocity = 20.0f;
	public float walkSpeed = 5.0f;
	public float runSpeed = 10.0f;

	[Header("Smooth Settings")]
	public float smoothTurn = 0.1f;
	public float smoothSpeed = 0.25f;

	private float smoothTurnVelocity;
	private float smoothSpeedVelocity;
	private float currentSpeed;
	private bool jumpRequest = false;
	private bool boostRequest = false;

	// TODO setup animator component

	private KeyCode RUNNING_KEY = KeyCode.LeftShift;
	private KeyCode JUMP_KEY = KeyCode.Space;
	private KeyCode BOOST_KEY = KeyCode.B; // temporary

	[Header("Advanced Settings")]
	[Tooltip("Tolerance for considering groundedness")]
	public float groundedSkin = 0.05f;
	[Tooltip("Layers considered when checking groundedness")]
	public LayerMask mask;
	private Vector3 playerSize;
	private Vector3 boxSize;
	private bool grounded = false;

	void Awake () {
		rb = gameObject.GetComponent<Rigidbody>();
		playerSize = GetComponent<BoxCollider>().size;
		boxSize = new Vector3(playerSize.x, groundedSkin, playerSize.z);
	}

	void Update() {
		// Handle jump inputs before handling in FixedUpdate.
		if (Input.GetKeyDown(BOOST_KEY) && grounded) {
			boostRequest = true;
		}
		else if (Input.GetKeyDown(JUMP_KEY) && grounded) {
			jumpRequest = true;
		}
	}

	/*
		Handles player movement, which is partially	based off built-in physics.
		Also, it was the only way to make the 3rd person camera stop being jittery :/
	*/
	void FixedUpdate () {
		MoveHandler();
		JumpHandler();
	}

	/**
		Handles 'joystick' input for walking and running.
		This is done with translation. (No physics)
	*/
	void MoveHandler() {
		// Get input vector.
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDirection = input.normalized;

		// Set player's facing direction (with smoothing).
		if (inputDirection != Vector2.zero) {
			float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,targetRotation, ref smoothTurnVelocity, smoothTurn);
		}

		// Translate player given input magnitude (with smoothing).
		bool running = Input.GetKey(RUNNING_KEY);
		float targetSpeed = (running ? runSpeed : walkSpeed) * inputDirection.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothSpeedVelocity, smoothSpeed);
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
	}

	/*
		Handles jumping (relies on jumpRequest status) and tracks player groundedness.
		This is done by applying forces to the player's rigidbody.
	*/
	void JumpHandler() {
		// Execute jump if permitted.
		if (jumpRequest || boostRequest) {
			Debug.Log("Jumping!");
			rb.AddForce(Vector3.up * (jumpRequest ? jumpVelocity : boostVelocity), ForceMode.Impulse);
			jumpRequest = false;
			boostRequest = false;
		}
		// Update grounded status.
		Vector3 boxCenter = transform.position + Vector3.down * (playerSize.y + boxSize.y) * 0.5f;
		grounded = (Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, mask).Length > 0);
	}

}
