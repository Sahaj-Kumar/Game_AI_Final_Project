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
			return currentPath.corners[1];
		}
		else {
			Debug.LogError(":(");
			return transform.position;
		}
	}
}
