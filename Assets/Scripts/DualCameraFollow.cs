using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualCameraFollow : MonoBehaviour {

	public Transform target1;
	public Transform target2;
	[Range(0, 1)]
	public float targetBias = 0.5f; // smoothly updates only in FixedUpdate
	[Tooltip("Offset vector normalized at runtime")]
	public Vector3 offsetVector;
	public float minZoom;
	public float distanceFactor = 1.0f;
	public float smoothFactor = 0.125f;

	private Vector3 velocity = Vector3.zero;
	private KeyCode ZOOM_IN_KEY = KeyCode.J;
	private KeyCode ZOOM_OUT_KEY = KeyCode.U;

	void Start () {
		offsetVector.Normalize();
	}

	void Update () {
		if (Input.GetKeyDown(ZOOM_IN_KEY)) {
			minZoom = Mathf.Clamp(minZoom - 1, 0, Mathf.Infinity);
		}
		else if (Input.GetKeyDown(ZOOM_OUT_KEY)) {
			minZoom++;
		}
	}

	// NOTE: If target(s) movement is NOT reliant on built-in physics, use LateUpdate instead of FixedUpdate.
	void FixedUpdate () {
			float distance = Vector3.Distance(target1.position, target2.position);
			Vector3 midPoint = Vector3.Lerp(target1.position, target2.position, targetBias);
			Vector3 offset = offsetVector * Mathf.Clamp(distance * distanceFactor * Mathf.Sin(targetBias * Mathf.PI), minZoom, Mathf.Infinity);
			Vector3 desiredPosition =  midPoint + offset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothFactor * Time.deltaTime);
			transform.position = smoothedPosition;
			transform.LookAt(midPoint);
	}
}
