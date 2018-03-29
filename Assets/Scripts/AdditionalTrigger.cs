using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalTrigger : MonoBehaviour {

	private TriggerEvent masterTrigger;

	private List<int> relevantToValues;
	private int occupied = 0;


	void Start () {

	}

	void OnTriggerEnter(Collider other) {
		int layerID = (int) Mathf.Pow(2, other.gameObject.layer);
		if (relevantToValues.Contains(layerID)) {
			occupied++;
			masterTrigger.FOO();
		}
    }

	void OnTriggerExit(Collider other) {
		int layerID = (int) Mathf.Pow(2, other.gameObject.layer);
		if (relevantToValues.Contains(layerID)) {
			occupied--;
		}
    }

    public void AssignRelevantValues(List<int> vals, TriggerEvent te) {
    	relevantToValues = vals;
    	masterTrigger = te;
    }

    public int Occupied() {
    	return occupied;
    }
}
