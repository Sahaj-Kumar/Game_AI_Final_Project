using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour {

	public GameObject[] eventObjects;
	public GameEvent[] events;
	public LayerMask[] relevantTo;
	private List<int> relevantToValues;
	private int occupied;

	public AdditionalTrigger[] otherTriggers;

	public bool oneTime;

	void Start () {
		if (eventObjects.Length != events.Length) {
			Debug.LogError("Trigger Event does not have even parameters: " + gameObject);
		}
		relevantToValues = new List<int>();
		foreach (LayerMask lm in relevantTo) {
			relevantToValues.Add(lm.value);
		}
		foreach (AdditionalTrigger at in otherTriggers) {
			at.AssignRelevantValues(relevantToValues, this);
		}

	}


	void OnTriggerEnter(Collider other) {
		
		int layerID = (int) Mathf.Pow(2, other.gameObject.layer);
		if (relevantToValues.Contains(layerID)) {
			occupied++;
			FOO();
		}
    }

	void OnTriggerExit(Collider other) {
		int layerID = (int) Mathf.Pow(2, other.gameObject.layer);
		if (relevantToValues.Contains(layerID)) {
			occupied--;
		}
    }

    public void FOO() {
    	if (occupied > 0) {
    		bool allOccupied = true;
    		foreach (AdditionalTrigger at in otherTriggers) {
    			if (at.Occupied() <= 0) {
    				allOccupied = false;
    			}
    		}
    		if (allOccupied) {
				int eventNumber = eventObjects.Length;
				for (int i = 0; i < eventNumber; i++) {
				StartCoroutine(events[i].getEnumerator(eventObjects[i]));
				}
				if (oneTime) {
					gameObject.GetComponent<BoxCollider>().enabled = false;
					foreach (AdditionalTrigger at in otherTriggers) {
						at.GetComponent<BoxCollider>().enabled = false;
					}
				}
    		}
    	}
    }
}
