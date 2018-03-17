using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitJumper : MonoBehaviour {

	//public AIController AIPlayer;
	private BoxCollider boxCollider;
	public Vector3 jumpVector;
	public LayerMask mask;

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
			Debug.Log("Entering pit");
			AIController AIPlayer = other.gameObject.GetComponent<AIController>();
			AIPlayer.jumpRequest = true;
			Vector3 otherSide = AIPlayer.transform.position;
			//otherSide.x += boxCollider.size.x * ((Physics.OverlapBox(LeftBoxCenter, LeftBoxSize, LeftBoxRotation, mask).Length > 0) ? 2 : -2);
			otherSide += (Physics.OverlapBox(LeftBoxCenter, LeftBoxSize, LeftBoxRotation, mask).Length > 0) ? jumpVector : -jumpVector;
			Debug.Log(otherSide);
			AIPlayer.LandingPoint = otherSide;
		}
	}

	IEnumerator FinishJump(AIController AIPlayer) {
		yield return new WaitForSeconds(0.1f);
		Debug.Log("Exiting pit");

		AIPlayer.LandingPoint = Vector3.zero;
	}

	void OnTriggerExit(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			
			//AIPlayer.LandingPoint = Vector3.zero;
			StartCoroutine(FinishJump(other.gameObject.GetComponent<AIController>()));
		}
	}
}
