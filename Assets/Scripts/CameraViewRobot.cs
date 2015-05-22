using UnityEngine;
using System.Collections;

public class CameraViewRobot : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Init.robotInstance == null)
			return;
		transform.LookAt ((Transform)Init.robotInstance);
		transform.position = new Vector3 (((Transform)Init.robotInstance).position.x - 3,
		                                  transform.position.y,
		                                  ((Transform)Init.robotInstance).position.z - 3);
	}
}
