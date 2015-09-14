using UnityEngine;
using System.Collections;

public class FollowRobot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Init.robotInstance == null)
			return;
		Transform theRobot = (Transform)Init.robotInstance;
		
		transform.position = new Vector3(theRobot.position.x, Mathf.RoundToInt(transform.position.y), theRobot.position.z - UI.pan);
		transform.LookAt(theRobot);
	}
}
