using UnityEngine;
using System.Collections;

public class RotateHead : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (UI.currentCamera == UI.CAMERA_ONBOARD)
			transform.localRotation = Quaternion.Euler(0f, UI.pan * 4, 0f);
		else
			transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
