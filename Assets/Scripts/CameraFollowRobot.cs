using UnityEngine;
using System.Collections;

public class CameraFollowRobot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Init.robotInstance == null)
			return;
		transform.position = new Vector3 (((Transform)Init.robotInstance).position.x, transform.position.y, ((Transform)Init.robotInstance).position.z);
	}
}
