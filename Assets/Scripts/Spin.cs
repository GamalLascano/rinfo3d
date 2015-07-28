using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(target.position, ((Transform)Init.robotInstance).position) > 5)
			return;

		target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.x,
		                                        target.localRotation.eulerAngles.y + 90f * Time.deltaTime,
		                                        target.localRotation.eulerAngles.z);
	}
}
