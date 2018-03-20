using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AStarFollow : MonoBehaviour {

	public Transform destination;

	//NavMeshAgent navMeshAgent;
	NavMeshPath currentPath;


	// Use this for initialization
	void Start () {
		
		//navMeshAgent = GetComponent<NavMeshAgent>();
		currentPath = new NavMeshPath();
		//if (navMeshAgent == null) {
		//	Debug.LogError("No Nav Mesh Agent on " + gameObject);
		//}

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void setDestination(Transform dest) {
		destination = dest;
	}

	public Vector3 getNextWaypoint() {
		if (destination != null) {
			NavMesh.CalculatePath(transform.position, destination.position, NavMesh.AllAreas, currentPath);
			//Debug.Log(currentPath.status);
			if (currentPath.corners.Length >= 2) {
				return currentPath.corners[1];
			}
			else {
				Debug.Log("No path available");
				return Vector3.zero;
			}
		}
		else {
			Debug.LogError("Destination not set");
			return transform.position;
		}
	}

	public Vector3 getNextWaypoint(Vector3 destin) {
		NavMesh.CalculatePath(transform.position, destin, NavMesh.AllAreas, currentPath);
		if (currentPath.corners.Length >= 2) {
			return currentPath.corners[1];
		}
		else {
			Debug.Log("No path availabe to " + destin);
			return Vector3.zero;
		}
	}
}
