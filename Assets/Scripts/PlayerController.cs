using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Rigidbody rb;
	public float acceleration = 1.0f;
	public float maxVelocity = 5.0f;

	public float walkSpeed = 1.0f;
	public float runSpeed = 2.0f;

	public float smoothTurn = 0.1f;
	private float smoothTurnVelocity;

	public float smoothSpeed = 0.25f;
	private float smoothSpeedVelocity;
	private float currentSpeed;

	// TODO setup animator component

	private KeyCode RUNNING_KEY = KeyCode.LeftShift;
	//private KeyCode JUMP_KEY = KeyCode.Space;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		Debug.Log("Hello world!");
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDirection = input.normalized;

		if (inputDirection != Vector2.zero) {
			float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,targetRotation, ref smoothTurnVelocity, smoothTurn);
		}

		bool running = Input.GetKey(RUNNING_KEY);
		float targetSpeed = (running ? runSpeed : walkSpeed) * inputDirection.magnitude;

		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothSpeedVelocity, smoothSpeed);

		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

		// TODO: update animator

	}

}
