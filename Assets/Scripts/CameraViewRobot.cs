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
	}
}
