using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public Vector3 positionOffset;
	public Vector3 rotationOffset;
	public float smoothFactor = 0.125f;

	private Vector3 velocity = Vector3.zero;

	void LateUpdate () {

			Vector3 desiredPosition = target.position + positionOffset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothFactor);
			transform.position = smoothedPosition;
			//transform.eulerAngles = rotationOffset;
			transform.LookAt(target);
	}
}
