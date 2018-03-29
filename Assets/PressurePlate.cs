using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {

	private Material inactive;
	public Material active;

	private int occupied = 0;

	void Start () {
		inactive = GetComponent<Renderer>().material;
	}

	void OnTriggerEnter(Collider other) {
		occupied++;
		if (occupied >= 1) {
			GetComponent<Renderer>().material = active;
		}
    }

	void OnTriggerExit(Collider other) {
		occupied--;
		if (occupied <= 0) {
			GetComponent<Renderer>().material = inactive;
		}
    }
}
