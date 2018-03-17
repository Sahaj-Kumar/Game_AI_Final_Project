using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour {

	private BoxCollider zone1;
	private int maskID;
	//public BoxCollider zone2;
	//public AIController AIPlayer;
	//public LayerMask mask;

	// Use this for initialization
	void Start () {
		//zone1 = gameObject.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log(LayerMask.GetMask("AI"));
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			//AIPlayer.useAStar = false;
			other.gameObject.GetComponent<AIController>().useAStar = true;
		}
		Debug.Log("Entered: " + other.gameObject);
	}

	void OnTriggerExit(Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer).Equals("AI")) {
			//AIPlayer.useAStar = true;
			other.gameObject.GetComponent<AIController>().useAStar = false;
		}
	}
}
