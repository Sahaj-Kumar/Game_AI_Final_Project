using System.Collections;
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
	private bool pickupRequest = false;

	private KeyCode BOOST_KEY = KeyCode.B; // temporary
	private KeyCode PICKUP_KEY = KeyCode.E; // temporary

	[Header("Advanced Settings")]
	[Tooltip("Tolerance for considering groundedness")]
	public float respawnElevation;
	private Vector3 jPlayer;
	public float groundedSkin = 0.05f;
	[Tooltip("Layers considered when checking groundedness")]
	public LayerMask mask;
	private Vector3 playerSize;
	private Vector3 boxSize;
	private bool grounded = false;

	public LayerMask mask2;
	private Transform pickup;


	private Vector3 colliderOffset;

	private Animator animator;

	void Awake () {
		rb = gameObject.GetComponent<Rigidbody>();
		BoxCollider bc = GetComponent<BoxCollider>();
		playerSize = bc.size;
		colliderOffset = bc.center;
		boxSize = new Vector3(playerSize.x, groundedSkin, playerSize.z);
		animator = GetComponent<Animator>();
	}

	void Update() {
		// Handle jump inputs before handling in FixedUpdate.
		if (Input.GetKeyDown(BOOST_KEY) && grounded) {
			boostRequest = true;
		}
		else if (Input.GetButtonDown("Jump") && grounded) {
			jumpRequest = true;
		}

		if (Input.GetKeyDown(PICKUP_KEY)) {
			pickupRequest = true;
		}
	}

	/*
		Handles player movement, which is partially based off built-in physics. Using FixedUpdate
		because it was the only way to make the 3rd person camera stop being jittery :/
	*/
	void FixedUpdate () {
		if (transform.position.y < respawnElevation) {
			transform.position = jPlayer + (Vector3.up * 5.0f);
		}
		MoveHandler(GetMovementInput());
		JumpHandler();
		PickupHandler();
		updateLastJump();
		float jumpAnimation = Mathf.Clamp(rb.velocity.y * (0.5f / jumpVelocity) + 0.5f, 0, 1);
		animator.SetFloat("verticalVelocity", jumpAnimation);
	}

	void updateLastJump() {
		if (IsSafeGrounded()) {
			jPlayer = transform.position;
		}
	}

	/*
		Returns horizontal and vertical input.	
	*/
	Vector2 GetMovementInput() {
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		return input.normalized;
	}

	/*
		Handles joystick input for walking and running using translation.
	*/
	void MoveHandler(Vector2 inputDirection) {

		// Set player's facing direction (with smoothing).
		if (inputDirection != Vector2.zero) {
			float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothTurnVelocity, smoothTurn);
		}

		// Translate player given input magnitude (with smoothing).
		bool running = Input.GetButton("Run");
		float targetSpeed = (running ? runSpeed : walkSpeed) * inputDirection.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothSpeedVelocity, smoothSpeed);
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

		float animationSpeedPercent = ((running) ? 1 : .5f) * inputDirection.magnitude;
		animator.SetFloat("speedPercent", animationSpeedPercent, smoothSpeed, Time.deltaTime);
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
		Vector3 boxCenter = transform.position + colliderOffset + Vector3.down * (playerSize.y + boxSize.y) * 0.5f;
		grounded = (Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, mask).Length > 0);
		animator.SetBool("grounded", grounded);
	}

	void PickupHandler() {
		if (pickupRequest) {
			if (pickup) {
				// TODO: only drop pickup if nothing is in the way of placement.
				pickup.position = transform.position + transform.forward * playerSize.z + transform.up * playerSize.y / 2;
				Rigidbody pickupRB = pickup.GetComponent<Rigidbody>();
				pickupRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				// NOTE: constant velocity of 2 is additionally added to pickup when letting go. 
				pickupRB.velocity = rb.velocity + transform.forward * (currentSpeed + 2f);
				pickup = null;
			}
			else {
				Vector3 pickupCenter = transform.position + transform.forward * playerSize.z;
				Vector3 pickupSize = playerSize;
				Collider[] pickups = Physics.OverlapBox(pickupCenter, pickupSize, Quaternion.identity, mask2);
				if (pickups.Length > 0) {
					pickup = pickups[0].gameObject.transform;
					pickup.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
					Debug.Log("can pick up: " + pickups[0]);
				}
				else {
					Debug.Log("nothing to pick up");
				}
			}
			pickupRequest = false;
		}
		else if (pickup) {
			pickup.position = transform.position + colliderOffset + transform.up * playerSize.y * 0.75f;
			pickup.rotation = transform.rotation;

		}
	}

	/*
		Returns whether the player is on solid ground.
	*/
	public bool IsGrounded() {
		return grounded;
	}

	private bool IsSafeGrounded() {
		Vector3 right = transform.position;
		right.x += playerSize.x;
		Vector3 left = transform.position;
		left.x -= playerSize.x;
		Vector3 top = transform.position;
		top.z += playerSize.z;
		Vector3 bottom = transform.position;
		bottom.z -= playerSize.z;
		return Physics.Raycast(right, Vector3.down, (playerSize.y + boxSize.y) + groundedSkin, mask) &&
			Physics.Raycast(left, Vector3.down, (playerSize.y + boxSize.y) + groundedSkin, mask) &&
			Physics.Raycast(top, Vector3.down, (playerSize.y + boxSize.y) + groundedSkin, mask) &&
			Physics.Raycast(bottom, Vector3.down, (playerSize.y + boxSize.y) + groundedSkin, mask);
 
	}

}
