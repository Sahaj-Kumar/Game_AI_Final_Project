using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitJumper : MonoBehaviour {

	public AIController AIPlayer;
	private BoxCollider boxCollider;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			Debug.Log("Entering pit");
			AIPlayer.jumpRequest = true;
			Vector3 otherSide = AIPlayer.transform.position;
			otherSide.x += boxCollider.size.x * ((transform.position.x > AIPlayer.transform.position.x) ? 2 : -2);
			Debug.Log(otherSide);
			AIPlayer.LandingPoint = otherSide;
		}
	}

	void OnTriggerExit(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			Debug.Log("Exiting pit");
			AIPlayer.LandingPoint = Vector3.zero;
		}
	}
}
