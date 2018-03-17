using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The beast
public class AIController : MonoBehaviour {

	public Transform player;
	public Transform gPlayer;
	public float closeRadius = 12.0f;
	public float slowRadius = 8.0f;
	public float acceptanceRadius = 5.0f;

	public float walkSpeed = 5.0f;
	public float runSpeed = 10.0f;
	public float smoothTurn = 0.1f;
	public float standingSmoothTurn = 0.2f;
	public float smoothSpeed = 0.25f;

	private AStarFollow pathfindingManager;

	private float currentSpeed;
	private float smoothTurnVelocity;
	private float smoothSpeedVelocity;

	public float jumpVelocity = 10.0f;
	public float groundedSkin = 0.05f;
	private Vector3 playerSize;
	private Rigidbody rb;
	public LayerMask mask;
	private Vector3 boxSize;
	private bool grounded = true;

	public bool useAStar = true;
	public bool jumpRequest = false;
	public Vector3 LandingPoint = Vector3.zero;

	private Animator animator;

	void Awake () {
		pathfindingManager = GetComponent<AStarFollow>();
		rb = GetComponent<Rigidbody>();

		pathfindingManager.setDestination(gPlayer);

		playerSize = GetComponent<BoxCollider>().size;
		boxSize = new Vector3(playerSize.x, groundedSkin, playerSize.z);
		animator = GetComponent<Animator>();
	}

	void FixedUpdate () {

		
		if (!jumpRequest || (jumpRequest && grounded)) {
			if (useAStar) {
				Vector3 direction = (pathfindingManager.getNextWaypoint() - transform.position);
				Vector2	input = new Vector2(direction.x, direction.z).normalized;
				MoveHandler(input);
			}
			else {
				Vector3 direction;
				if (LandingPoint != Vector3.zero) {
					Debug.Log("Using land point");
					direction = LandingPoint - transform.position;
				}
				else {
					direction = player.position - transform.position;
				}

				Vector2	input = new Vector2(direction.x, direction.z).normalized;
				MoveHandler(input);
				Debug.Log("AI in zone!");
			}
		}
		JumpRequest();
	}

	void MoveHandler(Vector2 inputDirection) {
		// Set player's facing direction (with smoothing).

		float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(transform.position.x, transform.position.z));

		float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
		transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothTurnVelocity, smoothTurn);
		//float targetSpeed = (moving ? (running ? runSpeed : walkSpeed) : 0) * inputDirection.magnitude;

		float targetSpeed;
		if (distance > closeRadius) {
			targetSpeed = runSpeed;
		}
		else if (distance > slowRadius) {
			float ratio = (distance - slowRadius) / (closeRadius - slowRadius);
			targetSpeed = walkSpeed + (runSpeed - walkSpeed) * ratio;
		}
		else if (distance > acceptanceRadius) {
			targetSpeed = walkSpeed;
		}
		else {
			targetSpeed = 0;
		}

		if (!grounded) {
			targetSpeed = runSpeed;
		}

		//Debug.Log(targetSpeed);
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothSpeedVelocity, smoothSpeed);
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

		animator.SetFloat("speedPercent", currentSpeed / runSpeed, smoothSpeed, Time.deltaTime);
	}

	public void JumpRequest() {
		// Execute jump if permitted.
		if (jumpRequest && grounded) {
			Debug.Log("AI Jumping!");
			rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
			jumpRequest = false;
		}
		// Update grounded status.
		Vector3 boxCenter = transform.position + Vector3.down * (playerSize.y + boxSize.y) * 0.5f;
		grounded = (Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, mask).Length > 0);
	}

}
