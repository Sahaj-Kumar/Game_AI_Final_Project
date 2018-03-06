using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public Vector3 positionOffset;
	public float smoothFactor = 0.125f;

	private Vector3 velocity = Vector3.zero;

	// NOTE: If target movement is NOT reliant on in-built, use LateUpdate rather than FixedUpdate.
	void FixedUpdate () {

			Vector3 desiredPosition = target.position + positionOffset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothFactor * Time.deltaTime);
			transform.position = smoothedPosition;
			transform.LookAt(target);

	}
}
