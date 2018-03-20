using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

	private Transform target;
	public string message;
	public Vector3 offset;


	// Use this for initialization
	void Start () {
		//GetComponentInChildren<Text>().text = message;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = target.position + offset;
	}

	public void Setup(Transform targ, string message) {
		target = targ;
		GetComponentInChildren<Text>().text = message;
	}
}
