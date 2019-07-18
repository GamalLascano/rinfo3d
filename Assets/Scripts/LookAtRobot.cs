using UnityEngine;
using System.Collections;

public class LookAtRobot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Init.robotInstance == null)
			return;
		Transform theRobot = (Transform)Init.robotInstance;

		Transform target = theRobot.Find("CuerpoRobot").Find("CabezaRobot");
		transform.position = new Vector3(theRobot.position.x - 3 + UI.pan, Mathf.RoundToInt(transform.position.y), theRobot.position.z - 3  - UI.pan);
		transform.LookAt(target);
	}
}
