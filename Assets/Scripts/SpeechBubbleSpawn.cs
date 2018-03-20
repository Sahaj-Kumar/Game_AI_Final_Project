using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbleSpawn : MonoBehaviour {

	public SpeechBubble speechBubble;
	public Transform target;

	// Use this for initialization
	void Start () {
		StartCoroutine(spawnSpeedBubble());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator spawnSpeedBubble() {
		yield return new WaitForSeconds(2f);
		SpeechBubble foo = Instantiate(speechBubble);
		foo.Setup(target, "Hi Sahaj");
	}
}
