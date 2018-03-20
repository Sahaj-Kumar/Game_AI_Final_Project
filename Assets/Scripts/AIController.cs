using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The beast
public class AIController : MonoBehaviour {

	public Transform player;

	private Vector3 gPlayer;
	private Vector3 jAIPlayer;
	private PlayerController playerController;
	private float groundReference;

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

	private bool jumpRequest = false;
	private Vector3 LandingPoint = Vector3.zero;

	private Animator animator;

	void Awake () {
		pathfindingManager = GetComponent<AStarFollow>();
		rb = GetComponent<Rigidbody>();
		groundReference = player.position.y;
		playerController = player.GetComponent<PlayerController>();

		//pathfindingManager.setDestination(gPlayer);

		playerSize = GetComponent<BoxCollider>().size;
		boxSize = new Vector3(playerSize.x, groundedSkin, playerSize.z);
		animator = GetComponent<Animator>();
	}

	void FixedUpdate () {

		MoveHandler(GetDirection());
		updatePlayerPositions();
		float jumpAnimation = Mathf.Clamp(rb.velocity.y * (0.5f / jumpVelocity) + 0.5f, 0, 1);
		animator.SetFloat("verticalVelocity", jumpAnimation);
		JumpHandler();
	}

	/*
		Updates player positions

		gPlayer (Grounded Player):
		Assumes position of player, unless when in midair in which case
		the position is the player's position with the Y position set
		to the player's last grounded Y position. This is used for AStar
		pathfinding to ensure AI can find player while they jump.

		jAIPlayer (Last Jump AI Player):
		Assumes position of player, unless when in midair in which case
		this value is not updated. This is used to respawn location in
		the VERY unlikely chance the AI falls in the void.

	*/
	void updatePlayerPositions() {
		// update grounded player position
		if (playerController.IsGrounded()) {
			gPlayer = player.position;
			groundReference = player.position.y;
		}
		else {
			Vector3 temp = player.position;
			temp.y = groundReference;
			gPlayer = temp;
		}
		// update last jump player position
		if (grounded) {
			jAIPlayer = transform.position;
		}
	}

	/*
		Gets direction for AI to steer towards. Attempts AStar with navigation
		mesh, and if fails reverts to direct naive steering towards player. Takes
		extreme cases to be unreliable in which case the level design would be
		likely be questionable.
	*/
	Vector2 GetDirection() {
		Vector3 direction;
		// mid-air steering to another platform
		if (LandingPoint != Vector3.zero) {
			direction = LandingPoint - transform.position;
		}
		else {
			// attempt AStar using Navmesh
			Vector3 nextWaypoint = pathfindingManager.getNextWaypoint(gPlayer);
			// if solution found, steer to first waypoint
			if (nextWaypoint == Vector3.zero) {
				direction = player.position - transform.position;
			}
			// otherwise use naive steering
			else {
				direction = nextWaypoint - transform.position;
			}
		}
		Vector2 input = new Vector2(direction.x, direction.z).normalized;
		return input; 
	}

	/*
		Given direction, smoothely steers towards target, taking into account
		various behavior radiuses and groundedness.
	*/
	void MoveHandler(Vector2 inputDirection) {
		
		float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(transform.position.x, transform.position.z));

		// Set player's facing direction (with smoothing).
		float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
		transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothTurnVelocity, smoothTurn);

		// determin desiredSpeed given radii and groundedness
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

		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothSpeedVelocity, smoothSpeed);
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

		animator.SetFloat("speedPercent", currentSpeed / runSpeed, smoothSpeed, Time.deltaTime);
	}

	/*
		Handles grounded status as well as jumping in the case a request is made.
	*/
	public void JumpHandler() {
		// Disregard jump if not possible
		if (jumpRequest && !grounded) {
			jumpRequest = false;
		}
		// execute jump
		else if (jumpRequest && grounded) {
			//Debug.Log("AI Jumping!");
			rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
			jumpRequest = false;
		}
		// Update grounded status.
		Vector3 boxCenter = transform.position + Vector3.down * (playerSize.y + boxSize.y) * 0.5f;
		grounded = (Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, mask).Length > 0);
		animator.SetBool("grounded", grounded);
	}

	/*
		Request jump to be made by external script.
	*/
	public void RequestJump() {
		jumpRequest = true;
	}

	/*
		AI groundedness to be checked by external script.
	*/
	public bool IsGrounded() {
		return grounded;
	}

	/*
		Set AI landing point for secure landing on a jump. Called by an external script.
	*/
	public void SetLandingPoint(Vector3 lp) {
		LandingPoint = lp;
	}

	/*
		Clear landing point on successful jump. Called by an external script.
	*/
	public void ClearLandingPoint() {
		LandingPoint = Vector3.zero;
	}
}
