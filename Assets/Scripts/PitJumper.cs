using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitJumper : MonoBehaviour {

	//public AIController AIPlayer;
	private BoxCollider boxCollider;
	public Vector3 jumpVector;
	public LayerMask mask;
	public bool leftJump = true;
	public bool rightJump = true;

	private Vector3 LeftBoxCenter;
	private Quaternion LeftBoxRotation;
	private Vector3 LeftBoxSize;


	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider>();
		// configure left half of pit
		LeftBoxCenter = transform.position;
		LeftBoxRotation = transform.rotation;
		LeftBoxCenter -= transform.right * boxCollider.size.x * 0.25f;
		LeftBoxSize = new Vector3(boxCollider.size.x / 2.0f, boxCollider.size.y, boxCollider.size.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			bool onLeft = Physics.OverlapBox(LeftBoxCenter, LeftBoxSize, LeftBoxRotation, mask).Length > 0;
			AIController AIPlayer = other.gameObject.GetComponent<AIController>();
			if ((onLeft && rightJump) || (!onLeft && leftJump)) {
				AIPlayer.RequestJump();
			}
			Vector3 otherSide = AIPlayer.transform.position;
			otherSide += onLeft ? jumpVector : -jumpVector;
			AIPlayer.SetLandingPoint(otherSide);
		}
	}

	IEnumerator FinishJump(AIController AIPlayer) {
		//yield return new WaitForSeconds(0.1f);
		Debug.Log("Exiting pit");
		while (true) {
			yield return new WaitForSeconds(0.05f);
			if (AIPlayer.IsGrounded()) {
				break;
			}
		}
		AIPlayer.ClearLandingPoint();
	}

	void OnTriggerExit(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			
			//AIPlayer.LandingPoint = Vector3.zero;
			StartCoroutine(FinishJump(other.gameObject.GetComponent<AIController>()));
		}
	}
}
