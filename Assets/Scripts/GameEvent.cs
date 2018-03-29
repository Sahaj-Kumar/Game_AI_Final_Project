using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameEvent : ScriptableObject {

	public abstract IEnumerator getEnumerator(GameObject eventObject);

}

[CreateAssetMenu(fileName = "New Rotation Event", menuName = "RotationEvent")]
public class ChangeRotation : GameEvent  {

	public Quaternion newRotation;
	public float time;

	/*
	ChangeRotation(float time, Quaternion newRotation) {
		this.newRotation = newRotation;
		this.time = time;
	}
	*/
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return SetNewRotation(eventObject, time);
	}

	private IEnumerator SetNewRotation(GameObject eventObject, float time) {
    	float percent = 0f;
    	Quaternion startRotation = eventObject.transform.rotation;
    	while (percent < 1) {
	   		eventObject.transform.rotation = Quaternion.Lerp(startRotation, newRotation, percent);
	   		percent += Time.deltaTime / time;
	    	Debug.Log(percent);
	    	yield return null;
	    }
	    eventObject.transform.rotation = newRotation;
	}
}

[CreateAssetMenu(fileName = "New Position Event", menuName = "PositionEvent")]
public class ChangePosition : GameEvent  {

	public Vector3 positionChange;
	public float time;
	/*
	ChangePosition(float time, Vector3 positionChange) {
		this.positionChange = positionChange;
		this.time = time;
	}
	*/
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return SetNewPosition(eventObject, time);
	}

	private IEnumerator SetNewPosition(GameObject eventObject, float time) {
    	float percent = 0f;
    	Vector3 startPosition = eventObject.transform.position;
    	Vector3 endPosition = startPosition + positionChange;
    	while (percent < 1) {
	   		eventObject.transform.position = Vector3.Lerp(startPosition, endPosition, percent);
	   		percent += Time.deltaTime / time;
	    	Debug.Log(percent);
	    	yield return null;
	    }
	    eventObject.transform.position = endPosition;
	}
}

[CreateAssetMenu(fileName = "New Set Trigger Event", menuName = "SetTriggerEvent")]
public class SetTrigger : GameEvent {

	public bool setting;
	/*
	SetTrigger(bool setting) {
		this.setting = setting;
	}
	*/
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return EnableTrigger(eventObject);
	}

	private IEnumerator EnableTrigger(GameObject eventObject) {
		BoxCollider bc = eventObject.GetComponent<BoxCollider>();
		if (bc) {
			bc.enabled = setting;
		}
		else {
			Debug.LogError(eventObject + ": does not have box collider component");
		}
		yield return null;
	}

}

[CreateAssetMenu(fileName = "New Spawn Object Event", menuName = "SpawnObjectEvent")]
public class SpawnObject : GameEvent {

	public float delay;
	public Vector3 location;
	/*
	SetTrigger(bool setting) {
		this.setting = setting;
	}
	*/
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return EnableTrigger(eventObject);
	}

	private IEnumerator EnableTrigger(GameObject eventObject) {
		yield return new WaitForSeconds(delay);
		Instantiate(eventObject).transform.position = location;
	}
}

[CreateAssetMenu(fileName = "New AI Steer Event", menuName = "AISteerEvent")]
public class AISteer : GameEvent {

	public float delay;
	public bool setting;

	/*
	SetTrigger(bool setting) {
		this.setting = setting;
	}
	*/
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return EnableTrigger(eventObject);
	}

	private IEnumerator EnableTrigger(GameObject eventObject) {
		yield return new WaitForSeconds(delay);
		AIController controller = eventObject.GetComponent<AIController>();
		controller.SetSeeking(setting);
	}
}

[System.Serializable]
public class TestWoah : GameEvent {

	public float delay;
	public bool setting;
	public override IEnumerator getEnumerator(GameObject eventObject) {
		return EnableTrigger(eventObject);
	}

	private IEnumerator EnableTrigger(GameObject eventObject) {
		yield return new WaitForSeconds(delay);
		AIController controller = eventObject.GetComponent<AIController>();
		controller.SetSeeking(setting);
	}
}

